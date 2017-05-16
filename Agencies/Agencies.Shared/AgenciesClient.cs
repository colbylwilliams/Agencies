using System;

using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NomadCode.Auth;
using NomadCode.BotFramework;

namespace Agencies.Shared
{
    public class AgenciesClient
    {
        const string conversationIdKey = "conversationId";

        static AgenciesClient _shared;
        public static AgenciesClient Shared => _shared ?? (_shared = new AgenciesClient ());

        public AuthUserConfig AuthUser { get; set; }

        HttpClient _httpClient;
        HttpClient httpClient => _httpClient ?? (_httpClient = new HttpClient { BaseAddress = new Uri ("https://digital-agencies-functions.azurewebsites.net/") });


        AgenciesClient ()
        {
        }


        public async Task<Conversation> GetConversationAsync (string conversationId = null)
        {
            if (AuthUser == null)
            {
                throw new InvalidOperationException ("Must call GetAuthUserConfigAsync before calling this method");
            }

            try
            {
                var conversationJson = await httpClient.GetStringAsync ($"api/tokens/bot/{conversationId}");

                Log.Debug ($"conversationJson: {conversationJson}");

                var conversation = JsonConvert.DeserializeObject<Conversation> (conversationJson);

                return conversation;
            }
            catch (Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }

        public async Task<string> GetFaceApiTokenAsync ()
        {
            if (AuthUser == null)
            {
                throw new InvalidOperationException ("Must call GetAuthUserConfigAsync before calling this method");
            }

            try
            {
                var faceApiToken = await httpClient.GetStringAsync ("api/tokens/face");

                Log.Debug ($"Token: {faceApiToken}");

                return faceApiToken;
            }
            catch (Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }


        public async Task<AuthUserConfig> GetAuthUserConfigAsync ()
        {
            try
            {
                var keychain = new Keychain ();

                var storedKeys = keychain.GetItemFromKeychain (AzureAppServiceUser.AuthenticationHeader);

                if (!string.IsNullOrEmpty (storedKeys.Account) && !string.IsNullOrEmpty (storedKeys.PrivateKey))
                {
                    httpClient.DefaultRequestHeaders.Remove (AzureAppServiceUser.AuthenticationHeader);

                    httpClient.DefaultRequestHeaders.Add (AzureAppServiceUser.AuthenticationHeader, storedKeys.PrivateKey);

                    var userConfigJson = await httpClient.GetStringAsync ("api/user/config");

                    Log.Debug ($"userConfigJson {userConfigJson}");

                    AuthUser = JsonConvert.DeserializeObject<AuthUserConfig> (userConfigJson);

                    return AuthUser;
                }

                return null;
            }
            catch (HttpRequestException reEx)
            {
                if (reEx.Message.Contains ("401"))
                {
                    new Keychain ().RemoveItemFromKeychain (AzureAppServiceUser.AuthenticationHeader);

                    return null;
                }

                Log.Error (reEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }


        public async Task<AuthUserConfig> GetAuthUserConfigAsync (string providerToken, string providerAuthCode)
        {
            try
            {
                if (!string.IsNullOrEmpty (providerToken) && !string.IsNullOrEmpty (providerAuthCode))
                {
                    var auth = JObject.Parse ($"{{'id_token':'{providerToken}','authorization_code':'{providerAuthCode}'}}").ToString ();

                    var authResponse = await httpClient.PostAsync (".auth/login/google", new StringContent (auth, Encoding.UTF8, "application/json"));

                    if (authResponse.IsSuccessStatusCode)
                    {
                        var azureUserJson = await authResponse.Content.ReadAsStringAsync ();

                        Log.Debug ($"azureUserJson: {azureUserJson}");

                        var azureUser = JsonConvert.DeserializeObject<AzureAppServiceUser> (azureUserJson);


                        Log.Debug ($"azureUser.AuthenticationToken {azureUser.AuthenticationToken}");

                        httpClient.DefaultRequestHeaders.Remove (AzureAppServiceUser.AuthenticationHeader);

                        httpClient.DefaultRequestHeaders.Add (AzureAppServiceUser.AuthenticationHeader, azureUser.AuthenticationToken);

                        var keychain = new Keychain ();

                        keychain.SaveItemToKeychain (AzureAppServiceUser.AuthenticationHeader, "azure", azureUser.AuthenticationToken);

                        var userConfigJson = await httpClient.GetStringAsync ("api/user/config");

                        Log.Debug ($"userConfigJson {userConfigJson}");

                        AuthUser = JsonConvert.DeserializeObject<AuthUserConfig> (userConfigJson);

                        return AuthUser;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }
    }
}

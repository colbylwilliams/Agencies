using System;

using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NomadCode.Auth;
using NomadCode.Azure;
using NomadCode.BotFramework;

namespace Agencies.Shared
{
    public class AgenciesClient
    {
        const string conversationIdKey = "conversationId";

        static AgenciesClient _shared;
        public static AgenciesClient Shared => _shared ?? (_shared = new AgenciesClient ());

        AzureClient azureClient => AzureClient.Shared;

        HttpClient _httpClient;
        HttpClient httpClient => _httpClient ?? (_httpClient = new HttpClient { BaseAddress = new Uri ("https://digital-agencies-functions.azurewebsites.net/") });

        public AuthUserConfig AuthUser { get; set; }

        AgenciesClient ()
        {
        }


        public async Task<Conversation> GetConversation (string conversationId = null)
        {
            try
            {
                var paramDictionary = new Dictionary<string, string> ();

                if (!string.IsNullOrEmpty (conversationId))
                {
                    paramDictionary.Add (conversationIdKey, conversationId);
                }

                var channel = await azureClient.InvokeApiAsync<Conversation> ("getBotToken", HttpMethod.Get, paramDictionary);

                return channel;
            }
            catch (Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }


        public async Task<string> GetFaceApiSubscription ()
        {
            try
            {
                var paramDictionary = new Dictionary<string, string> ();

                var key = await azureClient.InvokeApiAsync<string> ("getFaceToken", HttpMethod.Get, paramDictionary);

                return key;
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

                        httpClient.DefaultRequestHeaders.Add (AzureAppServiceUser.AuthenticationHeader, azureUser.AuthenticationToken);

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

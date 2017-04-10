using System;
using NomadCode.Azure;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;

namespace Agencies.Shared
{
    public class AgenciesClient
    {

        static AgenciesClient _shared;
        public static AgenciesClient Shared => _shared ?? (_shared = new AgenciesClient());

        AzureClient azureClient => AzureClient.Shared;

        AgenciesClient()
        {
        }

        public async Task<string> GetInitialConversationToken()
        {
            try
            {
                var paramDictionary = new Dictionary<string, string>();// { { StorageToken.ContentIdParam, avContent.Id } };

                var storageToken = await azureClient.MobileServiceClient.InvokeApiAsync<string>("getBotToken", HttpMethod.Get, paramDictionary);

                return storageToken;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }
    }
}

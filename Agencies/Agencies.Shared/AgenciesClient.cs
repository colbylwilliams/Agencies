using System;
using NomadCode.Azure;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Bot.Connector.DirectLine;

namespace Agencies.Shared
{
    public class AgenciesClient
    {
        const string conversationIdKey = "conversationId";

        static AgenciesClient _shared;
        public static AgenciesClient Shared => _shared ?? (_shared = new AgenciesClient ());

        AzureClient azureClient => AzureClient.Shared;

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
    }
}

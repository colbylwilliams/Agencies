using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using Microsoft.Bot.Connector.DirectLine;

namespace Agencies.Functions
{
	public static class BotTokenGenerator
	{
		static string _botDirectLineSecret;
		static string botDirectLineSecret => _botDirectLineSecret ?? (_botDirectLineSecret = Environment.GetEnvironmentVariable ("MS_BotDirectLineSecret"));

		// [FunctionName("GetBotToken")]
		// public static async Task<HttpResponseMessage> GetBotToken(
		//    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tokens/bot/{conversationId=}")]HttpRequestMessage req,
		//    [Bot] IDirectLineClient client, string conversationId, TraceWriter log)

		// https://github.com/Azure/azure-webjobs-sdk-script/issues/1507
		// This bug incorrectly sets the bot client's binding direction to 'out'
		// as a workaround we're explicitly providing the function.json which means
		// we can't us attribute-binding at all in this funciton.
		[FunctionName ("GetBotToken")]
		public static async Task<HttpResponseMessage> GetBotToken ([HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "tokens/bot/{conversationId=}")]HttpRequestMessage req, /*IDirectLineClient client,*/ string conversationId, TraceWriter log)
		{
			var client = new DirectLineClient (botDirectLineSecret);

			if (!string.IsNullOrEmpty (conversationId))
			{
				var conversation = await client.Conversations.ReconnectToConversationAsync (conversationId);

				if (conversation != null)
				{
					return req.CreateResponse (HttpStatusCode.OK, conversation);
				}
			}

			return req.CreateResponse (HttpStatusCode.OK, await client.Tokens.GenerateTokenForNewConversationAsync ());
		}
	}
}
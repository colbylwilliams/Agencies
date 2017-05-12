using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.BotFramework;

using Microsoft.Bot.Connector.DirectLine;

namespace Agencies.Functions
{
	public static class BotTokenGenerator
	{
		[FunctionName ("GetBotToken")]
		public static async Task<HttpResponseMessage> GetBotToken (
			[HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "tokens/bot/{conversationId=}")]HttpRequestMessage req,
			/*[Bot] IDirectLineClient client*/Binder binder, string conversationId, TraceWriter log)
		{
			// Attempt to workaround https://github.com/Microsoft/BotBuilder-WebJobs-BotExtension/issues/5
			var client = await binder.BindAsync<IDirectLineClient>(new BotAttribute());

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
using Agencies.AppService.Models;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace Agencies.AppService.Controllers
{
	//[BotAuthentication]
	[MobileAppController]
	public class GetBotTokenController : ApiController
	{
		const string conversationIdKey = "conversationId";

		AgenciesContext context;

		DirectLineClient client;

		public string BotDirectLineSecret { get; }

		const string connString = "MS_BotDirectLineSecret";

		public GetBotTokenController()
		{
			context = new AgenciesContext ();

			BotDirectLineSecret = ConfigurationManager.AppSettings[connString];

			client = new DirectLineClient(BotDirectLineSecret);
		}


		// GET api/GetBotToken
		[HttpGet]
#if !DEBUG
		[Authorize]
#endif
		public async Task<Conversation> GetAsync()
		{
			var conversationId = Request.GetQueryNameValuePairs().FirstOrDefault(kv => kv.Key == conversationIdKey).Value;

			if (!string.IsNullOrEmpty(conversationId))
			{
				var conversation = await client.Conversations.ReconnectToConversationAsync(conversationId);

				if (conversation != null)
				{
					return conversation;
				}
			}

			return await client.Tokens.GenerateTokenForNewConversationAsync();
		}
	}
}
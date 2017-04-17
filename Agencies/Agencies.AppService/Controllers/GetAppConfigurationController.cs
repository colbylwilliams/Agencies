using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Bot.Connector.DirectLine;

using Agencies.AppService.Models;
using Agencies.Domain;

namespace Agencies.AppService.Controllers
{
	[MobileAppController]
	public class GetAppConfigurationController : ApiController
	{
		const string conversationIdKey = "conversationId";

		const string faceApiSubscriptionKey = "MS_FaceApiSubscriptionKey";
		const string botDirectLineSecretKey = "MS_BotDirectLineSecret";
		//const string mobileCenterAppSecretIosKey = "MS_MobileCenterAppSecret_Ios";
		//const string mobileCenterAppSecretAndroidKey = "MS_MobileCenterAppSecret_Android";

		AgenciesContext context;

		DirectLineClient botDirectLineClient;

		string faceApiSubscription;
		string botDirectLineSecret;
		//string mobileCenterAppSecretIos;
		//string mobileCenterAppSecretAndroid;

		public GetAppConfigurationController()
		{
			context = new AgenciesContext();

			faceApiSubscription = ConfigurationManager.AppSettings[faceApiSubscriptionKey];
			botDirectLineSecret = ConfigurationManager.AppSettings[botDirectLineSecretKey];
			//mobileCenterAppSecretIos = ConfigurationManager.AppSettings[mobileCenterAppSecretIosKey];
			//mobileCenterAppSecretAndroid = ConfigurationManager.AppSettings[mobileCenterAppSecretAndroidKey];

			botDirectLineClient = new DirectLineClient(botDirectLineSecret);
		}

		// GET api/GetAppConfiguration
		[HttpGet]
#if !DEBUG
		[Authorize]
#endif
		public async Task<AgenciesAppConfiguration> GetAsync()
		{
			var config = new AgenciesAppConfiguration
			{
				FaceApiSubscriptionKey = faceApiSubscription,
				//MobileCenterAppSecretIos = mobileCenterAppSecretIos,
				//MobileCenterAppSecretAndroid = mobileCenterAppSecretAndroid
			};

			var conversationId = Request.GetQueryNameValuePairs().FirstOrDefault(kv => kv.Key == conversationIdKey).Value;

			if (!string.IsNullOrEmpty(conversationId))
			{
				var conversation = await botDirectLineClient.Conversations.ReconnectToConversationAsync(conversationId);

				if (conversation == null)
				{
					conversation = await botDirectLineClient.Tokens.GenerateTokenForNewConversationAsync();
				}

				config.Convnersation = conversation;
			}


			return config;
		}
	}
}
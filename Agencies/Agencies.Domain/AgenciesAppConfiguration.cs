using Microsoft.Bot.Connector.DirectLine;

namespace Agencies.Domain
{
    public class AgenciesAppConfiguration
    {
		public string FaceApiSubscriptionKey { get; set; }

		//public string MobileCenterAppSecretIos { get; set; }

		//public string MobileCenterAppSecretAndroid { get; set; }

		public Conversation Convnersation { get; set; }
	}
}

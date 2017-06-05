namespace Agencies
{
	public static partial class Keys
	{
		public static partial class MobileCenter
		{
#if __IOS__
			public const string AppSecret = @"";
#elif __ANDROID__
			public const string AppSecret = @"";
#endif
		}

		public static partial class Azure
		{
			const string serviceUrl = @"https://digital-agencies-functions.azurewebsites.net/";

#if DEBUG
			const string localServiceUrl = @"http://localhost:7071/";

			public static string ServiceUrl => SettingsStudio.Settings.UseLocalServer ? localServiceUrl : serviceUrl;
#else
			public static string ServiceUrl => serviceUrl;
#endif
			public static string AuthUrl => serviceUrl;

			public static partial class Storage
			{
				public const string AccountKey = @"";

				public const string AccountName = @"";

				public const string EndpointSuffix = @"core.windows.net";

				public static string ConnectionString = $"DefaultEndpointsProtocol=https;AccountName={AccountName};AccountKey={AccountKey};EndpointSuffix={EndpointSuffix}";

				public static string BaseUrl = $"https://{AccountName}.blob.{EndpointSuffix}";
			}
		}

		public static partial class CognitiveServices
		{
			public static partial class FaceApi
			{
				//public const string SubscriptionKey = @"";
			}
		}

		public static partial class Google
		{
			//public const string ServerClientId = @"";

			//public const string ClientId = @"";
		}
	}
}

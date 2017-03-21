namespace Agencies
{
	public static partial class Keys
	{
		public static class MobileCenter
		{
#if __IOS__
			public const string AppSecret = @"";
#elif __ANDROID__
			public const string AppSecret = @"";
#endif
		}

		public static class Azure
		{
#if DEBUG
			// public const string ServiceUrl = @"http://10.0.0.221:55521/";
			public const string ServiceUrl = @"https://digital-agencies.azurewebsites.net/";
#else
			public const string ServiceUrl = @"https://digital-agencies.azurewebsites.net/";
#endif
			public static class Storage
			{
				const string accountKey = @"";

				public const string AccountName = @"digitalagenciesdev";

				public const string EndpointSuffix = @"core.windows.net";

				public static string ConnectionString = $"DefaultEndpointsProtocol=https;AccountName={AccountName};AccountKey={accountKey};EndpointSuffix=core.windows.net";

				public static string BaseUrl = $"https://{AccountName}.blob.core.windows.net";
			}
		}
	}
}

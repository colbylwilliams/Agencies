//#if __MOBILE__
//using Microsoft.Azure.Mobile;
//using Microsoft.Azure.Mobile.Crashes;
//using Microsoft.Azure.Mobile.Analytics;
//#endif

using System.Threading.Tasks;

using Plugin.VersionTracking;

using SettingsStudio;

using NomadCode.Azure;

//using Agencies.Domain;


namespace Agencies
{
	public static class Bootstrap
	{
		public static void Run ()
		{
			CrossVersionTracking.Current.Track ();

			Settings.RegisterDefaultSettings ();

			//#if __MOBILE__
			// Crashes.GetErrorAttachment = (report) => ErrorAttachment.AttachmentWithText (CrossVersionTracking.Current.ToString ());

			//if (!string.IsNullOrEmpty (Keys.MobileCenter.AppSecret))
			//{
			//	MobileCenter.Start (Keys.MobileCenter.AppSecret, typeof (Analytics), typeof (Crashes));

			//	Settings.UserReferenceKey = MobileCenter.InstallId?.ToString ("N") ?? "anonymous";
			//}

#if __ANDROID__

			Settings.VersionNumber = CrossVersionTracking.Current.CurrentVersion;

			Settings.BuildNumber = CrossVersionTracking.Current.CurrentBuild;

#endif
			//InitializeDataStore ();
			//#endif
		}

		public static async Task InitializeDataStoreAsync ()
		{
			//AzureClient.Shared.RegisterTable<AvContent> ();

			await AzureClient.Shared.InitializeAzync (Keys.Azure.PublicServiceUrl);
		}
	}
}

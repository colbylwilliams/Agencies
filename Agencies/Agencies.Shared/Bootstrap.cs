#if __MOBILE__
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile.Analytics;
#endif

using System.Threading.Tasks;

using Plugin.VersionTracking;

using SettingsStudio;

using NomadCode.Azure;


namespace Agencies
{
    public static class Bootstrap
    {
        public static void Run ()
        {
            CrossVersionTracking.Current.Track ();

            Settings.RegisterDefaultSettings ();

            // don't delete - this api should return soon
            // Crashes.GetErrorAttachment = (report) => ErrorAttachment.AttachmentWithText (CrossVersionTracking.Current.ToString ());

            if (!string.IsNullOrEmpty (Keys.MobileCenter.AppSecret))
            {
                MobileCenter.Start (Keys.MobileCenter.AppSecret, typeof (Analytics), typeof (Crashes));

                Settings.UserReferenceKey = MobileCenter.InstallId?.ToString ("N") ?? "anonymous";
            }

#if __ANDROID__

			Settings.VersionNumber = CrossVersionTracking.Current.CurrentVersion;

			Settings.BuildNumber = CrossVersionTracking.Current.CurrentBuild;
#endif

#if DEBUG
            NomadCode.BotFramework.BotClient.Shared.ResetConversation = Settings.ResetConversation;
#endif
        }

        public static async Task InitializeDataStoreAsync ()
        {
            //AzureClient.Shared.RegisterTable<AvContent> ();

            try
            {
                AzureClient.Shared.AuthProvider = Microsoft.WindowsAzure.MobileServices.MobileServiceAuthenticationProvider.Google;

                await AzureClient.Shared.InitializeAzync (Keys.Azure.ServiceUrl);
            }
            catch (System.Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }
    }
}

using System;

using Foundation;
using UIKit;
using UserNotifications;

using Microsoft.WindowsAzure.MobileServices;

using NomadCode.Azure;

using Google.SignIn;

using Facebook.CoreKit;

namespace Agencies.iOS
{
    [Register ("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate, IUNUserNotificationCenterDelegate
    {
        static bool processingNotification;


        public override UIWindow Window { get; set; }


        public AppDelegate ()
        {
            Bootstrap.Run ();
        }


        public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
        {
            // must assign delegate before app finishes launching.
            UNUserNotificationCenter.Current.Delegate = this;

            var googleServiceDictionary = NSDictionary.FromFile ("GoogleService-Info.plist");

            SignIn.SharedInstance.ClientID = googleServiceDictionary ["CLIENT_ID"].ToString ();
            SignIn.SharedInstance.ServerClientID = googleServiceDictionary ["SERVER_CLIENT_ID"].ToString ();

            ApplicationDelegate.SharedInstance.FinishedLaunching (application, launchOptions);

            return true;
        }


        public override bool OpenUrl (UIApplication app, NSUrl url, NSDictionary options)
        {
            var openUrlOptions = new UIApplicationOpenUrlOptions (options);

            var opened = SignIn.SharedInstance.HandleUrl (url, openUrlOptions.SourceApplication, openUrlOptions.Annotation);

            if (opened)
            {
                opened = ApplicationDelegate.SharedInstance.OpenUrl (app, url, openUrlOptions.SourceApplication, openUrlOptions.Annotation);
            }

            return opened;

            //return base.OpenUrl(app, url, options);
        }


        #region Push Notifications

        public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
        {
            Log.Debug ($"RegisteredForRemoteNotifications");

            var push = AzureClient.Shared.MobileServiceClient.GetPush ();

            push.RegisterAsync (deviceToken);
        }


        public override void FailedToRegisterForRemoteNotifications (UIApplication application, NSError error)
        {
            Log.Debug ($"FailedToRegisterForRemoteNotifications {error}");
        }


        // For a push notification to trigger a download operation, the notification’s payload must include
        //  the content-available key with its value set to 1. When that key is present, the system wakes
        //  the app in the background (or launches it into the background) and calls the app delegate’s 
        //  application:didReceiveRemoteNotification:fetchCompletionHandler: method. Your implementation 
        //  of that method should download the relevant content and integrate it into your app.
        public override void DidReceiveRemoteNotification (UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            Log.Debug ($"DidReceiveRemoteNotification:");

            for (int i = 0; i < userInfo.Keys.Length; i++)
            {
                Log.Debug ($"                             {userInfo.Keys [i]} : {userInfo.Values [i]}");
            }


            if (processingNotification)
            {
                Log.Debug ($"DidReceiveRemoteNotification: Already processing notificaiton. Returning");
                completionHandler (UIBackgroundFetchResult.NewData);
                return;
            }


            processingNotification = true;

            Log.Debug ($"DidReceiveRemoteNotification: Get All AvContent Async...");

            try
            {
                //await ProducerClient.Shared.GetAllAvContentAsync (true, Settings.TestProducer ? UserRoles.Producer : UserRoles.General);

                Log.Debug ($"DidReceiveRemoteNotification: Finished Getting Data.");

                completionHandler (UIBackgroundFetchResult.NewData);
            }
            catch (Exception ex)
            {
                Log.Debug ($"DidReceiveRemoteNotification: ERROR: FAILED TO GET NEW DATA {ex.Message}");

                completionHandler (UIBackgroundFetchResult.Failed);
            }
            finally
            {
                processingNotification = false;
            }
        }

        #endregion


    }
}

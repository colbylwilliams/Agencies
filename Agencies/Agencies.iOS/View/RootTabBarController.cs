using System;
using System.Threading.Tasks;

using UIKit;

using Google.SignIn;

using NomadCode.Azure;
using NomadCode.BotFramework;
using NomadCode.UIExtensions;
using NomadCode.ClientAuth;

using SettingsStudio;

using Agencies.Shared;

namespace Agencies.iOS
{
    public partial class RootTabBarController : UITabBarController
    {
        bool initialLoginAttempt = true;


        public RootTabBarController (IntPtr handle) : base (handle) { }


        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            AzureClient.Shared.AthorizationChanged += handleAzureAuthChanged;
            ClientAuthManager.Shared.AthorizationChanged += handleClientAuthChanged;

            SelectedIndex = Settings.SelectedTabIndex;

            ViewControllerSelected += (sender, e) =>
            {
                if (sender is UITabBarController tabController)
                {
                    Settings.SelectedTabIndex = (int)tabController.SelectedIndex;
                }
            };
        }


        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);

            Task.Run (async () => await loginAsync ());
        }


        void handleAzureAuthChanged (object s, bool e)
        {
            Log.Debug ($"Authenticated: {e}");
        }


        void handleClientAuthChanged (object s, ClientAuthDetails e)
        {
            Log.Debug ($"Authenticated: {e}");
        }


        async Task loginAsync ()
        {
            try
            {
                if (!AzureClient.Shared.Initialized) await Bootstrap.InitializeDataStoreAsync ();

                // try authenticating with an existing token
                if (!AzureClient.Shared.Authenticated)
                {
                    if (initialLoginAttempt)
                    {
                        initialLoginAttempt = false;

                        await AzureClient.Shared.AuthenticateAsync ();
                    }
                    else // see if we have what we need in the ClientAuthManager
                    {
                        var details = ClientAuthManager.Shared.ClientAuthDetails;

                        var auth = await AzureClient.Shared.AuthenticateAsync (details?.Token, details?.AuthCode);

                        if (auth.Authenticated)
                        {
                            BotClient.Shared.CurrentUserId = auth.Sid;

                            if (details != null)
                            {
                                BotClient.Shared.CurrentUserName = details.Username;
                                BotClient.Shared.CurrentUserEmail = details.Email;
                                BotClient.Shared.SetAvatarUrl (auth.Sid, details.AvatarUrl);
                            }
                        }
                    }
                }

                // if that worked, initialize the bot
                if (AzureClient.Shared.Authenticated)
                {
                    if (!BotClient.Shared.Initialized)
                    {
                        await BotClient.Shared.ConnectSocketAsync (conversationId => AgenciesClient.Shared.GetConversation (conversationId));
                    }

                    var faceApiKey = await AgenciesClient.Shared.GetFaceApiSubscription ();

                    Log.Debug ($"Face API Key: {faceApiKey}");
                }
                else // otherwise prompt the user to login
                {
                    BeginInvokeOnMainThread (() =>
                    {
                        var authViewController = new AuthViewController ();

                        if (authViewController != null)
                        {
                            var authNavController = new UINavigationController (authViewController);

                            if (authNavController != null)
                            {
                                PresentViewController (authNavController, true, null);
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }


        async Task logoutAsync ()
        {
            try
            {
                SignIn.SharedInstance.SignOutUser ();

                await AzureClient.Shared.LogoutAsync ();

                BotClient.Shared.Reset ();

                await loginAsync ();
            }
            catch (Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }
    }
}

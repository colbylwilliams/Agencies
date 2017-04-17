using System;
using System.Threading.Tasks;

using UIKit;

using Google.SignIn;

using NomadCode.Azure;
using NomadCode.BotFramework;
using NomadCode.UIExtensions;

using SettingsStudio;

using Agencies.Shared;

namespace Agencies.iOS
{
    public partial class RootTabBarController : UITabBarController
    {
        public RootTabBarController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            SelectedIndex = Settings.SelectedTabIndex;

            ViewControllerSelected += (sender, e) =>
            {
                if (sender is UITabBarController tabController)
                {
                    Settings.SelectedTabIndex = (int)tabController.SelectedIndex;
                }
            };

            AzureClient.Shared.AthorizationChanged += handleAthorizationChanged;
        }


        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);

            Task.Run (async () => await loginAsync ());
        }



        async Task loginAsync ()
        {
            try
            {
                if (!AzureClient.Shared.Initialized) await Bootstrap.InitializeDataStoreAsync ();

                // try authenticating with an existing token
                if (!AzureClient.Shared.Authenticated) await AzureClient.Shared.AuthenticateAsync ();

                // if that worked, initialize the bot
                if (AzureClient.Shared.Authenticated && !BotClient.Shared.Initialized)
                {
                    await BotClient.Shared.ConnectSocketAsync (conversationId => AgenciesClient.Shared.GetConversation (conversationId));
                }
                else // otherwise prompt the user to login
                {
                    BeginInvokeOnMainThread (() =>
                    {
                        var loginNavController = Storyboard.Instantiate<LoginNavigationController> ();

                        if (loginNavController != null)
                        {
                            PresentViewController (loginNavController, true, null);
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


        void handleAthorizationChanged (object s, bool e)
        {
            Log.Debug ($"Authenticated: {e}");
        }
    }
}

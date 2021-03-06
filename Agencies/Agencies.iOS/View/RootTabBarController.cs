﻿using System;
using System.Threading.Tasks;

using UIKit;

using Google.SignIn;

using NomadCode.BotFramework;
using NomadCode.ClientAuth;

using SettingsStudio;

using Agencies.Shared;
using NomadCode.Auth;

namespace Agencies.iOS
{

	public partial class RootTabBarController : UITabBarController
	{
		bool initialLoginAttempt = true;


		public RootTabBarController (IntPtr handle) : base (handle) { }


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ClientAuthManager.Shared.AthorizationChanged += handleClientAuthChanged;

			SelectedIndex = Settings.SelectedTabIndex;

			ViewControllerSelected += (sender, e) =>
			{
#if CSHARP_6
                var tabController = sender as UITabBarController;

                if (sender != null)
#else
				if (sender is UITabBarController tabController)
#endif
				{
					Settings.SelectedTabIndex = (int) tabController.SelectedIndex;
				}
			};
		}


		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			authenticate ();
		}


		void handleAzureAuthChanged (object s, bool e)
		{
			Log.Debug ($"Authenticated: {e}");
		}


		void handleClientAuthChanged (object s, ClientAuthDetails e)
		{
			Log.Debug ($"Authenticated: {e}");
		}


		void authenticate ()
		{
			Task.Run (async () =>
			{
				//BotClient.Shared.ResetCurrentUser ();
				//ClientAuthManager.Shared.LogoutAuthProviders ();
				//throw new Exception ("stop and re-comment out lines");

				try
				{
					var details = ClientAuthManager.Shared.ClientAuthDetails;

					// try authenticating with an existing token
					if (AgenciesClient.Shared.AuthUser == null && details != null)
					{
						var user = await AgenciesClient.Shared.GetAuthUserConfigAsync () ?? await AgenciesClient.Shared.GetAuthUserConfigAsync (details?.Token, details?.AuthCode);

						if (user != null)
						{
							BotClient.Shared.CurrentUserId = user.Id;

							BotClient.Shared.CurrentUserName = details.Username;
							BotClient.Shared.CurrentUserEmail = details.Email;
							BotClient.Shared.SetAvatarUrl (user.Id, details.AvatarUrl);

							await BotClient.Shared.ConnectSocketAsync (conversationId => AgenciesClient.Shared.GetConversationAsync (conversationId));

							FaceClient.Shared.SubscriptionKey = await AgenciesClient.Shared.GetFaceApiTokenAsync ();
						}
						else
						{
							logout ();
						}
					}
					else // otherwise prompt the user to login
					{
						if (AgenciesClient.Shared.AuthUser == null)
						{
							BeginInvokeOnMainThread (() => presentAuthController ());
						}
					}
				}
				catch (Exception ex)
				{
					Log.Error (ex.Message);
					throw;
				}
			});
		}


		void presentAuthController ()
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
		}


		void logout ()
		{
			try
			{
				SignIn.SharedInstance.SignOutUser ();
				BotClient.Shared.Reset ();

				authenticate ();
			}
			catch (Exception ex)
			{
				Log.Error (ex.Message);
				throw;
			}
		}
	}
}

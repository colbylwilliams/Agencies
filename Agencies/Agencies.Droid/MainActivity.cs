using System;
using System.Threading.Tasks;

using Android.App;

using Android.OS;

using NomadCode.ClientAuth;

using Agencies.Shared;

using BotClient = NomadCode.BotFramework.BotClient;

namespace Agencies.Droid
{
	[Activity (Label = "Agencies", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{

		bool initialLoginAttempt = true;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			//IFaceServiceClient client = new FaceServiceRestClient (Keys.CognitiveServices.FaceApi.SubscriptionKey);
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			Task.Run (() => loginAsync ());
		}


		async Task loginAsync ()
		{
			try
			{
				//BotClient.Shared.ResetCurrentUser();
				//ClientAuthManager.Shared.LogoutAuthProviders();

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
						//logoutAsync();
						BotClient.Shared.ResetCurrentUser ();
						ClientAuthManager.Shared.LogoutAuthProviders ();
					}
				}
				else // otherwise prompt the user to login
				{
					RunOnUiThread (() =>
					 {
						 ClientAuthManager.Shared.AuthActivityLayoutResId = Resource.Layout.LoginActivityLayout;

						 ClientAuthManager.Shared.GoogleWebClientResId = Resource.String.default_web_client_id;
						 ClientAuthManager.Shared.GoogleButtonResId = Resource.Id.sign_in_button;

						 StartActivity (typeof (AuthActivity));
					 });
				}
			}
			catch (Exception ex)
			{
				Log.Error (ex.Message);
				throw;
			}
		}
	}
}
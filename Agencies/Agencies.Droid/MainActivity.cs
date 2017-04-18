using System;
using System.Threading.Tasks;

using Android.App;

using Android.OS;

using NomadCode.Azure;
using NomadCode.BotFramework;
using NomadCode.ClientAuth;

using Agencies.Shared;

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

            authenticate ();
        }


        void authenticate ()
        {
            Task.Run (async () =>
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
                        // otherwise prompt the user to login
                        RunOnUiThread (() =>
                        {
                            ClientAuthManager.Shared.LoginResources = (Resource.String.default_web_client_id, Resource.Layout.LoginActivityLayout, Resource.Id.sign_in_button);

                            StartActivity (typeof (AuthActivity));
                        });
                    }
                }
                catch (Exception ex)
                {
                    Log.Error (ex.Message);
                    throw;
                }
            });
        }
    }
}
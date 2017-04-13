using Android.App;
using Android.Widget;
using Android.OS;
using Com.Microsoft.Projectoxford.Face;

using System.Threading.Tasks;
using NomadCode.Azure;
using NomadCode.BotFramework;
using System;

namespace Agencies.Droid
{
    [Activity (Label = "Agencies", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
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
                    if (!AzureClient.Shared.Initialized)
                    {
                        await Bootstrap.InitializeDataStoreAsync ();
                    }

                    if (!AzureClient.Shared.Authenticated)
                    {
                        // try authenticating with an existing token
                        await AzureClient.Shared.AuthenticateAsync ();
                    }

                    // if that worked, initialize the bot
                    if (AzureClient.Shared.Authenticated)
                    {
                        if (!BotClient.Shared.Initialized)
                        {
                            await BotClient.Shared.ConnectSocketAsync ();
                        }
                    }
                    else
                    {
                        // otherwise prompt the user to login
                        RunOnUiThread (() =>
                        {
                            StartActivity (typeof (LoginActivity));
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
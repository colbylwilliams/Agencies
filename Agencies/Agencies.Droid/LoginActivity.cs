
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Android.Gms.Auth.Api;
using Android.Support.V4.App;
using Android.Gms.Common;

using NomadCode.BotFramework;
using System.Threading.Tasks;
using NomadCode.Azure;

namespace Agencies.Droid
{
    [Activity (Label = "LoginActivity")]
    public class LoginActivity : FragmentActivity, View.IOnClickListener, GoogleApiClient.IOnConnectionFailedListener//AppCompatActivity
    {
        const int RC_SIGN_IN = 9001;

        GoogleApiClient googleApiClient;

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            // Create your application here

            SetContentView (Resource.Layout.LoginActivityLayout);

            // use this format to help users set up the app
            //if (GetString (Resource.String.google_app_id) == "YOUR-APP-ID")
            //throw new System.Exception ("Invalid google-services.json file.  Make sure you've downloaded your own config file and added it to your app project with the 'GoogleServicesJson' build action.");

            var webClientId = GetString (Resource.String.default_web_client_id);

            GoogleSignInOptions gso = new GoogleSignInOptions.Builder (GoogleSignInOptions.DefaultSignIn)
                                                             .RequestEmail ()
                                                             .RequestIdToken (webClientId)
                                                             .RequestServerAuthCode (webClientId)
                                                             .Build ();

            googleApiClient = new GoogleApiClient.Builder (this)
                                                 .EnableAutoManage (this, this)
                                                 .AddApi (Auth.GOOGLE_SIGN_IN_API, gso)
                                                 .Build ();

            FindViewById<SignInButton> (Resource.Id.sign_in_button).SetOnClickListener (this);
        }



        public void OnClick (View v)
        {
            switch (v.Id)
            {
                case Resource.Id.sign_in_button:
                    signIn ();
                    break;
                default:
                    break;
            }
        }


        void signIn ()
        {
            var signInIntent = Auth.GoogleSignInApi.GetSignInIntent (googleApiClient);
            StartActivityForResult (signInIntent, RC_SIGN_IN);
        }


        protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult (requestCode, resultCode, data);

            if (requestCode == RC_SIGN_IN)
            {
                var result = Auth.GoogleSignInApi.GetSignInResultFromIntent (data);
                handleSignInResult (result);
            }
        }


        void handleSignInResult (GoogleSignInResult result)
        {
            //Log.d (TAG, "handleSignInResult:" + result.isSuccess ());
            if (result.IsSuccess)
            {
                // Signed in successfully, show authenticated UI.
                GoogleSignInAccount user = result.SignInAccount;

                if (user != null)
                {
                    Log.Debug ($"user.Account.Name: {user.Account.Name}");
                    Log.Debug ($"acct.DisplayName: {user.DisplayName}");
                    Log.Debug ($"acct.Email: {user.Email}");
                    Log.Debug ($"acct.FamilyName: {user.FamilyName}");
                    Log.Debug ($"acct.GivenName: {user.GivenName}");
                    Log.Debug ($"acct.GrantedScopes: {string.Join (",", user.GrantedScopes)}");
                    Log.Debug ($"acct.Id: {user.Id}");
                    Log.Debug ($"acct.IdToken: {user.IdToken}");
                    Log.Debug ($"acct.PhotoUrl: {user.PhotoUrl}");
                    Log.Debug ($"acct.ServerAuthCode: {user.ServerAuthCode}");

                    BotClient.Shared.CurrentUserName = user.DisplayName;
                    BotClient.Shared.CurrentUserEmail = user.Email;

                    Task.Run (async () =>
                    {
                        var auth = await AzureClient.Shared.AuthenticateAsync (user.IdToken, user.ServerAuthCode);

                        BotClient.Shared.CurrentUserId = auth.Sid;

                        RunOnUiThread (() => Finish ());
                        //BeginInvokeOnMainThread (() => DismissViewController (true, null));
                    });
                }
            }
            else
            {
                // Signed out, show unauthenticated UI.
                Log.Error ($"Google SingIn failed with code:{result.Status}");
            }
        }


        public void OnConnectionFailed (ConnectionResult result)
        {
            Log.Error ($"{result.ErrorMessage} code: {result.ErrorCode}");
        }
    }
}

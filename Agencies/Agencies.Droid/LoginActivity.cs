
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

            GoogleSignInOptions gso = new GoogleSignInOptions.Builder (GoogleSignInOptions.DefaultSignIn)
                                                             .RequestEmail ()
                                                             .RequestIdToken (Keys.Google.ServerClientId)
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
                GoogleSignInAccount acct = result.SignInAccount;
                //mStatusTextView.setText (getString (R.string.signed_in_fmt, acct.getDisplayName ()));
            }
            else
            {
                // Signed out, show unauthenticated UI.
            }
        }

        public void OnConnectionFailed (ConnectionResult result)
        {
            Log.Error ($"{result.ErrorMessage} code: {result.ErrorCode}");
        }
    }
}

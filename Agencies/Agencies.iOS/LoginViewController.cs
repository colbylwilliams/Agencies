using System;

using Foundation;
using UIKit;
using Google.SignIn;
using System.Threading.Tasks;
using NomadCode.Azure;
using NomadCode.BotFramework;
using SettingsStudio;

namespace Agencies.iOS
{
    public partial class LoginViewController : UIViewController, ISignInDelegate, ISignInUIDelegate
    {
        public LoginViewController (IntPtr handle) : base (handle) { }

        //SignInButton _googleSignInButton;
        //SignInButton googleSignInButton => _googleSignInButton ?? (_googleSignInButton = new SignInButton ());

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            SignIn.SharedInstance.UIDelegate = this;
            SignIn.SharedInstance.Delegate = this;

            googleSignInButton.Style = ButtonStyle.Wide;
            googleSignInButton.ColorScheme = ButtonColorScheme.Dark;

            // Uncomment to automatically sign in the user.
            //SignIn.SharedInstance.SignInUserSilently ();

            // Uncomment to automatically sign out the user.
            //SignIn.SharedInstance.SignOutUser ();
        }


        #region ISignInDelegate

        public void DidSignIn (SignIn signIn, GoogleUser user, NSError error)
        {
            if (error == null)
            {
                // Perform any operations on signed in user here.
                var userId = user.UserID;                  // For client-side use only!
                var idToken = user.Authentication.IdToken; // Safe to send to the server
                var accessToken = user.Authentication.AccessToken;
                var serverAuth = user.ServerAuthCode;
                var fullName = user.Profile.Name;
                var givenName = user.Profile.GivenName;
                var familyName = user.Profile.FamilyName;
                var email = user.Profile.Email;
                // ...;
                Log.Debug ($"\n\tuserId: {userId},\n\tidToken: {idToken},\n\taccessToken: {accessToken},\n\tserverAuth: {serverAuth},\n\tfullName: {fullName},\n\tgivenName: {givenName},\n\tfamilyName: {familyName},\n\temail: {email},\n\t");

                BotClient.CurrentUserName = user?.Profile?.Name;
                BotClient.CurrentUserEmail = user?.Profile?.Email;

                Task.Run (async () =>
                {
                    var auth = await AzureClient.Shared.AuthenticateAsync (user.Authentication.IdToken, user.ServerAuthCode);

                    BotClient.CurrentUserId = auth.Sid;

                    BeginInvokeOnMainThread (() => DismissViewController (true, null));
                });
            }
            else
            {
                Log.Error (error.LocalizedDescription);
            }
        }


        [Export ("signIn:didDisconnectWithUser:withError:")]
        public void DidDisconnect (SignIn signIn, GoogleUser user, NSError error)
        {
            Log.Debug ("Google User DidDisconnect");

            // Perform any operations when the user disconnects from app here.
        }

        #endregion
    }
}
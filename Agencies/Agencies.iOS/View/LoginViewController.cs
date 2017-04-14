using System;

using Foundation;
using UIKit;
using Google.SignIn;
using System.Threading.Tasks;
using NomadCode.Azure;
using NomadCode.BotFramework;
using NomadCode.BotFramework.iOS;

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


            facebookSignInButton.ReadPermissions = new string [] { @"public_profile", @"email"/*, @"user_friends"*/};


            // Uncomment to automatically sign in the user.
            SignIn.SharedInstance.SignInUserSilently ();

            // Uncomment to automatically sign out the user.
            //SignIn.SharedInstance.SignOutUser ();

        }


        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);

            if (Facebook.CoreKit.AccessToken.CurrentAccessToken != null)
            {
                // User is logged in, do work such as go to next view controller. 
                Log.Debug ($"Facebook Current Access Token: {Facebook.CoreKit.AccessToken.CurrentAccessToken}");
            }
            else
            {
                Log.Debug ($"Facebook Current Access Token: null");
            }
        }


        #region ISignInDelegate

        public void DidSignIn (SignIn signIn, GoogleUser user, NSError error)
        {
            if (error == null && user != null)
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
                var imageUrl = user.Profile.GetImageUrl (64);
                // ...;
                Log.Debug ($"\n\tuserId: {userId},\n\tidToken: {idToken},\n\taccessToken: {accessToken},\n\tserverAuth: {serverAuth},\n\tfullName: {fullName},\n\tgivenName: {givenName},\n\tfamilyName: {familyName},\n\temail: {email},\n\timageUrl: {imageUrl},\n\t");

                BotClient.Shared.CurrentUserName = user?.Profile?.Name;
                BotClient.Shared.CurrentUserEmail = user?.Profile?.Email;

                Task.Run (async () =>
                {
                    var auth = await AzureClient.Shared.AuthenticateAsync (user.Authentication.IdToken, user.ServerAuthCode);

                    BeginInvokeOnMainThread (() =>
                    {
                        BotClient.Shared.CurrentUserId = auth.Sid;

                        BotClient.Shared.SetAvatarUrl (auth.Sid, user.Profile.GetImageUrl ((nuint)MessageCell.AvatarImageHeight)?.ToString ());

                        DismissViewController (true, null);
                    });
                });
            }
            else
            {
                Log.Error (error?.LocalizedDescription);
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
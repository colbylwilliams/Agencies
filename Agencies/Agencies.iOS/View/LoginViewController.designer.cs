// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Agencies.iOS
{
	[Register ("LoginViewController")]
	partial class LoginViewController
	{
		[Outlet]
		Facebook.LoginKit.LoginButton facebookSignInButton { get; set; }

		[Outlet]
		Google.SignIn.SignInButton googleSignInButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (googleSignInButton != null) {
				googleSignInButton.Dispose ();
				googleSignInButton = null;
			}

			if (facebookSignInButton != null) {
				facebookSignInButton.Dispose ();
				facebookSignInButton = null;
			}
		}
	}
}

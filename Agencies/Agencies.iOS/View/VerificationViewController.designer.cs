// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Agencies.iOS
{
	[Register ("VerificationViewController")]
	partial class VerificationViewController
	{
		[Outlet]
		UIKit.UIButton VerifyButton { get; set; }

		[Action ("ChooseImage1Action:")]
		partial void ChooseImage1Action (Foundation.NSObject sender);

		[Action ("ChooseImage2Action:")]
		partial void ChooseImage2Action (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (VerifyButton != null) {
				VerifyButton.Dispose ();
				VerifyButton = null;
			}
		}
	}
}

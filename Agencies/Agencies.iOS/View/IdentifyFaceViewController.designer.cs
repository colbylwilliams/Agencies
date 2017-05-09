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
	[Register ("IdentifyFaceViewController")]
	partial class IdentifyFaceViewController
	{
		[Outlet]
		UIKit.UIButton GoButton { get; set; }

		[Outlet]
		UIKit.UIImageView SelectedFaceImageView { get; set; }

		[Action ("ImageTapped:")]
		partial void ImageTapped (UIKit.UITapGestureRecognizer sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (SelectedFaceImageView != null) {
				SelectedFaceImageView.Dispose ();
				SelectedFaceImageView = null;
			}

			if (GoButton != null) {
				GoButton.Dispose ();
				GoButton = null;
			}
		}
	}
}

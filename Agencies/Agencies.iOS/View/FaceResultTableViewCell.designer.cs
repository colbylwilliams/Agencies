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
	[Register ("FaceResultTableViewCell")]
	partial class FaceResultTableViewCell
	{
		[Outlet]
		UIKit.UIImageView FaceImageView { get; set; }

		[Outlet]
		UIKit.UILabel FaceResultLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (FaceImageView != null) {
				FaceImageView.Dispose ();
				FaceImageView = null;
			}

			if (FaceResultLabel != null) {
				FaceResultLabel.Dispose ();
				FaceResultLabel = null;
			}
		}
	}
}

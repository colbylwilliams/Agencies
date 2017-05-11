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
	[Register ("DetectionResultsTableViewCell")]
	partial class DetectionResultsTableViewCell
	{
		[Outlet]
		UIKit.UILabel AgeLabel { get; set; }

		[Outlet]
		UIKit.UIImageView FaceImageView { get; set; }

		[Outlet]
		UIKit.UILabel FacialHairLabel { get; set; }

		[Outlet]
		UIKit.UILabel GenderLabel { get; set; }

		[Outlet]
		UIKit.UILabel HeadPoseLabel { get; set; }

		[Outlet]
		UIKit.UILabel SmileLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (FaceImageView != null) {
				FaceImageView.Dispose ();
				FaceImageView = null;
			}

			if (AgeLabel != null) {
				AgeLabel.Dispose ();
				AgeLabel = null;
			}

			if (GenderLabel != null) {
				GenderLabel.Dispose ();
				GenderLabel = null;
			}

			if (HeadPoseLabel != null) {
				HeadPoseLabel.Dispose ();
				HeadPoseLabel = null;
			}

			if (FacialHairLabel != null) {
				FacialHairLabel.Dispose ();
				FacialHairLabel = null;
			}

			if (SmileLabel != null) {
				SmileLabel.Dispose ();
				SmileLabel = null;
			}
		}
	}
}

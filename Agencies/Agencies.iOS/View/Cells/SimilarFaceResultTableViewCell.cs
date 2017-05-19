using System;
using Agencies.iOS.Extensions;
using Agencies.Shared;
using UIKit;

namespace Agencies.iOS
{
	public partial class SimilarFaceResultTableViewCell : UITableViewCell, IHandleResults<SimilarFaceResult>
	{
		public SimilarFaceResultTableViewCell (IntPtr handle) : base (handle)
		{
		}


		public void SetResult (SimilarFaceResult result)
		{
			FaceImageView.Image = result.Face?.GetImage ();
			ConfidenceLabel.Text = $"Confidence: {result.Confidence.ToString ()}";
		}
	}
}

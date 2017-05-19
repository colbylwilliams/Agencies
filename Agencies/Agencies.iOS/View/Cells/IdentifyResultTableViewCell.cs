using System;
using Agencies.iOS.Extensions;
using Agencies.Shared;
using UIKit;

namespace Agencies.iOS
{
	public partial class IdentifyResultTableViewCell : UITableViewCell, IHandleResults<IdentificationResult>
	{
		public IdentifyResultTableViewCell (IntPtr handle) : base (handle)
		{
		}


		public void SetResult (IdentificationResult result)
		{
			FaceImageView.Image = result.Face?.GetImage ();
			PersonNameLabel.Text = result.Person?.Name;
			ConfidenceLabel.Text = $"Confidence: {result.Confidence.ToString ()}";
		}
	}
}
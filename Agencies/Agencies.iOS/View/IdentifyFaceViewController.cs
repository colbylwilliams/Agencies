using System;

using Foundation;
using NomadCode.UIExtensions;
using UIKit;

namespace Agencies.iOS
{
	public partial class IdentifyFaceViewController : UIViewController
	{
        const string EmbedSegueId = "Embed";

		public IdentifyFaceViewController (IntPtr handle) : base (handle)
		{
		}


        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            SelectedFaceImageView.AddBorder (UIColor.Red, 2);
        }


        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue (segue, sender);

            if (segue.Identifier == EmbedSegueId && segue.DestinationViewController is GroupsTableViewController groupsTVC)
            {
                
            }
        }
	}
}
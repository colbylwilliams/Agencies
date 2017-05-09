using System;
using UIKit;

namespace Agencies.iOS
{
    public partial class FaceResultTableViewCell : UITableViewCell
    {
        public UIImageView ResultImage => FaceImageView;
        public UILabel ResultLabel => FaceResultLabel;

        public FaceResultTableViewCell (IntPtr handle) : base (handle)
        {
        }
    }
}
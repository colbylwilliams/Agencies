using System;
using UIKit;

namespace Agencies.iOS
{
    public partial class FaceResultTableViewCell : UITableViewCell
    {
        public UIImageView ResultImage => FaceImageView;
        public UILabel NameLabel => FaceResultLabel1;
        public UILabel ConfidenceLabel => FaceResultLabel2;

        public FaceResultTableViewCell (IntPtr handle) : base (handle)
        {
        }
    }
}
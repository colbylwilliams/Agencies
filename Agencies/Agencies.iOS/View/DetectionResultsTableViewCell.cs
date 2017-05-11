using System;
using UIKit;

namespace Agencies.iOS
{
    public partial class DetectionResultsTableViewCell : UITableViewCell
    {
        public UIImageView FaceImage => FaceImageView;
        public UILabel Age => AgeLabel;
        public UILabel Smile => SmileLabel;
        public UILabel Gender => GenderLabel;
        public UILabel HeadPose => HeadPoseLabel;
        public UILabel FacialHair => FacialHairLabel;

        public DetectionResultsTableViewCell (IntPtr handle) : base (handle)
        {
        }
    }
}
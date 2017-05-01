using System;
using UIKit;

namespace Agencies.iOS
{
    public partial class GroupPersonCVC : UICollectionViewCell
    {
        public UIImageView PersonImage => ImageView;

        public UILabel FaceIdLabel => TextView;

        public GroupPersonCVC (IntPtr handle) : base (handle)
        {
        }
    }
}
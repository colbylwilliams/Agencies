using System;
using UIKit;

namespace Agencies.iOS
{
    public partial class GroupPersonCVC : UICollectionViewCell
    {
        public UIImageView PersonImage => ImageView;

        public UILabel PersonName => TextView;

        public GroupPersonCVC (IntPtr handle) : base (handle)
        {
        }
    }
}
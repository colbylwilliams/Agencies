using System;
using UIKit;

namespace Agencies.iOS
{
    public partial class FaceCVC : UICollectionViewCell
    {
        public UIImageView FaceImage => FaceImageView;

        public FaceCVC (IntPtr handle) : base (handle)
        {
        }
    }
}
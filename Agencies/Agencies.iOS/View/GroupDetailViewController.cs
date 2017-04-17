using System;

using Foundation;
using UIKit;

namespace Agencies.iOS
{
    public partial class GroupDetailViewController : UIViewController, IUICollectionViewSource
    {
        public GroupDetailViewController (IntPtr handle) : base (handle)
        {
        }


        public UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
        {
            throw new NotImplementedException ();
        }


        public nint GetItemsCount (UICollectionView collectionView, nint section)
        {
            throw new NotImplementedException ();
        }


        partial void SaveAction (NSObject sender)
        {

        }
    }
}
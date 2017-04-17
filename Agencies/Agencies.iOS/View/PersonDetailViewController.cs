using System;

using Foundation;
using UIKit;

namespace Agencies.iOS
{
    public partial class PersonDetailViewController : UIViewController, IUICollectionViewSource
    {
        public PersonDetailViewController (IntPtr handle) : base (handle)
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


        partial void AddFaceAction (NSObject sender)
        {

        }


        partial void SaveAction (NSObject sender)
        {

        }
    }
}
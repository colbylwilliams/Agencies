using System;

using Foundation;
using UIKit;

namespace Agencies.iOS
{
    public partial class PersonDetailViewController : UICollectionViewController
    {
        public PersonDetailViewController (IntPtr handle) : base (handle)
        {
        }


        public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
        {
            throw new NotImplementedException ();
        }


        public override nint GetItemsCount (UICollectionView collectionView, nint section)
        {
            throw new NotImplementedException ();
        }


        partial void SaveAction (NSObject sender)
        {

        }


        partial void AddFaceAction (NSObject sender)
        {

        }
    }
}
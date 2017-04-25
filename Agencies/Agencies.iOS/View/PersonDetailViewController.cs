using System;
using Agencies.Shared;
using Foundation;
using UIKit;

namespace Agencies.iOS
{
    public partial class PersonDetailViewController : UICollectionViewController
    {
        public PersonGroup Group { get; set; }
        public bool NeedsTraining { get; set; }

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
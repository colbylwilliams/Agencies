using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Agencies.iOS
{
    public class ThreeItemRowCollectionViewController : BaseCollectionViewController, IUICollectionViewDelegateFlowLayout
    {
        public ThreeItemRowCollectionViewController (IntPtr handle) : base (handle)
        {
        }


        [Export ("collectionView:layout:sizeForItemAtIndexPath:")]
        public CGSize GetSizeForItem (UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            return new CGSize (CollectionView.Frame.Width / 3 - 10, (CollectionView.Frame.Width / 3 - 10));
        }


        [Export ("collectionView:layout:minimumLineSpacingForSectionAtIndex:")]
        public nfloat GetMinimumLineSpacingForSection (UICollectionView collectionView, UICollectionViewLayout layout, nint section)
        {
            return 10;
        }


        [Export ("collectionView:layout:minimumInteritemSpacingForSectionAtIndex:")]
        public nfloat GetMinimumInteritemSpacingForSection (UICollectionView collectionView, UICollectionViewLayout layout, nint section)
        {
            return 10;
        }
    }
}
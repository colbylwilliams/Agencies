using System;
using Agencies.Shared;
using Foundation;
using UIKit;

namespace Agencies.iOS
{
    public partial class GroupPersonCollectionViewController : UICollectionViewController
    {
        PersonGroup group;

        public GroupPersonCollectionViewController (IntPtr handle) : base (handle)
        {
        }


        public override nint NumberOfSections (UICollectionView collectionView) => group?.People?.Count ?? 0;


        public override nint GetItemsCount (UICollectionView collectionView, nint section) => group?.People? [(int)section]?.Faces?.Count ?? 0;


        public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
        {
            return base.GetCell (collectionView, indexPath);

            //      MPOPersonFaceCell cell = [collectionView dequeueReusableCellWithReuseIdentifier: @"faceCell" forIndexPath: indexPath];
            //      GroupPerson* person = self.group.people [indexPath.section];

            //[cell.faceImageView setImage: [(PersonFace *)person.faces[indexPath.row] image]];
            //cell.faceImageView.tag = indexPath.section;
            //[cell.personName setText:person.personName];
            //cell.faceImageView.userInteractionEnabled = YES;
            //if (cell.faceImageView.gestureRecognizers.count == 0) {
            //    [cell.faceImageView addGestureRecognizer:[[UILongPressGestureRecognizer alloc] initWithTarget:self action:@selector (longPressAction          //return cell;
        }
    }
}
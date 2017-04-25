using System;
using Agencies.Shared;
using Foundation;
using UIKit;

namespace Agencies.iOS
{
    public partial class PersonFaceCollectionViewController : UICollectionViewController
    {
        public Person Person { get; set; }

        public PersonFaceCollectionViewController (IntPtr handle) : base (handle)
        {
        }


        public override nint NumberOfSections (UICollectionView collectionView) => Person?.Faces?.Count ?? 0;


        public override nint GetItemsCount (UICollectionView collectionView, nint section) => Person?.Faces?.Count ?? 0;


        public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell ("Cell", indexPath) as GroupPersonCVC;

            var face = Person.Faces [indexPath.Row];

            cell.PersonImage.Image = UIImage.FromFile (face.PhotoPath);
            cell.PersonName.Text = Person.Name;

            //cell.faceImageView.tag = indexPath.section;
            //cell.faceImageView.userInteractionEnabled = YES;

            //if (cell.faceImageView.gestureRecognizers.count == 0)
            //{

            //[cell.faceImageView addGestureRecognizer:[[UILongPressGestureRecognizer alloc] initWithTarget:self action:@selector (longPressAction         

            return cell;
        }
    }
}
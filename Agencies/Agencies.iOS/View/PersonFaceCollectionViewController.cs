using System;
using Agencies.Shared;
using Foundation;
using UIKit;

namespace Agencies.iOS
{
    public partial class PersonFaceCollectionViewController : UICollectionViewController
    {
        public PersonGroup Group { get; set; }
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

            cell.PersonName.Text = Person.Name;
            cell.PersonImage.Image = UIImage.FromFile (face.PhotoPath);
            cell.PersonImage.UserInteractionEnabled = true;
            cell.PersonImage.Tag = indexPath.Section; //keep track of the person this imageview is for - used in longPressAction

            if (cell.PersonImage.GestureRecognizers == null || cell.PersonImage.GestureRecognizers?.Length == 0)
            {
                cell.PersonImage.AddGestureRecognizer (new UILongPressGestureRecognizer (longPressAction));
            }

            return cell;
        }


        async void longPressAction (UIGestureRecognizer gestureRecognizer)
        {
            if (gestureRecognizer.State == UIGestureRecognizerState.Began)
            {
                try
                {
                    var faceIndex = gestureRecognizer.View.Tag;

                    var result = await this.ShowActionSheet ("Do you want to remove this face?", string.Empty, "Yes");

                    if (result == "Yes")
                    {
                        var face = Person.Faces [(int)faceIndex];

                        this.ShowHUD ("Deleting this face");

                        await FaceClient.Shared.DeleteFace (Person, Group, face);

                        this.ShowSimpleHUD ("Face deleted");

                        CollectionView.ReloadData ();
                    }
                }
                catch (Exception)
                {
                    this.ShowSimpleAlert ("Failed to delete person.");
                }
            }
        }
    }
}
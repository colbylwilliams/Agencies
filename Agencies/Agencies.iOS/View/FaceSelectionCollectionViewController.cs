using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agencies.Shared;
using Foundation;
using NomadCode.UIExtensions;
using UIKit;

namespace Agencies.iOS
{
    public partial class FaceSelectionCollectionViewController : ThreeItemRowCollectionViewController
    {
        public PersonGroup Group { get; set; }
        public Person Person { get; set; }
        public List<Face> DetectedFaces { get; set; }
        public UIImage SourceImage { get; set; }

        List<UIImage> croppedImages;

        public FaceSelectionCollectionViewController (IntPtr handle) : base (handle)
        {
        }


        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);

            cropImages ();
        }


        public override void ViewWillDisappear (bool animated)
        {
            croppedImages.ForEach (i => i.Dispose ());
            croppedImages.Clear ();

            base.ViewWillDisappear (animated);
        }


        void cropImages ()
        {
            croppedImages = new List<UIImage> ();

            foreach (var face in DetectedFaces)
            {
                croppedImages.Add (SourceImage.Crop (face.FaceRectangle));
            }
        }


        public override nint NumberOfSections (UICollectionView collectionView) => 1;


        public override nint GetItemsCount (UICollectionView collectionView, nint section) => DetectedFaces.Count;


        public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell ("Cell", indexPath) as FaceCVC;

            var detectedFace = DetectedFaces [indexPath.Row];
            var image = croppedImages [indexPath.Row];

            cell.FaceImage.Image = image;

            return cell;
        }


        public async override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
        {
            var result = await this.ShowTwoOptionAlert ("Please choose", "Do you want to add this face?");

            if (result)
            {
                await addFace (DetectedFaces [indexPath.Row], SourceImage);
            }
        }


        async Task addFace (Face face, UIImage image)
        {
            try
            {
                this.ShowHUD ("Adding face");

                await FaceClient.Shared.AddFaceForPerson (Person, Group, face, image);

                var index = DetectedFaces.IndexOf (face);
                DetectedFaces.RemoveAt (index);
                croppedImages.RemoveAt (index);

                this.ShowSimpleHUD ("Face added for this person");

                CollectionView.ReloadData ();

                //NeedsTraining = true;
            }
            catch (Exception)
            {
                this.HideHUD ().ShowSimpleAlert ("Failed to add face.");
            }
        }
    }
}
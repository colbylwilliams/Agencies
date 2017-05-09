using System;
using System.Collections.Generic;
using Agencies.Shared;
using Foundation;
using NomadCode.UIExtensions;
using UIKit;

namespace Agencies.iOS
{
    public partial class FaceSelectionCollectionViewController : ThreeItemRowCollectionViewController
    {
        public List<Face> DetectedFaces { get; set; }
        public UIImage SourceImage { get; set; }

        List<UIImage> croppedImages;

        FaceSelectionViewController FaceSelectionController => ParentViewController as FaceSelectionViewController;

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

            SourceImage = null;

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
            var result = await this.ShowTwoOptionAlert ("Please choose", "Do you want to use this face?");

            if (result)
            {
                FaceSelectionController.SelectFace (DetectedFaces [indexPath.Row]);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Agencies.Shared;
using Foundation;
using UIKit;

namespace Agencies.iOS
{
    public partial class FaceSelectionViewController : UIViewController
    {
        class Segues
        {
            public const string Embed = "Embed";
        }

        public List<Face> DetectedFaces { get; set; }
        public UIImage SourceImage { get; set; }
        public string ReturnSegue { get; set; }
        public Face SelectedFace { get; private set; }

        FaceSelectionCollectionViewController FaceSelectionCVC => ChildViewControllers [0] as FaceSelectionCollectionViewController;

        public FaceSelectionViewController (IntPtr handle) : base (handle)
        {
        }


        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue (segue, sender);

            if (segue.Identifier == Segues.Embed && segue.DestinationViewController is FaceSelectionCollectionViewController faceSelectionCVC)
            {
                faceSelectionCVC.DetectedFaces = DetectedFaces;
                faceSelectionCVC.SourceImage = SourceImage;
            }
        }


        public void SelectFace (Face face)
        {
            SelectedFace = face;
            PerformSegue (ReturnSegue, this);
        }
    }
}
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

        public PersonGroup Group { get; set; }
        public Person Person { get; set; }
        public List<Face> DetectedFaces { get; set; }
        public UIImage SourceImage { get; set; }
        public bool NeedsTraining { get; set; }

        FaceSelectionCollectionViewController FaceSelectionCVC => ChildViewControllers [0] as FaceSelectionCollectionViewController;

        public FaceSelectionViewController (IntPtr handle) : base (handle)
        {
        }


        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue (segue, sender);

            if (segue.Identifier == Segues.Embed && segue.DestinationViewController is FaceSelectionCollectionViewController faceSelectionCVC)
            {
                faceSelectionCVC.Group = Group;
                faceSelectionCVC.Person = Person;
                faceSelectionCVC.DetectedFaces = DetectedFaces;
                faceSelectionCVC.SourceImage = SourceImage;
            }
        }
    }
}
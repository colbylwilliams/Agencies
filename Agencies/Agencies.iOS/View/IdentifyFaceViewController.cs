using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agencies.Shared;
using Foundation;
using NomadCode.UIExtensions;
using UIKit;

namespace Agencies.iOS
{
    public partial class IdentifyFaceViewController : UIViewController
    {
        class Segues
        {
            public const string Embed = "Embed";
            public const string SelectFaces = "SelectFaces";
            public const string FaceSelected = "IdentifyFaceSelected";
        }

        public List<Face> DetectedFaces { get; set; }
        public UIImage SourceImage { get; set; }

        GroupsTableViewController GroupsTableController => ChildViewControllers [0] as GroupsTableViewController;

        public IdentifyFaceViewController (IntPtr handle) : base (handle)
        {
        }


        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue (segue, sender);

            if (segue.Identifier == Segues.Embed && segue.DestinationViewController is GroupsTableViewController groupsTVC)
            {
                groupsTVC.AutoSelect = true;
            }
            else if (segue.Identifier == Segues.SelectFaces && segue.DestinationViewController is FaceSelectionViewController faceSelectionController)
            {
                faceSelectionController.ReturnSegue = Segues.FaceSelected;
                faceSelectionController.DetectedFaces = DetectedFaces;
                faceSelectionController.SourceImage = SourceImage;
            }
        }


        [Action ("UnwindToIdentify:")]
        public void UnwindToIdentify (UIStoryboardSegue segue)
        {
            var faceSelection = segue.SourceViewController as FaceSelectionViewController;

            if (faceSelection.SelectedFace != null)
            {
                UseFace (faceSelection.SelectedFace);
            }

            //await addFace (DetectedFaces [indexPath.Row], SourceImage);
        }


        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            SelectedFaceImageView.AddBorder (UIColor.Red, 2);
        }


        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);

            GroupsTableController.GroupSelectionChanged += OnGroupSelectionChanged;
            GoButton.TouchUpInside += IdentifyAction;
        }


        public override void ViewWillDisappear (bool animated)
        {
            GroupsTableController.GroupSelectionChanged -= OnGroupSelectionChanged;
            GoButton.TouchUpInside -= IdentifyAction;

            base.ViewWillDisappear (animated);
        }


        void OnGroupSelectionChanged (object sender, PersonGroup selection)
        {
            checkInputs ();
        }


        partial void ImageTapped (UITapGestureRecognizer sender)
        {
            ChooseImage ().Forget ();
        }


        async Task ChooseImage ()
        {
            try
            {
                SourceImage = await this.ShowImageSelectionDialog ();

                if (SourceImage != null)
                {
                    //make sure the image is in the .Up position
                    SourceImage.FixOrientation ();

                    this.ShowHUD ("Detecting faces");

                    DetectedFaces = await FaceClient.Shared.DetectFacesInPhoto (SourceImage);

                    if (DetectedFaces.Count == 0)
                    {
                        this.ShowSimpleHUD ("No faces detected");
                    }
                    else if (DetectedFaces.Count == 1)
                    {
                        UseFace (DetectedFaces [0]);
                    }
                    else // > 1 face
                    {
                        this.HideHUD ();

                        PerformSegue (Segues.SelectFaces, this);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error (ex);
                this.HideHUD ().ShowSimpleAlert ("Error detecting faces in the provided image");
            }
        }


        void UseFace (Face face)
        {
            var croppedFaceImg = SourceImage.Crop (face.FaceRectangle);
            SelectedFaceImageView.Image = croppedFaceImg;
            checkInputs ();
        }


        void checkInputs ()
        {
            if (SelectedFaceImageView.Image != null && GroupsTableController.SelectedPersonGroup != null)
            {
                GoButton.Enabled = true;
            }
            else
            {
                GoButton.Enabled = false;
            }
        }


        void IdentifyAction (object sender, EventArgs e)
        {

        }
    }
}
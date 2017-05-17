using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agencies.Shared;
using Foundation;
using NomadCode.UIExtensions;
using UIKit;

namespace Agencies.iOS
{
    public partial class VerificationViewController : UIViewController
    {
        class Segues
        {
            public const string Embed = "Embed";
        }

        public VerificationType VerificationType { get; set; }

        UIImage SourceImage1;
        UIImage SourceImage2;
        List<Face> verifyFaces;
        List<Face> targetFaces;

        FaceSelectionCollectionViewController Face1SelectionController => ChildViewControllers[0] as FaceSelectionCollectionViewController;
        FaceSelectionCollectionViewController Face2SelectionController => ChildViewControllers[1] as FaceSelectionCollectionViewController;

        public VerificationViewController(IntPtr handle) : base(handle)
        {
        }


        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            VerifyButton.TouchUpInside += Verify;
        }


        public override void ViewWillDisappear(bool animated)
        {
            VerifyButton.TouchUpInside -= Verify;

            base.ViewWillDisappear(animated);
        }


        partial void ChooseImage1Action(NSObject sender)
        {
            ChooseImage(Face1SelectionController).Forget();
        }


        partial void ChooseImage2Action(NSObject sender)
        {
            ChooseImage(Face2SelectionController).Forget();
        }


        void checkInputs()
        {
            VerifyButton.Enabled = SourceImage1 != null && SourceImage2 != null;
        }


        async Task ChooseImage(FaceSelectionCollectionViewController selectionController)
        {
            try
            {
                var image = await this.ShowImageSelectionDialog();

                if (image != null)
                {
                    //make sure the image is in the .Up position
                    image.FixOrientation();

                    this.ShowHUD("Detecting faces");

                    var detectedFaces = await FaceClient.Shared.DetectFacesInPhoto(image);

                    if (detectedFaces.Count == 0)
                    {
                        this.ShowSimpleHUD("No faces detected");
                    }
                    else
                    {
                        //selectionController.

                        this.HideHUD();
                    }

                    checkInputs();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                this.HideHUD().ShowSimpleAlert("Error using image selected");
            }
        }


        async void Verify(object sender, EventArgs e)
        {
            try
            {
                this.ShowHUD("Detecting faces");

                //DetectedFaces = await FaceClient.Shared.DetectFacesInPhoto(SourceImage

                //if (DetectedFaces.Count == 0)
                //{
                //    this.ShowSimpleHUD("No faces detected");
                //}
                //else // > 1 face
                //{
                //    this.HideHUD();

                //    DetectionResultsController.SetResults(DetectedFaces);
                //}
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                this.HideHUD().ShowSimpleAlert("Error detecting faces in the selected image");
            }
        }
    }
}
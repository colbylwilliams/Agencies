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

		FaceSelectionCollectionViewController Face1SelectionController => ChildViewControllers [0] as FaceSelectionCollectionViewController;
		FaceSelectionCollectionViewController Face2SelectionController => ChildViewControllers [1] as FaceSelectionCollectionViewController;

		public VerificationViewController (IntPtr handle) : base (handle)
		{
		}


		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			VerifyButton.TouchUpInside += Verify;
			Face1SelectionController.FaceSelectionChanged += FaceSelectionChanged;
			Face2SelectionController.FaceSelectionChanged += FaceSelectionChanged;
		}


		public override void ViewWillDisappear (bool animated)
		{
			VerifyButton.TouchUpInside -= Verify;
			Face1SelectionController.FaceSelectionChanged -= FaceSelectionChanged;
			Face2SelectionController.FaceSelectionChanged -= FaceSelectionChanged;

			base.ViewWillDisappear (animated);
		}


		partial void ChooseImage1Action (NSObject sender)
		{
			ChooseImage (Face1SelectionController).Forget ();
		}


		partial void ChooseImage2Action (NSObject sender)
		{
			ChooseImage (Face2SelectionController).Forget ();
		}


		void FaceSelectionChanged (object sender, EventArgs e)
		{
			checkInputs ();
		}


		void checkInputs ()
		{
			VerifyButton.Enabled = Face1SelectionController.HasSelection && Face2SelectionController.HasSelection;
		}


		async Task ChooseImage (FaceSelectionCollectionViewController selectionController)
		{
			try
			{
				var image = await this.ShowImageSelectionDialog ();

				if (image != null)
				{
					//make sure the image is in the .Up position
					image.FixOrientation ();

					this.ShowHUD ("Detecting faces");

					var detectedFaces = await FaceClient.Shared.DetectFacesInPhoto (image);

					if (detectedFaces.Count == 0)
					{
						this.ShowSimpleHUD ("No faces detected");
					}
					else
					{
						selectionController.SetDetectedFaces (image, detectedFaces);

						this.HideHUD ();
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error (ex);
				this.HideHUD ().ShowSimpleAlert ("Error using image selected");
			}
		}


		async void Verify (object sender, EventArgs e)
		{
			try
			{
				this.ShowHUD ("Verifying faces");

				VerifyResult result = null;
				string successMsg = "These two faces are from the same person.  The confidence is {0}";
				string failMsg = "These two faces are not from the same person.";

				switch (VerificationType)
				{
					case VerificationType.FaceAndFace:
						result = await FaceClient.Shared.Verify (Face1SelectionController.SelectedFace, Face2SelectionController.SelectedFace);
						break;
					case VerificationType.FaceAndPerson:
						result = await FaceClient.Shared.Verify (Face1SelectionController.SelectedFace, Face2SelectionController.SelectedFace);
						break;
				}

				this.HideHUD ();

				if (result.IsIdentical)
				{
					this.ShowSimpleAlert (successMsg.Fmt (result.Confidence));
				}
				else
				{
					this.ShowSimpleAlert (failMsg);
				}
			}
			catch (Exception ex)
			{
				Log.Error (ex);
				this.HideHUD ().ShowSimpleAlert ("Error verifying the selected faces");
			}
		}
	}
}
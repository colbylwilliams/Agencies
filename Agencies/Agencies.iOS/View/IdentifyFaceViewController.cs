using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agencies.iOS.Extensions;
using Agencies.Shared;
using Foundation;
using NomadCode.UIExtensions;
using UIKit;

namespace Agencies.iOS
{
	public partial class IdentifyFaceViewController : PopoverPresentationViewController
	{
		class Segues
		{
			public const string Embed = "Embed";
			public const string SelectFaces = "SelectFaces";
			public const string FaceSelected = "IdentifyFaceSelected";
			public const string ShowResults = "ShowResults";
		}

		public List<Face> DetectedFaces { get; set; }
		public UIImage SourceImage { get; set; }

		List<IdentificationResult> Results;
		Face chosenFace;

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
			else if (segue.Identifier == Segues.SelectFaces && segue.DestinationViewController is FaceSelectionCollectionViewController faceSelectionController)
			{
				faceSelectionController.PopoverPresentationController.Delegate = this;
				faceSelectionController.ReturnSegue = Segues.FaceSelected;
				faceSelectionController.DetectedFaces = DetectedFaces;
				faceSelectionController.SourceImage = SourceImage;
			}
			else if (segue.Identifier == Segues.ShowResults && segue.DestinationViewController is FaceResultsTableViewController resultsTVC)
			{
				resultsTVC.PopoverPresentationController.Delegate = this;
				resultsTVC.Results = Results;
			}
		}


		[Action ("UnwindToIdentify:")]
		public void UnwindToIdentify (UIStoryboardSegue segue)
		{
			var faceSelection = segue.SourceViewController as FaceSelectionCollectionViewController;

			if (faceSelection.SelectedFace != null)
			{
				UseFace (faceSelection.SelectedFace);
			}
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
			GoButton.TouchUpInside += Identify;
		}


		public override void ViewWillDisappear (bool animated)
		{
			GroupsTableController.GroupSelectionChanged -= OnGroupSelectionChanged;
			GoButton.TouchUpInside -= Identify;

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
						this.HideHUD ();
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
			chosenFace = face;
			var croppedFaceImg = SourceImage.Crop (face.FaceRectangle);
			face.SavePhotoFromCropped (croppedFaceImg);
			SelectedFaceImageView.Image = croppedFaceImg;
			checkInputs ();
		}


		void checkInputs ()
		{
			GoButton.Enabled = chosenFace != null &&
				SelectedFaceImageView.Image != null &&
				GroupsTableController.SelectedPersonGroup != null;
		}


		async void Identify (object sender, EventArgs e)
		{
			try
			{
				var group = GroupsTableController.SelectedPersonGroup;

				this.ShowHUD ("Identifying faces");

				Results = await FaceClient.Shared.Identify (group, chosenFace);

				if (Results.Count == 0)
				{
					this.ShowSimpleHUD ("Not able to identify this face against the selected group");
				}

				this.HideHUD ();

				PerformSegue (Segues.ShowResults, this);
			}
			catch (Exception ex)
			{
				Log.Error (ex);
				this.HideHUD ().ShowSimpleAlert ("Error identifying face");
			}
		}


		protected override string GetPopoverCloseText (UIViewController presentedViewController)
		{
			return presentedViewController is FaceResultsTableViewController ? "Done" : "Cancel";
		}
	}
}
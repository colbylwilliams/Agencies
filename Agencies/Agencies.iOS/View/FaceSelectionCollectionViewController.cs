using System;
using System.Collections.Generic;
using Agencies.Shared;
using Foundation;
using NomadCode.UIExtensions;
using UIKit;

namespace Agencies.iOS
{
	public partial class FaceSelectionCollectionViewController : ItemsPerRowCollectionViewController
	{
		public List<Face> DetectedFaces { get; set; }
		public UIImage SourceImage { get; set; }
		public string ReturnSegue { get; set; }
		public Face SelectedFace { get; private set; }

		public event EventHandler FaceSelectionChanged;

		List<UIImage> croppedImages;

		public bool HasSelection => SelectedFace != null;


		public FaceSelectionCollectionViewController (IntPtr handle) : base (handle)
		{
		}


		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (SourceImage != null)
			{
				cropImages ();
			}
		}


		public override void ViewWillDisappear (bool animated)
		{
			cleanup ();

			base.ViewWillDisappear (animated);
		}


		void cleanup (bool disposeCurrentImage = false)
		{
			if (croppedImages != null)
			{
				croppedImages.ForEach (i => i.Dispose ());
				croppedImages.Clear ();
			}

			if (SourceImage != null)
			{
				if (disposeCurrentImage)
				{
					SourceImage.Dispose ();
				}

				SourceImage = null;
			}
		}


		public void SetDetectedFaces (UIImage sourceImage, List<Face> detectedFaces)
		{
			cleanup (true);

			SourceImage = sourceImage;
			DetectedFaces = detectedFaces;
			SelectedFace = null;

			cropImages ();

			CollectionView.ReloadData ();
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


		public override nint GetItemsCount (UICollectionView collectionView, nint section) => DetectedFaces?.Count ?? 0;


		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.Dequeue<FaceCVC> (indexPath) as FaceCVC;

			var detectedFace = DetectedFaces [indexPath.Row];
			var image = croppedImages [indexPath.Row];

			cell.SetFaceImage (detectedFace, image);

			return cell;
		}


		public async override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			SelectedFace = DetectedFaces [indexPath.Row];

			var cell = collectionView.CellForItem (indexPath);
			cell.Highlighted = true;

			if (ReturnSegue != null)
			{
				var result = await this.ShowTwoOptionAlert ("Please choose", "Do you want to use this face?");

				if (result)
				{
					PerformSegue (ReturnSegue, this);
				}
			}
			else
			{
				FaceSelectionChanged?.Invoke (this, EventArgs.Empty);
			}
		}


		public override void ItemDeselected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			SelectedFace = null;

			var cell = collectionView.CellForItem (indexPath);
			cell.Highlighted = false;

			FaceSelectionChanged?.Invoke (this, EventArgs.Empty);
		}
	}
}
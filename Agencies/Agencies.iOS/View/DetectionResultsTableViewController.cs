using System;
using System.Collections.Generic;
using Agencies.iOS.Extensions;
using Agencies.Shared;
using Foundation;
using UIKit;

namespace Agencies.iOS
{
    public partial class DetectionResultsTableViewController : UITableViewController
    {
        public List<Face> DetectedFaces { get; set; }

        public DetectionResultsTableViewController (IntPtr handle) : base (handle)
        {
        }


        public void SetResults (List<Face> detectedFaces)
        {
            DetectedFaces = detectedFaces;
            TableView.ReloadData ();
        }


        public override nint NumberOfSections (UITableView tableView) => 1;


        public override nint RowsInSection (UITableView tableView, nint section) => DetectedFaces?.Count ?? 0;


        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell ("Cell", indexPath) as DetectionResultsTableViewCell;

            var face = DetectedFaces [indexPath.Row];
            var hasFacialHair = face.HasFacialHair ? "Yes" : "No";

            cell.FaceImage.Image = face.GetImage ();
            cell.Age.Text = $"Age: {face.Attributes.Age.ToString ()}";
            cell.Gender.Text = $"Gender: {face.Attributes.Gender}";
            cell.FacialHair.Text = $"Facial Hair: {hasFacialHair}";
            cell.HeadPose.Text = $"Head Pose: {face.Attributes?.HeadPose?.Roll}° roll, {face.Attributes?.HeadPose?.Yaw}° yaw, {face.Attributes?.HeadPose?.Pitch}° pitch";
            cell.Smile.Text = $"Smile: {face.Attributes?.Smile.ToString ()}";

            return cell;
        }
    }
}
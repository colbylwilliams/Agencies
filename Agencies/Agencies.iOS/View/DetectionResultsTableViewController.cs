using System;
using System.Collections.Generic;
using Agencies.iOS.Extensions;
using Agencies.Shared;
using Foundation;
using NomadCode.UIExtensions;
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
            var cell = tableView.Dequeue<DetectionResultsTableViewCell> (indexPath);

            var face = DetectedFaces [indexPath.Row];

            cell.ImageView.Image = face.GetImage ();
            cell.Title.Text = face.Id;
            cell.Size.Text = $"Position: {face.FaceRectangle.Left},{face.FaceRectangle.Top}; Size: {face.FaceRectangle.Width}x{face.FaceRectangle.Height}";

            var attrs = face.Attributes;

            if (attrs != null)
            {
                cell.Age.Text = $"Age: {attrs.Age}";
                cell.Gender.Text = $"Gender: {attrs.Gender}";
                cell.Hair.Text = attrs.Hair?.ToString ();
                cell.Smile.Text = $"Smile Intensity: {attrs.SmileIntensity}";
                cell.FacialHair.Text = attrs.FacialHair?.ToString ();
                cell.Glasses.Text = $"Glasses: {attrs.Glasses}";
                cell.Emotion.Text = attrs.Emotion?.ToString ();
                cell.Makeup.Text = attrs.Makeup?.ToString ();

                cell.HeadPose.Text = attrs.HeadPose?.ToString ();
                cell.Accessories.Text = attrs.Accessories?.ToString ();
                cell.Occlusion.Text = attrs.Occlusion?.ToString ();
                cell.Blur.Text = attrs.Blur?.ToString ();
                cell.Noise.Text = attrs.Noise?.ToString ();
                cell.Exposure.Text = attrs.Exposure?.ToString ();
            }

            return cell;
        }
    }
}
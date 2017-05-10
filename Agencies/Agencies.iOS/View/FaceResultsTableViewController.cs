using System;
using System.Collections.Generic;
using Agencies.iOS.Extensions;
using Agencies.Shared;
using Foundation;
using UIKit;

namespace Agencies.iOS
{
    public partial class FaceResultsTableViewController : BaseTableViewController
    {
        public List<IdentificationResult> Results { get; set; }

        public FaceResultsTableViewController (IntPtr handle) : base (handle)
        {
        }


        public override nint NumberOfSections (UITableView tableView) => 1;


        public override nint RowsInSection (UITableView tableView, nint section) => Results?.Count ?? 0;


        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell ("Cell", indexPath) as FaceResultTableViewCell;

            var result = Results [indexPath.Row];

            cell.ResultImage.Image = result.Face?.GetImage ();
            cell.NameLabel.Text = result.Person?.Name;
            cell.ConfidenceLabel.Text = $"Confidence: {result.Confidence.ToString ()}";

            return cell;
        }
    }
}
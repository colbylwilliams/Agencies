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
        List<Face> Faces { get; set; }

        public FaceResultsTableViewController (IntPtr handle) : base (handle)
        {
        }


        public override nint NumberOfSections (UITableView tableView) => 1;


        public override nint RowsInSection (UITableView tableView, nint section) => Faces?.Count ?? 0;


        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell ("Cell", indexPath) as FaceResultTableViewCell;

            var face = Faces [indexPath.Row];

            cell.ResultImage.Image = face.GetImage ();
            cell.ResultLabel.Text = "";

            return cell;
        }
    }
}
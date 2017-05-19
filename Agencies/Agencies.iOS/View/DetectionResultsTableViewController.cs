using System;
using System.Collections.Generic;
using Agencies.Shared;
using Foundation;
using NomadCode.UIExtensions;
using UIKit;

namespace Agencies.iOS
{
	public partial class DetectionResultsTableViewController : BaseTableViewController
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

			cell.SetFace (face);

			return cell;
		}
	}
}
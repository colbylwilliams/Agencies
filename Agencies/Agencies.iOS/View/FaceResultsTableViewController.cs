using System;
using System.Collections.Generic;
using Foundation;
using NomadCode.UIExtensions;
using UIKit;

namespace Agencies.iOS
{
	public partial class FaceResultsTableViewController<TCell, TResult> : BaseTableViewController
	where TCell : UITableViewCell, IHandleResults<TResult>
	{
		public List<TResult> Results { get; set; }

		public FaceResultsTableViewController (IntPtr handle) : base (handle)
		{
		}


		public override nint NumberOfSections (UITableView tableView) => 1;


		public override nint RowsInSection (UITableView tableView, nint section) => Results?.Count ?? 0;


		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.Dequeue<TCell> (indexPath);

			var result = Results [indexPath.Row];

			cell.SetResult (result);

			return cell;
		}
	}
}
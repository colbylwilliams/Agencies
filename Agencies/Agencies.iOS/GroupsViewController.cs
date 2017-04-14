using System;
using Agencies.Shared;
using Foundation;
using UIKit;

namespace Agencies.iOS
{
    public partial class GroupsViewController : UITableViewController
    {
        public GroupsViewController (IntPtr handle) : base (handle)
        {
        }


        public override nint NumberOfSections (UITableView tableView)
        {
            return 1;
        }


        public override nint RowsInSection (UITableView tableView, nint section)
        {
            return FaceClient.Current.Groups.Count;
        }
    }
}
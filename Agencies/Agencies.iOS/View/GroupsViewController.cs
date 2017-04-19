using System;

using Foundation;
using UIKit;
using Agencies.Shared;

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
            return FaceClient.Shared.Groups.Count;
        }


        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell ("Cell", indexPath);

            //cell = [[UITableViewCell alloc] initWithStyle: UITableViewCellStyleValue1 reuseIdentifier:showUserInfoCellIdentifier];

            cell.TextLabel.Text = ((PersonGroup*)GLOBAL.groups [indexPath.row]).groupName;
            cell.BackgroundColor = UIColor.Clear;
            return cell;

        }
    }
}
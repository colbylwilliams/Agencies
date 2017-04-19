using System;

using Foundation;
using UIKit;
using Agencies.Shared;
using System.Collections.Generic;

namespace Agencies.iOS
{
    public partial class GroupsViewController : UITableViewController
    {
        List<PersonGroup> Groups;

        public GroupsViewController (IntPtr handle) : base (handle)
        {
        }


        public async override void ViewDidLoad ()
        {
            Groups = await FaceClient.Shared.GetGroups ();

            base.ViewDidLoad ();
        }


        public override nint NumberOfSections (UITableView tableView)
        {
            return 1;
        }


        public override nint RowsInSection (UITableView tableView, nint section)
        {
            return Groups.Count;
        }


        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell ("Cell", indexPath);

            //cell = [[UITableViewCell alloc] initWithStyle: UITableViewCellStyleValue1 reuseIdentifier:showUserInfoCellIdentifier];

            cell.TextLabel.Text = Groups [indexPath.Row].Name;
            cell.BackgroundColor = UIColor.Clear;

            return cell;
        }
    }
}
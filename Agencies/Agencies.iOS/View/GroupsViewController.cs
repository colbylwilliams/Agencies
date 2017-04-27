using System;
using Foundation;
using UIKit;
using Agencies.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agencies.iOS
{
    public partial class GroupsViewController : UITableViewController
    {
        const string DetailSegueId = "GroupDetail";

        List<PersonGroup> Groups;

        public GroupsViewController (IntPtr handle) : base (handle)
        {
        }


        public override void ViewWillAppear (bool animated)
        {
            loadGroups ().Forget ();

            base.ViewWillAppear (animated);
        }


        async Task loadGroups ()
        {
            try
            {
                Groups = await FaceClient.Shared.GetGroups ();

                TableView.ReloadData ();
            }
            catch (Exception)
            {
                this.ShowSimpleAlert ("Error loading groups.");
            }
        }


        public override nint NumberOfSections (UITableView tableView) => 1;


        public override nint RowsInSection (UITableView tableView, nint section) => Groups?.Count ?? 0;


        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell ("Cell", indexPath);

            cell.TextLabel.Text = Groups [indexPath.Row].Name;
            cell.BackgroundColor = UIColor.Clear;

            return cell;
        }


        public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }


        public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            if (editingStyle == UITableViewCellEditingStyle.Delete)
            {
                tableView.BeginUpdates ();

                var group = Groups [indexPath.Row];
                Groups.Remove (group);
                deleteGroup (group).Forget ();

                tableView.DeleteRows (new NSIndexPath [] { indexPath }, UITableViewRowAnimation.Automatic);
                tableView.EndUpdates ();
            }
        }


        async Task deleteGroup (PersonGroup personGroup)
        {
            try
            {
                //no UI feedback here since this is done via swipe to delete

                //await this.ShowHUD ()

                await FaceClient.Shared.DeleteGroup (personGroup);

                //TableView.ReloadData ();
            }
            catch (Exception)
            {
                this.ShowSimpleAlert ("Error deleting group.");
            }
        }


        public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
        {
            PerformSegue (DetailSegueId, this);
        }


        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue (segue, sender);

            if (segue.Identifier == DetailSegueId && TableView.IndexPathForSelectedRow != null)
            {
                var group = Groups [TableView.IndexPathForSelectedRow.Row];
                (segue.DestinationViewController as GroupDetailViewController).Group = group;
            }
        }
    }
}
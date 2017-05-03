using System;
using Foundation;
using UIKit;
using Agencies.Shared;

namespace Agencies.iOS
{
    public partial class GroupsViewController : UIViewController, IHandleChildSelection<PersonGroup>
    {
        const string EmbedSegueId = "Embed";
        const string DetailSegueId = "GroupDetail";

        PersonGroup selectedGroup;

        public GroupsViewController (IntPtr handle) : base (handle)
        {
        }


        public void HandleChildSelection (PersonGroup selection)
        {
            selectedGroup = selection;
            PerformSegue (DetailSegueId, this);
        }


        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue (segue, sender);

            if (segue.Identifier == EmbedSegueId && segue.DestinationViewController is GroupsTableViewController groupsTVC)
            {
                groupsTVC.AllowDelete = true;
            }
            else if (segue.Identifier == DetailSegueId && selectedGroup != null && segue.DestinationViewController is GroupDetailViewController groupDetailVC)
            {
                groupDetailVC.Group = selectedGroup;
                selectedGroup = null;
            }
        }
    }
}
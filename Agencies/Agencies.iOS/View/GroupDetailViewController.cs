using System;
using Foundation;
using UIKit;
using Agencies.Shared;
using System.Threading.Tasks;

namespace Agencies.iOS
{
    public partial class GroupDetailViewController : UIViewController
    {
        const string EmbedSegueId = "Embed";
        const string AddPersonSegueId = "AddPerson";

        public PersonGroup Group { get; set; }
        public bool NeedsTraining { get; set; }

        GroupPersonCollectionViewController GroupPersonCVC => ChildViewControllers [0] as GroupPersonCollectionViewController;

        public GroupDetailViewController (IntPtr handle) : base (handle)
        {
        }


        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue (segue, sender);

            if (segue.Identifier == EmbedSegueId && Group != null)
            {
                var groupPeopleCVC = segue.DestinationViewController as GroupPersonCollectionViewController;

                groupPeopleCVC.Group = Group;
            }
            else if (segue.Identifier == AddPersonSegueId)
            {
                var groupPersonVC = segue.DestinationViewController as PersonDetailViewController;

                groupPersonVC.Group = Group;
                groupPersonVC.NeedsTraining = this.NeedsTraining;
            }
        }


        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);

            if (Group != null)
            {
                GroupName.Text = Group.Name;
            }
        }


        partial void SaveAction (NSObject sender)
        {
            if (GroupName.Text.Length == 0)
            {
                this.ShowSimpleAlert ("Please input the group name");
                return;
            }

            if (Group == null)
            {
                createNewGroup ().Forget ();
            }
            else
            {
                updateGroup ().Forget ();
            }
        }


        partial void AddAction (NSObject sender)
        {
            if (GroupName.Text.Length == 0)
            {
                this.ShowSimpleAlert ("Please input the group name");
                return;
            }

            AddPerson ().Forget ();
        }


        async Task AddPerson ()
        {
            if (Group == null)
            {
                var createGroup = await this.ShowTwoOptionAlert ("Create Group?", "Do you want to create this new group?");

                if (!createGroup)
                {
                    return;
                }

                await createNewGroup ();
            }

            if (Group != null) //just to make sure we succeeded in the case we created a new group above
            {
                PerformSegue (AddPersonSegueId, this);
            }
        }


        async Task updateGroup ()
        {
            try
            {
                this.ShowHUD ("Saving Group");

                await FaceClient.Shared.UpdatePersonGroup (Group, GroupName.Text);

                this.ShowSimpleHUD ("Group saved");

                //_shouldExit = NO;
                await trainGroup ();
            }
            catch (Exception)
            {
                this.ShowSimpleAlert ("Failed to update group.");
            }
        }


        async Task createNewGroup ()
        {
            try
            {
                this.ShowHUD ("Creating group");

                Group = await FaceClient.Shared.CreatePersonGroup (GroupName.Text);

                GroupPersonCVC.Group = Group;

                this.ShowSimpleHUD ("Group created");

                GroupPersonCVC.CollectionView.ReloadData ();
            }
            catch (Exception)
            {
                this.ShowSimpleAlert ("Failed to create group.");
            }
        }


        async Task trainGroup ()
        {
            try
            {
                this.ShowHUD ("Training group");

                await FaceClient.Shared.TrainGroup (Group);

                this.ShowSimpleHUD ("This group is trained.");

                //if (_shouldExit)
                //{
                //    this.NavigationController.PopViewController (true);
                //}
            }
            catch (Exception)
            {
                this.ShowSimpleHUD ("Failed in training group.");
            }
        }
    }
}
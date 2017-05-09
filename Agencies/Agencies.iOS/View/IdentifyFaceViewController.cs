using System;
using System.Threading.Tasks;
using Agencies.Shared;
using Foundation;
using NomadCode.UIExtensions;
using UIKit;

namespace Agencies.iOS
{
    public partial class IdentifyFaceViewController : UIViewController
    {
        const string EmbedSegueId = "Embed";

        GroupsTableViewController GroupsTableController => ChildViewControllers [0] as GroupsTableViewController;

        public IdentifyFaceViewController (IntPtr handle) : base (handle)
        {
        }


        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue (segue, sender);

            if (segue.Identifier == EmbedSegueId && segue.DestinationViewController is GroupsTableViewController groupsTVC)
            {
                groupsTVC.AutoSelect = true;
            }
        }


        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            SelectedFaceImageView.AddBorder (UIColor.Red, 2);
        }


        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);

            GroupsTableController.GroupSelectionChanged += OnGroupSelectionChanged;
            GoButton.TouchUpInside += IdentifyAction;
        }


        public override void ViewWillDisappear (bool animated)
        {
            GroupsTableController.GroupSelectionChanged -= OnGroupSelectionChanged;
            GoButton.TouchUpInside -= IdentifyAction;

            base.ViewWillDisappear (animated);
        }


        void OnGroupSelectionChanged (object sender, PersonGroup selection)
        {
            checkInputs ();
        }


        partial void ImageTapped (UITapGestureRecognizer sender)
        {
            ChooseImage ().Forget ();
        }


        async Task ChooseImage ()
        {
            var image = await this.ShowImageSelectionDialog ();

            if (image != null)
            {
                SelectedFaceImageView.Image = image;
                checkInputs ();
            }
        }


        void checkInputs ()
        {
            if (SelectedFaceImageView.Image != null && GroupsTableController.SelectedPersonGroup != null)
            {
                GoButton.Enabled = true;
            }
            else
            {
                GoButton.Enabled = false;
            }
        }


        void IdentifyAction (object sender, EventArgs e)
        {

        }
    }
}
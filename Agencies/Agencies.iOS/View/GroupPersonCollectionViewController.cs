using System;
using System.Threading.Tasks;
using Agencies.Shared;
using Foundation;
using UIKit;

namespace Agencies.iOS
{
    public partial class GroupPersonCollectionViewController : ThreeItemRowCollectionViewController
    {
        public PersonGroup Group { get; set; }

        public Person SelectedPerson { get; private set; }

        bool isForVerification;

        public GroupPersonCollectionViewController (IntPtr handle) : base (handle)
        {
        }


        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);

            if (IsInitialLoad)
            {
                if (Group != null && Group.People?.Count == 0)
                {
                    loadPeople ().Forget ();
                }
            }
            else
            {
                CollectionView.ReloadData ();
            }
        }


        public override void WillMoveToParentViewController (UIViewController parent)
        {
            base.WillMoveToParentViewController (parent);
        }


        async Task loadPeople ()
        {
            try
            {
                this.ShowHUD ("Loading group");

                await FaceClient.Shared.GetPeopleForGroup (Group);

                foreach (var person in Group.People)
                {
                    await FaceClient.Shared.GetFacesForPerson (person, Group);
                }

                CollectionView.ReloadData ();

                this.HideHUD ();
            }
            catch (Exception ex)
            {
                Log.Error ($"Error getting people for group (FaceClient.Shared.GetPeopleForGroup) :: {ex.Message}");

                this.ShowSimpleHUD ("Error retrieving people for group");
            }
        }


        public override nint NumberOfSections (UICollectionView collectionView) => Group?.People?.Count ?? 0;


        public override nint GetItemsCount (UICollectionView collectionView, nint section)
        {
            var faces = Group?.People? [(int)section]?.Faces;

            if (faces != null)
            {
                return faces.Count == 0 ? 1 : faces.Count; //always return 1 so we can draw a dummy face cell and allow deletion, etc.
            }

            return 0;
        }


        public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell ("Cell", indexPath) as GroupPersonCVC;

            var person = Group.People [indexPath.Section];

            cell.PersonName.Text = person.Name;
            cell.PersonImage.Tag = indexPath.Section; //keep track of the person this imageview is for - used in longPressAction
            cell.PersonImage.UserInteractionEnabled = true;

            if (person.Faces?.Count > 0)
            {
                var face = person.Faces? [indexPath.Row];

                if (face != null)
                {
                    cell.PersonImage.Image = UIImage.FromFile (face.PhotoPath);
                    cell.PersonImage.Layer.BorderWidth = 0;
                }
            }
            else
            {
                cell.PersonImage.Image = null;
                cell.PersonImage.Layer.BorderColor = UIColor.Red.CGColor;
                cell.PersonImage.Layer.BorderWidth = 2;
            }

            if (cell.PersonImage.GestureRecognizers == null || cell.PersonImage.GestureRecognizers?.Length == 0)
            {
                cell.PersonImage.AddGestureRecognizer (new UILongPressGestureRecognizer (longPressAction));
            }

            return cell;
        }


        async void longPressAction (UIGestureRecognizer gestureRecognizer)
        {
            if (gestureRecognizer.State == UIGestureRecognizerState.Began)
            {
                try
                {
                    var personIndex = gestureRecognizer.View.Tag;
                    var person = Group.People [(int)personIndex];

                    var result = await this.ShowActionSheet ($"Do you want to remove all of {person.Name}'s faces?", string.Empty, "Yes");

                    if (result == "Yes")
                    {
                        this.ShowHUD ($"Deleting {person.Name}");

                        await FaceClient.Shared.DeletePerson (person, Group);

                        this.ShowSimpleHUD ($"{person.Name} deleted");

                        CollectionView.ReloadData ();
                    }
                }
                catch (Exception)
                {
                    this.ShowSimpleAlert ("Failed to delete person.");
                }
            }
        }


        public async override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
        {
            SelectedPerson = Group.People [indexPath.Section];

            if (!isForVerification)
            {
                ParentViewController.PerformSegue (GroupDetailViewController.Segues.PersonDetail, this);
            }
            else
            {
                var choice = await this.ShowActionSheet ("Please choose", "What would you like to do with the selected person?", "Use for verification", "Edit");

                switch (choice)
                {
                    case "Use for verification":
                        //            UIViewController* verificationController = nil;
                        //            for (UIViewController* controller in self.navigationController.viewControllers) {
                        //                if ([controller isKindOfClass:[MPOVerificationViewController class]]) {
                        //                    verificationController = controller;
                        //                    [(MPOVerificationViewController *)controller didSelectPerson: (GroupPerson*)self.group.people[_selectedPersonIndex] inGroup:self.group];
                        //                }
                        //            }
                        //            [self.navigationController popToViewController:verificationController animated:YES];
                        break;
                    case "Edit":
                        ParentViewController.PerformSegue (GroupDetailViewController.Segues.PersonDetail, this);
                        break;
                    default:
                        return;
                }
            }
        }
    }
}
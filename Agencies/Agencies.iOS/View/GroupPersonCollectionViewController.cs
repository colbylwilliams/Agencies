using System;
using System.Threading.Tasks;
using Agencies.Shared;
using Foundation;
using UIKit;

namespace Agencies.iOS
{
    public partial class GroupPersonCollectionViewController : UICollectionViewController
    {
        public PersonGroup Group { get; set; }

        bool initialLoad = true;

        public GroupPersonCollectionViewController (IntPtr handle) : base (handle)
        {
        }


        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            loadPeople ().Forget ();
        }


        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);

            if (!initialLoad)
            {
                CollectionView.ReloadData ();
            }

            initialLoad = false;
        }


        async Task loadPeople ()
        {
            try
            {
                await FaceClient.Shared.GetPeopleForGroup (Group);

                foreach (var person in Group.People)
                {
                    await FaceClient.Shared.GetFacesForPerson (person, Group);
                }

                CollectionView.ReloadData ();
            }
            catch (Exception ex)
            {
                Log.Error ($"Error getting people for group (FaceClient.Shared.GetPeopleForGroup) :: {ex.Message}");
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
                }
            }
            else
            {
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

                    var result = await this.ShowActionSheet ("Do you want to remove all of this person's faces?", string.Empty, "Yes");

                    if (result == "Yes")
                    {
                        var person = Group.People [(int)personIndex];

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


        public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
        {
            //base.ItemSelected (collectionView, indexPath);

            //		_selectedPersonIndex = indexPath.section;
            //		if (self.isForVarification)
            //		{
            //			UIActionSheet* use_person_sheet = [[UIActionSheet alloc]

            //									 initWithTitle: @"Hint"

            //									 delegate:self
            //									 cancelButtonTitle:@"Cancel"

            //									 destructiveButtonTitle: nil
            //									  otherButtonTitles:@"Use this person for verification", @"Edit this person", nil];
            //			use_person_sheet.tag = 1;

            //	[use_person_sheet showInView:self.view];
            //       return;
            //   }
            //MPOPersonFacesController* controller = [[MPOPersonFacesController alloc] initWithGroup:self.group andPerson:self.group.people [indexPath.section]];
            //controller.needTraining = self.needTraining;
            //[self.navigationController pushViewController:controller animated:YES];

        }


        //        - (CGSize) collectionView:(UICollectionView*) collectionView layout:(UICollectionViewLayout*) collectionViewLayout sizeForItemAtIndexPath:(NSIndexPath*) indexPath
        //        {
        //    return CGSizeMake(_facesCollectionView.width / 3 - 10, (_facesCollectionView.width / 3 - 10) * 4 / 3);
        //}

        //- (CGFloat) collectionView:(UICollectionView*) collectionView layout:(UICollectionViewLayout*) collectionViewLayout minimumLineSpacingForSectionAtIndex:(NSInteger) section
        //    {
        //    return 10;
        //    }

        //- (CGFloat) collectionView:(UICollectionView*) collectionView layout:(UICollectionViewLayout*) collectionViewLayout minimumInteritemSpacingForSectionAtIndex:(NSInteger) section
        //{
        //return 10;
        //}


        //        - (void) actionSheet:(UIActionSheet*) actionSheet clickedButtonAtIndex:(NSInteger) buttonIndex
        //		{
        //    if (actionSheet.tag == 0) {
        //        
        //    } else {
        //        if (buttonIndex == 0) {
        //            UIViewController* verificationController = nil;
        //            for (UIViewController* controller in self.navigationController.viewControllers) {
        //                if ([controller isKindOfClass:[MPOVerificationViewController class]]) {
        //                    verificationController = controller;
        //                    [(MPOVerificationViewController *)controller didSelectPerson: (GroupPerson*)self.group.people[_selectedPersonIndex] inGroup:self.group];
        //                }
        //            }
        //            [self.navigationController popToViewController:verificationController animated:YES];
        //        } else if (buttonIndex == 1) {
        //            MPOPersonFacesController* controller = [[MPOPersonFacesController alloc] initWithGroup:self.group andPerson:self.group.people [_selectedPersonIndex]];
        //            controller.needTraining = self.needTraining;
        //            [self.navigationController pushViewController:controller animated:YES];
        //        }
        //    }
        //}

    }
}
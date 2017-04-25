using System;
using Foundation;
using UIKit;
using Agencies.Shared;
using System.Threading.Tasks;

namespace Agencies.iOS
{
    public partial class GroupDetailViewController : UIViewController
    {
        public PersonGroup Group { get; set; }

        public GroupDetailViewController (IntPtr handle) : base (handle)
        {
            //ContainerView.
        }


        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue (segue, sender);

            if (segue.Identifier == "Embed" && Group != null)
            {
                var groupPeopleCVC = segue.DestinationViewController as GroupPersonCollectionViewController;

                groupPeopleCVC.Group = Group;
            }
        }


        partial void SaveAction (NSObject sender)
        {
            //InvokeOnMainThread ();

            if (GroupName.Text.Length == 0)
            {
                this.ShowSimpleDialog ("Please input the group name");
                return;
            }

            if (Group == null)
            {
                createNewGroup ();
            }
            else
            {
                updateGroup ();
            }
        }


        async void updateGroup ()
        {
            try
            {
                this.ShowHUD ("Saving Group");

                await FaceClient.Shared.UpdatePersonGroup (Group, GroupName.Text);

                //_shouldExit = NO;
                await trainGroup ();
            }
            catch (Exception)
            {
                this.ShowSimpleDialog ("Failed to update group.");
            }
        }


        //        - (void) longPressAction: (UIGestureRecognizer*) gestureRecognizer
        //		{
        //    if (gestureRecognizer.state == UIGestureRecognizerStateBegan) {
        //        _selectedPersonIndex = gestureRecognizer.view.tag;
        //        UIActionSheet* confirm_sheet = [[UIActionSheet alloc]

        //										 initWithTitle:@"Do you want to remove all of this person's faces?"
        //                                         delegate:self
        //										 cancelButtonTitle:@"Cancel"

        //										 destructiveButtonTitle:nil
        //										 otherButtonTitles:@"Yes", nil];
        //        confirm_sheet.tag = 0;
        //        [confirm_sheet showInView:self.view];
        //    }
        //}



        //        - (void) addPerson: (id) sender
        //		{
        //    if (!self.group && _groupNameField.text.length == 0) {
        //        [CommonUtil simpleDialog:@"please input the group name"];
        //        return;
        //    }
        //    if (!self.group) {
        //        UIAlertView* alertView = [[UIAlertView alloc] initWithTitle:@"Hint"
        //                                                            message:@"Do you want to create this new group?"
        //                                                           delegate:self
        //												  cancelButtonTitle:@"No"

        //												  otherButtonTitles:@"Yes", nil];
        //        _intension = INTENSION_ADD_PERSON;
        //        alertView.tag = 0;
        //        [alertView show];
        //        return;
        //    }
        //MPOPersonFacesController* controller = [[MPOPersonFacesController alloc] initWithGroup:self.group];
        //    controller.needTraining = self.needTraining;
        //    [self.navigationController pushViewController:controller animated:YES];
        //}



        async void createNewGroup ()
        {
            try
            {
                this.ShowHUD ("Creating group");

                //var id = new NSUuid ().AsString ();

                Group = await FaceClient.Shared.CreatePersonGroup (GroupName.Text);

                this.ShowSimpleHUD ("Group created");
            }
            catch (Exception)
            {
                this.ShowSimpleDialog ("Failed to create group.");
            }


            //        if (_intension == INTENSION_ADD_PERSON) {
            //            MPOPersonFacesController* controller = [[MPOPersonFacesController alloc] initWithGroup:self.group];
            //            controller.needTraining = self.needTraining;
            //            [self.navigationController pushViewController:controller animated:YES];
            //        } else {
            //            [CommonUtil showSimpleHUD:@"Group created" forController:self.navigationController];
            //        }
            //    }];
            //}
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
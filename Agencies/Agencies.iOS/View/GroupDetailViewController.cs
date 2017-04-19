using System;
using Xamarin.Cognitive.Face.iOS;
using Foundation;
using UIKit;
using Agencies.Shared;
using System.Threading.Tasks;

namespace Agencies.iOS
{
    public partial class GroupDetailViewController : UIViewController
    {
        PersonGroup group;



        public GroupDetailViewController (IntPtr handle) : base (handle)
        {
            //ContainerView.
        }


        //public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        //{
        //    base.PrepareForSegue (segue, sender);

        //    if (segue.Identifier == "Embed")
        //    {
        //        var groupPeopleCVC = segue.DestinationViewController as GroupPersonCollectionViewController;

        //        groupPeopleCVC.Group = group;
        //    }
        //}


        partial void SaveAction (NSObject sender)
        {
            Task.Run (async () =>
            {



                if (GroupName.Text.Length == 0)
                {
                    this.ShowSimpleDialog ("Please input the group name");
                    return;
                }

                if (group == null)
                {
                    await createNewGroup ();
                }
                else
                {
                    try
                    {
                        this.ShowHUD ("Saving Group");

                        await FaceClient.Shared.UpdatePersonGroup (group, GroupName.Text);
                    }
                    catch (Exception)
                    {
                        this.ShowSimpleDialog ("Failed to update group.");
                    }


                    this.HideHUD ();

                    //_shouldExit = NO;
                    await trainGroup ();
                }
            });
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



        async Task createNewGroup ()
        {
            try
            {
                this.ShowHUD ("Creating group");

                var id = new NSUuid ().AsString ();

                await FaceClient.Shared.CreatePersonGroup (id, GroupName.Text);
            }
            catch (Exception)
            {
                this.ShowSimpleDialog ("Failed to create group.");
            }

            this.HideHUD ();


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

                await FaceClient.Shared.TrainGroup (group);

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
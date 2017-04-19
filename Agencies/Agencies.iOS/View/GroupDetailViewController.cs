using System;
using Xamarin.Cognitive.Face.iOS;
using Foundation;
using UIKit;
using Agencies.Shared;

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
            if (GroupName.Text.Length == 0)
            {
                this.ShowSimpleDialog ("Please input the group name");
                return;
            }

            if (group == null)
            {
                createNewGroup ();
            }
            else
            {
                var client = new MPOFaceServiceClient (FaceClient.Shared.SubscriptionKey);

                this.ShowHUD ("Saving Group");

                client.UpdatePersonGroupWithPersonGroupId (group.Id, GroupName.Text, null, error =>
                {
                    this.HideHUD ();

                    if (error != null)
                    {
                        this.ShowSimpleDialog ("Failed in updating group.");
                        return;
                    }

                    group.Name = GroupName.Text;
                    //_shouldExit = NO;
                    trainGroup ();
                });
            }
        }


        void createNewGroup ()
        {
            var client = new MPOFaceServiceClient (FaceClient.Shared.SubscriptionKey);

            this.ShowHUD ("Creating group");

            var id = new NSUuid ().AsString ();

            client.CreatePersonGroupWithId (id, GroupName.Text, null, error =>
            {
                this.HideHUD ();

                if (error != null)
                {
                    this.ShowSimpleDialog ("Failed in creating group.");
                    return;
                }

                group = new PersonGroup
                {
                    Name = GroupName.Text,
                    Id = id
                };

                FaceClient.Shared.Groups.Add (group);
            });
        }


        void trainGroup ()
        {
            var client = new MPOFaceServiceClient (FaceClient.Shared.SubscriptionKey);

            this.ShowHUD ("Training group");

            client.TrainPersonGroupWithPersonGroupId (group.Id, error =>
            {
                this.HideHUD ();

                if (error != null)
                {
                    this.ShowSimpleHUD ("Failed in training group.");
                }
                else
                {
                    this.ShowSimpleHUD ("This group is trained.");
                }

                //if (_shouldExit)
                //{
                //    this.NavigationController.PopViewController (true);
                //}
            });
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
}
using System;
using Cognitive.Face.iOS;
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
        }





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
                var client = new MPOFaceServiceClient (Keys.CognitiveServices.FaceApi.SubscriptionKey);

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
                    //trainGroup();

                });
            }
        }


        void createNewGroup ()
        {
            var client = new MPOFaceServiceClient (Keys.CognitiveServices.FaceApi.SubscriptionKey);

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

                group = new PersonGroup ();
                group.Name = GroupName.Text;
                group.Id = id;

                FaceClient.Current.Groups.Add (group);
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
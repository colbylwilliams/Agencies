using System;
using Agencies.Shared;
using Foundation;
using UIKit;

namespace Agencies.iOS
{
    public partial class GroupPersonCollectionViewController : UICollectionViewController
    {
        public PersonGroup Group { get; set; }

        public GroupPersonCollectionViewController (IntPtr handle) : base (handle)
        {
        }


        public override nint NumberOfSections (UICollectionView collectionView) => Group?.People?.Count ?? 0;


        public override nint GetItemsCount (UICollectionView collectionView, nint section) => Group?.People? [(int)section]?.Faces?.Count ?? 0;


        public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell ("Cell", indexPath) as GroupPersonCVC;

            var person = Group.People [indexPath.Section];
            var face = person.Faces [indexPath.Row];

            cell.PersonImage.Image = UIImage.FromFile (face.PhotoPath);
            cell.PersonName.Text = person.Name;

            //cell.faceImageView.tag = indexPath.section;
            //cell.faceImageView.userInteractionEnabled = YES;

            //if (cell.faceImageView.gestureRecognizers.count == 0)
            //{

            //[cell.faceImageView addGestureRecognizer:[[UILongPressGestureRecognizer alloc] initWithTarget:self action:@selector (longPressAction         

            return cell;
        }


        public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
        {
            base.ItemSelected (collectionView, indexPath);

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
        //        if (buttonIndex == 0) {
        //            MPOFaceServiceClient* client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:ProjectOxfordFaceSubscriptionKey];
        //            MBProgressHUD* HUD = [[MBProgressHUD alloc] initWithView:self.navigationController.view];
        //            [self.navigationController.view addSubview:HUD];
        //            HUD.labelText = @"Deleting this person";
        //            [HUD show: YES];

        //            [client deletePersonWithPersonGroupId:self.group.groupId personId:((GroupPerson*) self.group.people [_selectedPersonIndex]).personId completionBlock:^(NSError* error) {
        //                [HUD removeFromSuperview];
        //                if (error) {
        //                    [CommonUtil showSimpleHUD:@"Failed in deleting this person" forController:self.navigationController];
        //                    return;
        //                }
        //	[self.group.people removeObjectAtIndex:_selectedPersonIndex];
        //                [_facesCollectionView reloadData];
        //            }];
        //        }
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
using System;
using System.Threading.Tasks;
using Agencies.Shared;
using Foundation;
using UIKit;

namespace Agencies.iOS
{
    public partial class PersonDetailViewController : UIViewController
    {
        class Segues
        {
            public const string Embed = "Embed";
        }

        public PersonGroup Group { get; set; }
        public Person Person { get; set; }
        public bool NeedsTraining { get; set; }

        PersonFaceCollectionViewController PersonFaceCVC => ChildViewControllers [0] as PersonFaceCollectionViewController;

        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue (segue, sender);

            if (segue.Identifier == Segues.Embed && Group != null)
            {
                var personFaceCVC = segue.DestinationViewController as PersonFaceCollectionViewController;

                personFaceCVC.Person = Person;
            }
        }


        public PersonDetailViewController (IntPtr handle) : base (handle)
        {
        }


        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);

            if (Person != null)
            {
                PersonName.Text = Person.Name;
            }
        }


        partial void SaveAction (NSObject sender)
        {
            if (PersonName.Text.Length == 0)
            {
                this.ShowSimpleAlert ("Please input the person's name");
                return;
            }

            if (Person == null)
            {
                //     _intension = INTENSION_SAVE_PERSON;
                createNewPerson ().Forget ();
            }
            else
            {
                updatePerson ().Forget ();
            }
        }


        async Task createNewPerson ()
        {
            try
            {
                this.ShowHUD ("Creating person");

                Person = await FaceClient.Shared.CreatePerson (PersonName.Text, Group);

                PersonFaceCVC.Person = Person;
                PersonFaceCVC.CollectionView.ReloadData ();

                this.ShowSimpleHUD ("Person created");
            }
            catch (Exception)
            {
                this.ShowSimpleAlert ("Failed to create person.");
            }
        }


        async Task updatePerson ()
        {
            try
            {
                this.ShowHUD ("Saving person");

                await FaceClient.Shared.UpdatePerson (Person, Group, PersonName.Text);

                PersonFaceCVC.CollectionView.ReloadData ();

                this.ShowSimpleHUD ("Person saved");

                //reload collection view?
            }
            catch (Exception)
            {
                this.ShowSimpleAlert ("Failed to update person.");
            }
        }


        partial void AddFaceAction (NSObject sender)
        {

            if (PersonName.Text.Length == 0)
            {
                this.ShowSimpleAlert ("Please input the person's name");
                return;
            }

            AddFace ().Forget ();
        }


        async Task AddFace ()
        {
            if (Person == null)
            {
                var createPerson = await this.ShowTwoOptionAlert ("Create Person?", "Do you want to create this new person?");

                if (!createPerson)
                {
                    return;
                }

                await createNewPerson ();
            }

            if (Person != null) //just to make sure we succeeded in the case we created a new person above
            {
                var result = await this.ShowActionSheet ("Select Image", "How would you like to choose an image?", "Select from album", "Take a photo");
                UIImage image = null;

                switch (result)
                {
                    case "Select from album":
                        image = await this.ShowPhotoPicker ();
                        break;
                    case "Take a photo":
                        image = await this.ShowCameraPicker ();
                        break;
                    default:
                        return;
                }

                if (image != null)
                {
                    await detectFaces (image);
                }
            }
        }


        async Task detectFaces (UIImage image)
        {
            try
            {
                this.ShowHUD ("Detecting faces");

                var faces = await FaceClient.Shared.DetectFacesInPhoto (image);

                if (faces.Count == 0)
                {
                    this.ShowSimpleHUD ("No faces detected");
                }
                else if (faces.Count == 1)
                {
                    this.ShowHUD ("Adding faces");

                    await FaceClient.Shared.AddFaceForPerson (Person, Group, faces [0], image);

                    PersonFaceCVC.CollectionView.ReloadData ();

                    NeedsTraining = true;

                    this.ShowSimpleHUD ("Faces saved");
                }
                else // > 1 face
                {
                    //            MPOAddPersonFaceController* controller = [[MPOAddPersonFaceController alloc] init];
                    //            controller.group = self.group;
                    //            controller.person = self.person;
                    //            controller.detectedFaces = _detectedFaces;
                    //            controller.image = selectedImage;
                    //            controller.needTraining = self.needTraining;
                    //            [self.navigationController pushViewController:controller animated:YES];
                }
            }
            catch (Exception)
            {
                this.ShowSimpleAlert ("Face detection failed");
            }
        }
    }
}
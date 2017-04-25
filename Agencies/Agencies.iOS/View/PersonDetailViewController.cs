using System;
using Agencies.Shared;
using Foundation;
using UIKit;

namespace Agencies.iOS
{
    public partial class PersonDetailViewController : UIViewController
    {
		const string EmbedSegueId = "Embed";

        public PersonGroup Group { get; set; }
        public Person Person { get; set; }
        public bool NeedsTraining { get; set; }

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == EmbedSegueId && Group != null)
			{
				var personFaceCVC = segue.DestinationViewController as PersonFaceCollectionViewController;

				personFaceCVC.Person = Person;
			}
			//else if (segue.Identifier == AddPersonSegueId)
			//{
			//	var groupPersonVC = segue.DestinationViewController as PersonDetailViewController;

			//	groupPersonVC.Group = Group;
			//	groupPersonVC.NeedsTraining = this.NeedsTraining;
			//}
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

        }


        partial void AddFaceAction (NSObject sender)
        {

        }
    }
}
using System;
using System.Threading.Tasks;
using Agencies.Shared;
using Foundation;
using NomadCode.UIExtensions;
using UIKit;

namespace Agencies.iOS
{
    public partial class FaceViewController : UIViewController
    {
        class Segues
        {
            public const string Verification = "Verification";
        }

        VerificationType VerificationType { get; set; }


        public FaceViewController (IntPtr handle) : base (handle)
        {
        }


        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue (segue, sender);

            if (segue.Identifier == Segues.Verification && segue.DestinationViewController is VerificationViewController verifyVC)
            {
                verifyVC.VerificationType = VerificationType;
            }
        }


        partial void VerificationAction (NSObject sender)
        {
            ChooseVerify ().Forget ();
        }


        async Task ChooseVerify ()
        {
            VerificationType = await this.ShowActionSheet ("Please Choose", "Choose verification type",
                                                     ("Face and Face", VerificationType.FaceAndFace),
                                                     ("Face and Person", VerificationType.FaceAndPerson));

            if (VerificationType == VerificationType.None)
            {
                return;
            }

            PerformSegue (Segues.Verification, this);
        }
    }
}
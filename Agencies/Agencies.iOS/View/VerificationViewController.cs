using System;
using Agencies.Shared;
using Foundation;
using UIKit;

namespace Agencies.iOS
{
    public partial class VerificationViewController : UIViewController
    {
        class Segues
        {
            public const string Embed = "Embed";
        }

        public VerificationType VerificationType { get; set; }

        public VerificationViewController (IntPtr handle) : base (handle)
        {
        }
    }
}
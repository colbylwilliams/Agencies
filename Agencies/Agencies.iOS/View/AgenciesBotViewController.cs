using System;
using Foundation;
using NomadCode.BotFramework.iOS;
using UIKit;

namespace Agencies.iOS
{
    public partial class AgenciesBotViewController : BotViewController
    {
        [Export ("initWithCoder:")]
        public AgenciesBotViewController (NSCoder coder) : base (coder)
        {
        }


        public AgenciesBotViewController (IntPtr handle) : base (handle)
        {
        }


        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            // Perform any additional setup after loading the view, typically from a nib.
        }


        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
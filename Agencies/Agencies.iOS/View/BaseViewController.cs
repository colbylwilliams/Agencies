using System;
using UIKit;

namespace Agencies.iOS
{
    public class BaseViewController : UIViewController
    {
        protected bool IsInitialLoad { get; private set; } = true;

        public BaseViewController (IntPtr handle) : base (handle)
        {
        }


        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);

            IsInitialLoad = false;
        }
    }
}
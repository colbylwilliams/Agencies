using System;
using UIKit;

namespace Agencies.iOS
{
    public class BaseCollectionViewController : UICollectionViewController
    {
        protected bool IsInitialLoad { get; private set; } = true;

        public BaseCollectionViewController (IntPtr handle) : base (handle)
        {
        }


        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);

            IsInitialLoad = false;
        }
    }
}
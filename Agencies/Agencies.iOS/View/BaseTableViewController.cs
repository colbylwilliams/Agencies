using System;
using UIKit;

namespace Agencies.iOS
{
    public abstract class BaseTableViewController : UITableViewController
    {
        protected bool IsInitialLoad { get; private set; } = true;

		public BaseTableViewController (IntPtr handle) : base (handle)
        {
		}


		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			IsInitialLoad = false;
		}
    }
}
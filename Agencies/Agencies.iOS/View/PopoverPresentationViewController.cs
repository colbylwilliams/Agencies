using System;
using Foundation;
using UIKit;

namespace Agencies.iOS
{
	public abstract class PopoverPresentationViewController : UIViewController, IUIPopoverPresentationControllerDelegate
	{
		public PopoverPresentationViewController () : base ("PopoverPresentationViewController", null)
		{
		}


		public PopoverPresentationViewController (IntPtr handle) : base (handle)
		{
		}


		[Export ("adaptivePresentationStyleForPresentationController:")]
		public UIModalPresentationStyle GetAdaptivePresentationStyle (UIPresentationController forPresentationController)
		{
			return UIModalPresentationStyle.FullScreen;
		}


		[Export ("presentationController:viewControllerForAdaptivePresentationStyle:")]
		public UIViewController GetViewControllerForAdaptivePresentation (UIPresentationController controller, UIModalPresentationStyle style)
		{
			UINavigationController navController = new UINavigationController (controller.PresentedViewController);

			if (navController != null)
			{
				var closeText = GetPopoverCloseText (navController.TopViewController);
				var doneButton = new UIBarButtonItem (closeText, UIBarButtonItemStyle.Done, DoneTapped);
				navController.TopViewController.NavigationItem.RightBarButtonItem = doneButton;
			}

			return navController;
		}


		protected virtual string GetPopoverCloseText (UIViewController presentedViewController)
		{
			return "Cancel";
		}


		public void DoneTapped (object sender, EventArgs e)
		{
			DismissViewController (true, null);
		}
	}
}
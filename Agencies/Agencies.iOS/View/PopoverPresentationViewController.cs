using System;
using Foundation;
using UIKit;

namespace Agencies.iOS
{
	public abstract class PopoverPresentationViewController : BaseViewController, IUIPopoverPresentationControllerDelegate
	{
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
			var navController = new UINavigationController (controller.PresentedViewController);

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
			//noticing some memory being hung onto here, so explicitly disposing
			var navController = PresentedViewController as UINavigationController;

			PresentedViewController.PresentationController.PresentedViewController.Dispose ();
			PresentedViewController.PresentingViewController.DismissViewController (true, null);
			navController.Dispose ();
		}
	}
}
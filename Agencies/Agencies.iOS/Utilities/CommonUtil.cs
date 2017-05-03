using System;
using Foundation;
using MBProgressHUD;
using UIKit;

namespace Agencies.iOS
{
    public static class UIUtil
    {
        static MTMBProgressHUD currentHud;

        public static void ShowHUD (this UIViewController vc, string message)
        {
            vc.HideHUD ();

            var hud = new MTMBProgressHUD (vc.NavigationController.View)
            {
                LabelText = message,
                RemoveFromSuperViewOnHide = true
            };

            vc.NavigationController.View.AddSubview (hud);
            hud.Show (true);

            currentHud = hud;
        }


        public static UIViewController HideHUD (this UIViewController vc)
        {
            if (currentHud != null)
            {
                currentHud.Hide (true);
                currentHud.RemoveFromSuperview ();
                currentHud = null;
            }

            return vc;
        }


        public static void ShowSimpleHUD (this UIViewController vc, string message)
        {
            vc.HideHUD ();

            var hud = new MTMBProgressHUD (vc.NavigationController.View)
            {
                LabelText = message,
                Mode = MBProgressHUDMode.Text,
                RemoveFromSuperViewOnHide = true
            };

            vc.NavigationController.View.AddSubview (hud);
            hud.Show (true);
            hud.Hide (true, 1.5);
        }


        public static Exception ToException (this NSError error)
        {
            if (error != null)
            {
                return new Exception (error.Description);
            }

            return null;
        }
    }
}
﻿using MBProgressHUD;
using UIKit;

namespace Agencies.iOS
{
    public static class UIUtil
    {
        public static void ShowSimpleDialog (this UIViewController vc, string message)
        {
            var alertController = UIAlertController.Create ("Hint", message, UIAlertControllerStyle.Alert);

            alertController.ShowViewController (vc, vc);
            //UIAlertView alartView = new UIAlertView ("Hint", message, null, "Ok");
        }


        static MTMBProgressHUD currentHud;


        public static void ShowHUD (this UIViewController vc, string message)
        {
            if (currentHud != null)
            {
                vc.HideHUD ();
            }

            var hud = new MTMBProgressHUD (vc.View)
            {
                LabelText = message,
                RemoveFromSuperViewOnHide = true
            };

            vc.View.AddSubview (hud);
            hud.Show (true);

            currentHud = hud;
        }


        public static void HideHUD (this UIViewController vc)
        {
            if (currentHud != null)
            {
                currentHud.Hide (true);
                currentHud.RemoveFromSuperview ();
                currentHud = null;
            }
        }


        public static void ShowSimpleHUD (this UIViewController vc, string message)
        {
            var hud = new MTMBProgressHUD (vc.View)
            {
                LabelText = message,
                Mode = MBProgressHUDMode.Text,
                RemoveFromSuperViewOnHide = true
            };

            vc.View.AddSubview (hud);
            hud.Show (true);
            hud.Hide (true, 1.5);
        }
    }
}
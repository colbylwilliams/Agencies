using System;
using System.Drawing;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using MBProgressHUD;
using UIKit;
using Photos;

namespace Agencies.iOS
{
    public static class UIUtil
    {
        static MTMBProgressHUD currentHud;

        public static void ShowSimpleAlert (this UIViewController vc, string message, string title = "Hint", string okText = "Ok")
        {
            vc.HideHUD ();

            var alertController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);
            alertController.AddAction (UIAlertAction.Create (okText, UIAlertActionStyle.Cancel, null));

            vc.PresentViewController (alertController, true, null);
        }


        public static Task ShowSimpleAlertWithWait (this UIViewController vc, string message, string title = "Hint", string okText = "Ok")
        {
            vc.HideHUD ();

            var tcs = new TaskCompletionSource<bool> ();

            var alertController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);
            alertController.AddAction (UIAlertAction.Create (okText, UIAlertActionStyle.Cancel, a => tcs.SetResult (true)));

            vc.PresentViewController (alertController, true, null);

            return tcs.Task;
        }


        public static Task<bool> ShowTwoOptionAlert (this UIViewController vc, string title, string message, string yesText = "Yes", string noText = "No")
        {
            vc.HideHUD ();

            var alertController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);
            var tcs = new TaskCompletionSource<bool> ();

            alertController.AddAction (UIAlertAction.Create (yesText, UIAlertActionStyle.Default, a => tcs.SetResult (true)));
            alertController.AddAction (UIAlertAction.Create (noText, UIAlertActionStyle.Cancel, a => tcs.SetResult (false)));

            vc.PresentViewController (alertController, true, null);

            return tcs.Task;
        }


        public static Task<T> ShowActionSheet<T> (this UIViewController vc, string title, string message, params T [] options)
        {
            vc.HideHUD ();

            var alertController = UIAlertController.Create (title, message, UIAlertControllerStyle.ActionSheet);
            var tcs = new TaskCompletionSource<T> ();

            foreach (var option in options)
            {
                alertController.AddAction (UIAlertAction.Create (option.ToString (), UIAlertActionStyle.Default, a => tcs.SetResult (option)));
            }

            alertController.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, a => tcs.SetResult (default (T))));

            vc.PresentViewController (alertController, true, null);

            return tcs.Task;
        }


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


        public async static Task<UIImage> ShowPhotoPicker (this UIViewController vc)
        {
            if (await CheckPhotoPermission ())
            {
                return await vc.ShowMediaPicker (UIImagePickerControllerSourceType.PhotoLibrary);
            }

            throw new Exception ("Need photo permission in order to pick photo");
        }


        public static Task<UIImage> ShowCameraPicker (this UIViewController vc)
        {
            return vc.ShowMediaPicker (UIImagePickerControllerSourceType.Camera);
        }


        public static Task<UIImage> ShowMediaPicker (this UIViewController vc, UIImagePickerControllerSourceType sourceType, bool allowEditing = true)
        {
            var tcs = new TaskCompletionSource<UIImage> ();

            var picker = new UIImagePickerController
            {
                SourceType = sourceType,
                MediaTypes = UIImagePickerController.AvailableMediaTypes (sourceType),
                AllowsEditing = allowEditing
            };

            picker.FinishedPickingMedia += (sender, e) =>
            {
                var image = e.EditedImage ?? e.OriginalImage;

                picker.DismissViewController (true, null);

                tcs.SetResult (image);
            };

            picker.Canceled += (sender, e) => tcs.SetResult (null);

            vc.PresentViewController (picker, true, null);

            return tcs.Task;
        }


        public static Task<bool> CheckPhotoPermission ()
        {
            var tcs = new TaskCompletionSource<bool> ();
            var status = PHPhotoLibrary.AuthorizationStatus;

            switch (status)
            {
                case PHAuthorizationStatus.Authorized:
                    tcs.SetResult (true);
                    break;
                case PHAuthorizationStatus.Denied:
                case PHAuthorizationStatus.Restricted:
                    tcs.SetResult (false);
                    break;
                case PHAuthorizationStatus.NotDetermined:
                    PHPhotoLibrary.RequestAuthorization (newStatus =>
                    {
                        switch (newStatus)
                        {
                            case PHAuthorizationStatus.Authorized:
                                tcs.SetResult (true);
                                break;
                            case PHAuthorizationStatus.Denied:
                            case PHAuthorizationStatus.Restricted:
                            default:
                                tcs.SetResult (false);
                                break;
                        }
                    });
                    break;
            }

            return tcs.Task;
        }


        public static Exception ToException (this NSError error)
        {
            if (error != null)
            {
                return new Exception (error.Description);
            }

            return null;
        }


        //public static UIImage Crop (this UIImage image, RectangleF rect)
        //{
        //    return image.Crop ((CGRect)rect);
        //}


        public static UIImage Crop (this UIImage image, CGRect rect)
        {
            rect = new CGRect (rect.X * image.CurrentScale,
                               rect.Y * image.CurrentScale,
                               rect.Width * image.CurrentScale,
                               rect.Height * image.CurrentScale);

            using (CGImage cr = image.CGImage.WithImageInRect (rect))
            {
                var cropped = UIImage.FromImage (cr, image.CurrentScale, image.Orientation);

                return cropped;
            }
        }
    }
}
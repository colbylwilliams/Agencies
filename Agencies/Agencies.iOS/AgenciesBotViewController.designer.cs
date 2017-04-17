// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Agencies.iOS
{
    [Register ("AgenciesBotViewController")]
    partial class AgenciesBotViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem PeopleButton { get; set; }

        [Action ("PeopleAction:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void PeopleAction (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (PeopleButton != null) {
                PeopleButton.Dispose ();
                PeopleButton = null;
            }
        }
    }
}
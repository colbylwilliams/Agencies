// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Agencies.iOS
{
	[Register ("GroupDetailViewController")]
	partial class GroupDetailViewController
	{
		[Outlet]
		UIKit.UITextField GroupName { get; set; }

		[Action ("SaveAction:")]
		partial void SaveAction (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (GroupName != null) {
				GroupName.Dispose ();
				GroupName = null;
			}
		}
	}
}

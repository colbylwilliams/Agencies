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
	[Register ("GroupsViewController")]
	partial class GroupsViewController
	{
		[Outlet]
		UIKit.UITableView GroupTable { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (GroupTable != null) {
				GroupTable.Dispose ();
				GroupTable = null;
			}
		}
	}
}

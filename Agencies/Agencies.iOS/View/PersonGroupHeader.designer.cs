// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Agencies.iOS
{
	[Register ("PersonGroupHeader")]
	partial class PersonGroupHeader
	{
		[Outlet]
		UIKit.UILabel PersonGroupName { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (PersonGroupName != null) {
				PersonGroupName.Dispose ();
				PersonGroupName = null;
			}
		}
	}
}

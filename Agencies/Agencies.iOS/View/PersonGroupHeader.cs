using System;
using UIKit;

namespace Agencies.iOS
{
	public partial class PersonGroupHeader : UICollectionReusableView
	{
        public UILabel PersonGroupNameLabel => PersonGroupName;

		public PersonGroupHeader (IntPtr handle) : base (handle)
		{
		}
	}
}
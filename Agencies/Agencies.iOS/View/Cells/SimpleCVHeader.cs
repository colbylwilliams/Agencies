using System;
using UIKit;

namespace Agencies.iOS
{
	public partial class SimpleCVHeader : UICollectionReusableView
	{
		public SimpleCVHeader (IntPtr handle) : base (handle)
		{
		}


		internal void SetTitle (string title)
		{
			TitleLabel.Text = title;
		}
	}
}
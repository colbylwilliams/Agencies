using System;
using Agencies.Shared;
using UIKit;

namespace Agencies.iOS
{
	public partial class PersonGroupHeader : UICollectionReusableView
	{
		public PersonGroupHeader (IntPtr handle) : base (handle)
		{
		}


		internal void SetPerson (Person person)
		{
			PersonGroupName.Text = person.Name;
		}
	}
}
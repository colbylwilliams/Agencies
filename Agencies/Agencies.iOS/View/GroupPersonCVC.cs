using System;
using Agencies.iOS.Extensions;
using Agencies.Shared;
using NomadCode.UIExtensions;
using UIKit;

namespace Agencies.iOS
{
	public partial class GroupPersonCVC : UICollectionViewCell
	{
		public GroupPersonCVC (IntPtr handle) : base (handle)
		{
		}


		public void SetPerson (Person person, int cellActionTag, int faceIndex)
		{
			ImageView.Tag = cellActionTag; //keep track of the person this imageview is for - used in longPressAction

			if (person.Faces?.Count > 0)
			{
				var face = person.Faces? [faceIndex];

				if (face != null)
				{
					TextView.Text = $"Face #{faceIndex + 1}";
					ImageView.Image = face.GetImage ();
					ImageView.RemoveBorder ();
				}
			}
			else
			{
				ImageView.Image = null;
				ImageView.AddBorder (UIColor.Red, 2);
			}
		}


		public void SetLongPressAction (Action<UIGestureRecognizer> action)
		{
			if (ImageView.GestureRecognizers == null || ImageView.GestureRecognizers?.Length == 0)
			{
				ImageView.AddGestureRecognizer (new UILongPressGestureRecognizer (action));
			}
		}
	}
}
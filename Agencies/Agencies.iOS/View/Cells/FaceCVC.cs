using System;
using UIKit;
using CoreGraphics;
using Agencies.Shared;

namespace Agencies.iOS
{
	public partial class FaceCVC : UICollectionViewCell
	{
		Face face;

		public override bool Highlighted
		{
			get
			{
				return base.Highlighted;
			}
			set
			{
				base.Highlighted = value;
				SetNeedsDisplay ();
			}
		}


		public FaceCVC (IntPtr handle) : base (handle)
		{
		}


		public override void Draw (CGRect rect)
		{
			base.Draw (rect);

			if (Highlighted)
			{
				CGContext context = UIGraphics.GetCurrentContext ();

				context.SetFillColor (1, 0, 0, 1);
				context.FillRect (Bounds);
			}
		}


		public void SetFaceImage (Face face, UIImage image)
		{
			this.face = face;
			FaceImageView.Image = image;
		}
	}
}
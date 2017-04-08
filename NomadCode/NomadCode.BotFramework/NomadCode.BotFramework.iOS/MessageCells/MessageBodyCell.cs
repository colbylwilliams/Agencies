using System;

using UIKit;
using Foundation;

//using Nova.Unified;

using Xamarin.TTTAttributedLabel;

namespace NomadCode.BotFramework.iOS
{
	public class MessageBodyCell : UITableViewCell, ITTTAttributedLabelDelegate
	{
		public static readonly nfloat AvatarHeight = 24;
		//public static readonly nfloat MinimumHeight = 22;

		public static readonly NSString ReuseId = new NSString ("MessageBodyCell");


		TTTAttributedLabel _bodyLabel;

		public TTTAttributedLabel BodyLabel
		{
			get
			{
				if (_bodyLabel == null)
				{
					_bodyLabel = new TTTAttributedLabel
					{
						TranslatesAutoresizingMaskIntoConstraints = false,
						BackgroundColor = UIColor.White,
						UserInteractionEnabled = true,
						Lines = 0,
						TextColor = Colors.MessageColor,
						Font = AttributedStringUtilities.MessageFont,
						LinkAttributes = AttributedStringUtilities.LinkStringAttributes.Dictionary,
						EnabledTextCheckingTypes = NSTextCheckingType.Link,
						WeakDelegate = this
					};

					_bodyLabel.SetContentCompressionResistancePriority (250, UILayoutConstraintAxis.Vertical);
				}
				return _bodyLabel;
			}
		}

		public NSIndexPath IndexPath { get; set; }

		public bool UsedForMessage { get; set; }

		public decimal TimeStamp { get; set; }


		[Export ("initWithStyle:reuseIdentifier:")]
		public MessageBodyCell (UITableViewCellStyle style, NSString reuseIdentifier) : base (style, reuseIdentifier)
		{
			SelectionStyle = UITableViewCellSelectionStyle.None;
			BackgroundColor = UIColor.White;

			configureSubviews ();
			configureConstraints ();
		}


		public override void PrepareForReuse ()
		{
			base.PrepareForReuse ();

			SelectionStyle = UITableViewCellSelectionStyle.None;
		}


		internal virtual void configureSubviews ()
		{
			ContentView.AddSubview (BodyLabel);
		}


		internal virtual void configureConstraints ()
		{
			var views = new NSDictionary (
				new NSString (@"bodyLabel"), BodyLabel
			);

			var metrics = new NSDictionary (
				new NSString (@"tumbSize"), NSNumber.FromNFloat (AvatarHeight + 15),
				new NSString (@"right"), NSNumber.FromNFloat (10)
			);

			ContentView.AddConstraints (NSLayoutConstraint.FromVisualFormat (@"H:|-tumbSize-[bodyLabel(>=0)]-right-|", 0, metrics, views));
			ContentView.AddConstraints (NSLayoutConstraint.FromVisualFormat (@"V:|[bodyLabel]|", 0, metrics, views));
		}


		//public virtual void SetMessage (string message)
		//{
		//	BodyLabel.Text = message;
		//	//BodyLabel.SetText (message);
		//}

		public virtual void SetMessage (NSAttributedString message)
		{
			//BodyLabel.AttributedText = message;
			BodyLabel.SetText (message);
		}

		[Export ("attributedLabel:didSelectLinkWithURL:")]
		public void DidSelectLinkWithURL (TTTAttributedLabel label, NSUrl url)
		{
			Console.WriteLine ($"DidSelectLinkWithURL Label = {label}, Url = {url})");
		}
	}
}
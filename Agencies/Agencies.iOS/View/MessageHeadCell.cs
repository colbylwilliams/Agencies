using System;
using Foundation;
//using Nova.Unified;
using UIKit;
//using Nova.Slack.Events;
using Agencies.Domain;


namespace Agencies.iOS
{
	public class MessageHeadCell : MessageBodyCell//, IMessageHeadCell
	{
		//public static readonly new nfloat MinimumHeight = 40;
		public static readonly nfloat AutoCompleteHeight = 50;

		public static readonly new NSString ReuseId = new NSString ("MessageHeadCell");

		public static readonly NSString AutoCompleteReuseId = new NSString ("AutoCompletionCell");

		UIImageView _thumbnailView;

		UILabel _titleLabel, _timestampLabel;

		public UILabel TitleLabel {
			get {
				if (_titleLabel == null)
				{
					_titleLabel = new UILabel
					{
						TranslatesAutoresizingMaskIntoConstraints = false,
						BackgroundColor = UIColor.White,
						UserInteractionEnabled = false,
						Lines = 0,
						TextColor = Colors.MessageColor,
						Font = UIFont.PreferredHeadline// UIFont.BoldSystemFontOfSize (DefaultFontSize)
					};
				}
				return _titleLabel;
			}
		}

		public UILabel TimestampLabel {
			get {
				if (_timestampLabel == null)
				{
					_timestampLabel = new UILabel
					{
						TranslatesAutoresizingMaskIntoConstraints = false,
						BackgroundColor = UIColor.White,
						UserInteractionEnabled = false,
						Lines = 0,
						TextColor = UIColor.LightGray,
						Font = UIFont.PreferredCaption2// UIFont.SystemFontOfSize (14)
					};
				}
				return _timestampLabel;
			}
		}

		public UIImageView ThumbnailView {
			get {
				if (_thumbnailView == null)
				{
					_thumbnailView = new UIImageView
					{
						TranslatesAutoresizingMaskIntoConstraints = false,
						UserInteractionEnabled = false,
						BackgroundColor = UIColor.FromWhiteAlpha (0.9f, 1.0f),
					};
					_thumbnailView.Layer.CornerRadius = 4;// AvatarHeight / 2;
					_thumbnailView.Layer.MasksToBounds = true;
				}
				return _thumbnailView;
			}
		}


		[Export ("initWithStyle:reuseIdentifier:")]
		public MessageHeadCell (UITableViewCellStyle style, NSString reuseIdentifier) : base (style, reuseIdentifier) { }


		public override void PrepareForReuse ()
		{
			base.PrepareForReuse ();

			TitleLabel.Font = UIFont.PreferredHeadline;//UIFont.BoldSystemFontOfSize (DefaultFontSize);
			TimestampLabel.Font = UIFont.PreferredCaption2;// UIFont.SystemFontOfSize (10);

			TitleLabel.Text = string.Empty;
			TimestampLabel.Text = string.Empty;
		}


		override internal void configureSubviews ()
		{
			base.configureSubviews ();

			ContentView.AddSubview (ThumbnailView);
			ContentView.AddSubview (TitleLabel);
			ContentView.AddSubview (TimestampLabel);
		}


		override internal void configureConstraints ()
		{
			var views = new NSDictionary (
				new NSString (@"thumbnailView"), ThumbnailView,
				new NSString (@"titleLabel"), TitleLabel,
				new NSString (@"timestampLabel"), TimestampLabel,
				new NSString (@"bodyLabel"), BodyLabel
			);

			var metrics = new NSDictionary (
				new NSString (@"tumbSize"), NSNumber.FromNFloat (AvatarHeight),
				new NSString (@"padding"), NSNumber.FromNFloat (15),
				new NSString (@"right"), NSNumber.FromNFloat (10),
				new NSString (@"left"), NSNumber.FromNFloat (5)
			);

			ContentView.AddConstraints (NSLayoutConstraint.FromVisualFormat (@"H:|-left-[thumbnailView(tumbSize)]-right-[titleLabel(>=0)]-left-[timestampLabel]-(>=right)-|", 0, metrics, views));
			ContentView.AddConstraints (NSLayoutConstraint.FromVisualFormat (@"H:|-left-[thumbnailView(tumbSize)]-right-[bodyLabel(>=0)]-right-|", 0, metrics, views));
			ContentView.AddConstraints (NSLayoutConstraint.FromVisualFormat (@"V:|-padding-[thumbnailView(tumbSize)]-(>=0)-|", 0, metrics, views));

			if (ReuseIdentifier.IsEqual (ReuseId))
			{
				ContentView.AddConstraints (NSLayoutConstraint.FromVisualFormat (@"V:|-right-[titleLabel(24)]-left-[bodyLabel(>=0@999)]-left-|", 0, metrics, views));
			}
			else
			{
				ContentView.AddConstraints (NSLayoutConstraint.FromVisualFormat (@"V:|[titleLabel]|", 0, metrics, views));
			}

			ContentView.AddConstraint (NSLayoutConstraint.Create (TitleLabel, NSLayoutAttribute.Baseline, NSLayoutRelation.Equal, TimestampLabel, NSLayoutAttribute.Baseline, 1, 0));
		}


		long loadingTicks;


		public long SetMessage (Message message) => SetMessage (message.LocalTimeStamp, message.Activity.From.Name, message.AttributedText);


		public long SetMessage (DateTime? timestamp, string username, NSAttributedString attrMessage)
		{
			loadingTicks = DateTime.UtcNow.Ticks;

			TitleLabel.Text = username;
			TimestampLabel.Text = timestamp?.ToShortTimeString ();

			SetMessage (attrMessage);

			return loadingTicks;
		}

		public bool SetAvatar (long key, UIImage avatar)
		{
			var same = key == loadingTicks;

			ThumbnailView.Image = same ? avatar : null;

			return same;
		}
	}
}
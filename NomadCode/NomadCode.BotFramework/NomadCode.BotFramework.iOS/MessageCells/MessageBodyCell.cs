using System;

using UIKit;
using Foundation;

using Xamarin.TTTAttributedLabel;

namespace NomadCode.BotFramework.iOS
{
    public class MessageBodyCell : UITableViewCell, ITTTAttributedLabelDelegate
    {
        public static readonly nfloat AvatarHeight = 24;
        public static readonly nfloat AutoCompleteHeight = 50;
        //public static readonly nfloat MinimumHeight = 22;

        public static readonly NSString MessageCellReuseId = new NSString("MessageCellReuseId");
        public static readonly NSString MessageHeaderCellReuseId = new NSString("MessageHeaderCellReuseId");

        public static readonly NSString HeroCellReuseId = new NSString("HeroCellReuseId");
        public static readonly NSString HeroHeaderCellReuseId = new NSString("HeroHeaderCellReuseId");
        //Receipt
        public static readonly NSString ThumbnailCellReuseId = new NSString("ThumbnailCellReuseId");
        public static readonly NSString ThumbnailHeaderCellReuseId = new NSString("ThumbnailHeaderCellReuseId");

        public static readonly NSString ReceiptCellReuseId = new NSString("ReceiptCellReuseId");
        public static readonly NSString ReceiptHeaderCellReuseId = new NSString("ReceiptHeaderCellReuseId");

        public static readonly NSString SigninCellReuseId = new NSString("SigninCellReuseId");
        public static readonly NSString SigninHeaderCellReuseId = new NSString("SigninHeaderCellReuseId");

        public static readonly NSString AutoCompleteReuseId = new NSString("AutoCompletionCell");

        long loadingTicks;


        #region Views

        UIImageView _avatarView, _heroImageView, _thumbnailImageView;

        TTTAttributedLabel _bodyLabel;

        UILabel _titleLabel, _timestampLabel;

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

                    _bodyLabel.SetContentCompressionResistancePriority(250, UILayoutConstraintAxis.Vertical);
                }
                return _bodyLabel;
            }
        }

        public UILabel TitleLabel
        {
            get
            {
                if (_titleLabel == null)
                {
                    _titleLabel = new UILabel
                    {
                        TranslatesAutoresizingMaskIntoConstraints = false,
                        BackgroundColor = UIColor.White,
                        UserInteractionEnabled = false,
                        Lines = 0,
                        TextColor = Colors.MessageColor,
                        Font = AttributedStringUtilities.HeaderFont,
                    };

                    _titleLabel.SetContentCompressionResistancePriority(300, UILayoutConstraintAxis.Vertical);
                }
                return _titleLabel;
            }
        }

        public UILabel TimestampLabel
        {
            get
            {
                if (_timestampLabel == null)
                {
                    _timestampLabel = new UILabel
                    {
                        TranslatesAutoresizingMaskIntoConstraints = false,
                        BackgroundColor = UIColor.White,
                        UserInteractionEnabled = false,
                        Lines = 0,
                        TextColor = UIColor.LightGray,
                        Font = AttributedStringUtilities.TimestampFont
                    };
                }
                return _timestampLabel;
            }
        }

        public UIImageView AvatarView
        {
            get
            {
                if (_avatarView == null)
                {
                    _avatarView = new UIImageView
                    {
                        TranslatesAutoresizingMaskIntoConstraints = false,
                        UserInteractionEnabled = false,
                        BackgroundColor = UIColor.FromWhiteAlpha(0.9f, 1.0f),
                    };
                    _avatarView.Layer.CornerRadius = 4;
                    _avatarView.Layer.MasksToBounds = true;
                }
                return _avatarView;
            }
        }

        public UIImageView HeroImageView
        {
            get
            {
                if (_heroImageView == null)
                {
                    _heroImageView = new UIImageView
                    {
                        TranslatesAutoresizingMaskIntoConstraints = false,
                        UserInteractionEnabled = false,
                        BackgroundColor = UIColor.FromWhiteAlpha(0.9f, 1.0f),
                    };
                    _heroImageView.Layer.CornerRadius = 4;
                    _heroImageView.Layer.MasksToBounds = true;
                }
                return _heroImageView;
            }
        }

        public UIImageView ThumbnailImageView
        {
            get
            {
                if (_thumbnailImageView == null)
                {
                    _thumbnailImageView = new UIImageView
                    {
                        TranslatesAutoresizingMaskIntoConstraints = false,
                        UserInteractionEnabled = false,
                        BackgroundColor = UIColor.FromWhiteAlpha(0.9f, 1.0f),
                    };
                    _thumbnailImageView.Layer.CornerRadius = 4;
                    _thumbnailImageView.Layer.MasksToBounds = true;
                }
                return _thumbnailImageView;
            }
        }

        #endregion

        bool headerCell;

        public NSIndexPath IndexPath { get; set; }

        //public bool UsedForMessage { get; set; }

        //public decimal TimeStamp { get; set; }


        [Export("initWithStyle:reuseIdentifier:")]
        public MessageBodyCell(UITableViewCellStyle style, NSString reuseIdentifier) : base(style, reuseIdentifier)
        {
            Log.Debug(reuseIdentifier.ToString());

            SelectionStyle = UITableViewCellSelectionStyle.None;
            BackgroundColor = UIColor.White;

            headerCell = reuseIdentifier == MessageHeaderCellReuseId
                      || reuseIdentifier == HeroHeaderCellReuseId
                      || reuseIdentifier == ThumbnailHeaderCellReuseId
                      || reuseIdentifier == ReceiptHeaderCellReuseId
                      || reuseIdentifier == SigninHeaderCellReuseId;

            if (headerCell)
            {
                ContentView.AddSubview(AvatarView);
                ContentView.AddSubview(TitleLabel);
                ContentView.AddSubview(TimestampLabel);
            }

            if (reuseIdentifier == MessageCellReuseId)
            {
                ContentView.AddSubview(BodyLabel);

                configureConstraintsForMessageCell();
            }
            else if (reuseIdentifier == MessageHeaderCellReuseId)
            {
                ContentView.AddSubview(BodyLabel);

                configureConstraintsForMessageHeaderCell();
            }
        }


        public override void PrepareForReuse()
        {
            base.PrepareForReuse();

            if (headerCell)
            {
                TitleLabel.Font = AttributedStringUtilities.HeaderFont;
                TimestampLabel.Font = AttributedStringUtilities.TimestampFont;

                TitleLabel.Text = string.Empty;
                TimestampLabel.Text = string.Empty;
            }

            SelectionStyle = UITableViewCellSelectionStyle.None;
        }


        void configureConstraintsForMessageCell()
        {
            var views = new NSDictionary(
                new NSString(@"bodyLabel"), BodyLabel
            );

            var metrics = new NSDictionary(
                new NSString(@"avatarSize"), NSNumber.FromNFloat(AvatarHeight + 15),
                new NSString(@"right"), NSNumber.FromNFloat(10)
            );

            ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat(@"H:|-avatarSize-[bodyLabel(>=0)]-right-|", 0, metrics, views));
            ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat(@"V:|[bodyLabel]|", 0, metrics, views));
        }


        void configureConstraintsForMessageHeaderCell()
        {
            var views = new NSDictionary(
                new NSString(@"avatarView"), AvatarView,
                new NSString(@"titleLabel"), TitleLabel,
                new NSString(@"timestampLabel"), TimestampLabel,
                new NSString(@"bodyLabel"), BodyLabel
            );

            var metrics = new NSDictionary(
                new NSString(@"avatarSize"), NSNumber.FromNFloat(AvatarHeight),
                new NSString(@"padding"), NSNumber.FromNFloat(13),
                new NSString(@"right"), NSNumber.FromNFloat(10),
                new NSString(@"left"), NSNumber.FromNFloat(5)
            );

            ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat(@"H:|-left-[avatarView(avatarSize)]-right-[titleLabel(>=0)]-left-[timestampLabel]-(>=right)-|", 0, metrics, views));
            ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat(@"H:|-left-[avatarView(avatarSize)]-right-[bodyLabel(>=0)]-right-|", 0, metrics, views));
            ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat(@"V:|-padding-[avatarView(avatarSize)]-(>=0)-|", 0, metrics, views));

            if (ReuseIdentifier.IsEqual(MessageHeaderCellReuseId))
            {
                ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat(@"V:|-right-[titleLabel]-(>=0@999)-[bodyLabel]-left-|", 0, metrics, views));
            }
            else
            {
                ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat(@"V:|[titleLabel]|", 0, metrics, views));
            }

            ContentView.AddConstraint(NSLayoutConstraint.Create(TitleLabel, NSLayoutAttribute.Baseline, NSLayoutRelation.Equal, TimestampLabel, NSLayoutAttribute.Baseline, 1, 0.5f));
        }


        void configureConstraintsForHeroCell()
        {
            var views = new NSDictionary(
                new NSString(@"avatarView"), AvatarView,
                new NSString(@"titleLabel"), TitleLabel,
                new NSString(@"timestampLabel"), TimestampLabel,
                new NSString(@"bodyLabel"), BodyLabel
            );

            var metrics = new NSDictionary(
                new NSString(@"avatarSize"), NSNumber.FromNFloat(AvatarHeight),
                new NSString(@"padding"), NSNumber.FromNFloat(13),
                new NSString(@"right"), NSNumber.FromNFloat(10),
                new NSString(@"left"), NSNumber.FromNFloat(5)
            );

            ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat(@"H:|-left-[avatarView(avatarSize)]-right-[titleLabel(>=0)]-left-[timestampLabel]-(>=right)-|", 0, metrics, views));
            ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat(@"H:|-left-[avatarView(avatarSize)]-right-[bodyLabel(>=0)]-right-|", 0, metrics, views));
            ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat(@"V:|-padding-[avatarView(avatarSize)]-(>=0)-|", 0, metrics, views));

            if (ReuseIdentifier.IsEqual(MessageHeaderCellReuseId))
            {
                ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat(@"V:|-right-[titleLabel]-(>=0@999)-[bodyLabel]-left-|", 0, metrics, views));
            }
            else
            {
                ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat(@"V:|[titleLabel]|", 0, metrics, views));
            }

            ContentView.AddConstraint(NSLayoutConstraint.Create(TitleLabel, NSLayoutAttribute.Baseline, NSLayoutRelation.Equal, TimestampLabel, NSLayoutAttribute.Baseline, 1, 0.5f));
        }


        void configureConstraintsForHeroHeaderCell()
        {
            var views = new NSDictionary(
                new NSString(@"avatarView"), AvatarView,
                new NSString(@"titleLabel"), TitleLabel,
                new NSString(@"timestampLabel"), TimestampLabel,
                new NSString(@"bodyLabel"), BodyLabel
            );

            var metrics = new NSDictionary(
                new NSString(@"avatarSize"), NSNumber.FromNFloat(AvatarHeight),
                new NSString(@"padding"), NSNumber.FromNFloat(13),
                new NSString(@"right"), NSNumber.FromNFloat(10),
                new NSString(@"left"), NSNumber.FromNFloat(5)
            );

            ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat(@"H:|-left-[avatarView(avatarSize)]-right-[titleLabel(>=0)]-left-[timestampLabel]-(>=right)-|", 0, metrics, views));
            ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat(@"H:|-left-[avatarView(avatarSize)]-right-[bodyLabel(>=0)]-right-|", 0, metrics, views));
            ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat(@"V:|-padding-[avatarView(avatarSize)]-(>=0)-|", 0, metrics, views));

            if (ReuseIdentifier.IsEqual(MessageHeaderCellReuseId))
            {
                ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat(@"V:|-right-[titleLabel]-(>=0@999)-[bodyLabel]-left-|", 0, metrics, views));
            }
            else
            {
                ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat(@"V:|[titleLabel]|", 0, metrics, views));
            }

            ContentView.AddConstraint(NSLayoutConstraint.Create(TitleLabel, NSLayoutAttribute.Baseline, NSLayoutRelation.Equal, TimestampLabel, NSLayoutAttribute.Baseline, 1, 0.5f));
        }


        public void SetMessage(NSAttributedString message) => BodyLabel.SetText(message);


        public long SetMessage(DateTime? timestamp, string username, NSAttributedString attrMessage)
        {
            loadingTicks = DateTime.UtcNow.Ticks;

            TitleLabel.Text = username;
            TimestampLabel.Text = timestamp?.ToShortTimeString();

            SetMessage(attrMessage);

            return loadingTicks;
        }


        public bool SetAvatar(long key, UIImage avatar)
        {
            var same = key == loadingTicks;

            AvatarView.Image = same ? avatar : null;

            return same;
        }


        [Export("attributedLabel:didSelectLinkWithURL:")]
        public void DidSelectLinkWithURL(TTTAttributedLabel label, NSUrl url)
        {
            Console.WriteLine($"DidSelectLinkWithURL Label = {label}, Url = {url})");
        }
    }
}
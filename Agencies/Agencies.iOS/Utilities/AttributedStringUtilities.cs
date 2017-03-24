using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UIKit;

using Foundation;

namespace Agencies.iOS
{
	public enum SlackLinkStringType
	{
		Unknown,
		User,
		Channel,
		Group
	}

	public class SlackLinkString
	{
		public SlackLinkStringType LinkType { get; set; }

		public string Original { get; set; }

		public string Display { get; set; }

		public NSUrl Link { get; set; }
	}


	public static class SlackLinkStringExtensions
	{
		static char [] TrimWrapper = { '<', '>' };

		const char pipeChar = '|';
		const char atChar = '@';
		const char hashChar = '#';



		const string userFmt = "@{0}";
		const string channelFmt = "#{0}";

		const string userPrefix = @"@U";
		const string channelPrefix = @"#C";

		const string channelCallout = @"!channel";

		const string messageFmt = @"<(.*?)>";

		public static SlackLinkString ToSlackLinkString (this string orig)
		{
			var slackString = new SlackLinkString ();

			slackString.Original = orig;

			var strArr = orig.Trim (TrimWrapper).Split (pipeChar);

			if (orig.Contains (userPrefix))
			{
				slackString.Display = (strArr.Length > 1) ? string.Format (userFmt, strArr [1]) : strArr [0].Trim (atChar);
			}
			else if (orig.Contains (channelPrefix))
			{
				slackString.Display = (strArr.Length > 1) ? string.Format (channelFmt, strArr [1]) : strArr [0].Trim (hashChar);
			}
			else
			{
				slackString.Display = (strArr.Length > 1) ? strArr [1] : strArr [0];
			}

			slackString.Link = (strArr.Length > 1) ? NSUrl.FromString (strArr [0]) : null;

			return slackString;
		}

		public static SlackLinkStringType GetSlackLinkType (this string orig)
		{
			return SlackLinkStringType.Unknown;
		}
	}


	public static class AttributedStringUtilities
	{
		const string messageFmt = @"<(.*?)>";

		static NSMutableParagraphStyle messageParagraphStyle = new NSMutableParagraphStyle
		{
			LineBreakMode = UILineBreakMode.WordWrap,
			Alignment = UITextAlignment.Left
		};


		public static UIStringAttributes MessageStringAttributes = new UIStringAttributes
		{
			Font = UIFont.PreferredBody,
			ParagraphStyle = messageParagraphStyle,
			ForegroundColor = Colors.MessageColor
		};


		public static UIStringAttributes LinkStringAttributes = new UIStringAttributes
		{
			Font = UIFont.PreferredBody,
			UnderlineStyle = NSUnderlineStyle.None,
			ParagraphStyle = messageParagraphStyle,
			ForegroundColor = Colors.MessageLinkColor
		};


		public static NSMutableAttributedString GetMessageAttributedString (this string message)
		{
			var linkStrings = new List<SlackLinkString> ();

			var messageCopy = message;

			for (Match match = Regex.Match (message, messageFmt); match.Success; match = match.NextMatch ())
				linkStrings.Add (match.Value.ToSlackLinkString ());

			foreach (var linkString in linkStrings)
				messageCopy = messageCopy.Replace (linkString.Original, linkString.Display);

			var attrString = new NSMutableAttributedString (messageCopy);

			attrString.AddAttributes (MessageStringAttributes, new NSRange (0, messageCopy.Length));

			foreach (var linkString in linkStrings)
			{
				var range = new NSRange (messageCopy.IndexOf (linkString.Display, StringComparison.Ordinal), linkString.Display.Length);

				if (linkString.Link != null)
					attrString.AddAttribute (UIStringAttributeKey.Link, linkString.Link, range);

				attrString.AddAttributes (LinkStringAttributes, range);
			}

			return attrString;
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UIKit;

using Foundation;

namespace Agencies.iOS
{
	public enum MarkdownFmtStringType
	{
		Unknown,
		Link,
		Bold,
		Italic,
		Strikethrough,
		UnorderedList,
		OrderedList,
		Pre,
		BlockQuote,
		Image
	}


	public class MarkdownFmtString
	{
		public MarkdownFmtStringType FmtStringType { get; set; }

		public string Original { get; set; }

		public string Display { get; set; }

		public NSUrl Link { get; set; }
	}


	public static class AttributedStringUtilities
	{
		const string linkFmt = @"(?<!\!)\[(.*?)\)";
		const string boldFmt = @"\*\*(.*?)\*\*";
		const string italicFmt = @"(?<!\*)\*{1}([^*].*?)\*{1}(?!\*)";
		const string imageFmt = @"\!\[(.*?)\)";
		const string strikeFmt = @"\~\~(.*?)\~\~";
		const string ulFmt = @"\s+[*•-]\s+(.*)";
		const string olFmt = @"\s+\d+\.\s+(.*)";
		const string preFmt = @"\`(.*?)\`";
		const string quoteFmt = @"\s+\>+\s+(.*)";

		static readonly char [] linkTrim = { '[', ')' };
		static readonly char [] boldTrim = { '*' };
		static readonly char [] italicTrim = { '*' };
		static readonly char [] imageTrim = { '!', '[', ')' };
		static readonly char [] strikeTrim = { '~' };
		static readonly char [] preTrim = { '`' };

		static readonly Tuple<char, char> ulReplace = new Tuple<char, char> ('*', '•');


		static readonly string [] linkSplit = { "](" };


		static nfloat fontSize = 16;

		static readonly UIFont messageFont = UIFont.SystemFontOfSize (fontSize);
		static UIFont messageFontBold = UIFont.BoldSystemFontOfSize (fontSize);
		static UIFont messageFontItalic = UIFont.ItalicSystemFontOfSize (fontSize);
		static UIFont messageFontPre = UIFont.FromName ("Menlo-Regular", 15);


		static readonly NSMutableParagraphStyle messageParagraphStyle = new NSMutableParagraphStyle
		{
			LineBreakMode = UILineBreakMode.WordWrap,
			Alignment = UITextAlignment.Left
		};



		public static NSMutableAttributedString GetMessageAttributedString (this string message)
		{
			var markdownFmtStrings = new List<MarkdownFmtString> ();

			//message = message.Replace ("\r\n*", "\r\n•");

			Log.Debug (message);

			var messageCopy = message;

			for (Match match = Regex.Match (message, linkFmt); match.Success; match = match.NextMatch ())
				markdownFmtStrings.Add (match.Value.ToSlackLinkString (MarkdownFmtStringType.Link));

			for (Match match = Regex.Match (message, imageFmt); match.Success; match = match.NextMatch ())
				markdownFmtStrings.Add (match.Value.ToSlackLinkString (MarkdownFmtStringType.Image));

			for (Match match = Regex.Match (message, boldFmt); match.Success; match = match.NextMatch ())
				markdownFmtStrings.Add (match.Value.ToSlackLinkString (MarkdownFmtStringType.Bold));

			for (Match match = Regex.Match (message, italicFmt); match.Success; match = match.NextMatch ())
				markdownFmtStrings.Add (match.Value.ToSlackLinkString (MarkdownFmtStringType.Italic));

			for (Match match = Regex.Match (message, ulFmt); match.Success; match = match.NextMatch ())
				markdownFmtStrings.Add (match.Value.ToSlackLinkString (MarkdownFmtStringType.UnorderedList));

			for (Match match = Regex.Match (message, olFmt); match.Success; match = match.NextMatch ())
				markdownFmtStrings.Add (match.Value.ToSlackLinkString (MarkdownFmtStringType.OrderedList));

			for (Match match = Regex.Match (message, strikeFmt); match.Success; match = match.NextMatch ())
				markdownFmtStrings.Add (match.Value.ToSlackLinkString (MarkdownFmtStringType.Strikethrough));

			for (Match match = Regex.Match (message, preFmt); match.Success; match = match.NextMatch ())
				markdownFmtStrings.Add (match.Value.ToSlackLinkString (MarkdownFmtStringType.Pre));

			for (Match match = Regex.Match (message, quoteFmt); match.Success; match = match.NextMatch ())
				markdownFmtStrings.Add (match.Value.ToSlackLinkString (MarkdownFmtStringType.BlockQuote));


			foreach (var markdownString in markdownFmtStrings)
				messageCopy = messageCopy.Replace (markdownString.Original, markdownString.Display);

			var attrString = new NSMutableAttributedString (messageCopy);

			attrString.AddAttributes (MessageStringAttributes, new NSRange (0, messageCopy.Length));

			foreach (var markdownString in markdownFmtStrings)
			{
				var range = new NSRange (messageCopy.IndexOf (markdownString.Display, StringComparison.Ordinal), markdownString.Display.Length);

				switch (markdownString.FmtStringType)
				{
					case MarkdownFmtStringType.Unknown:

						break;
					case MarkdownFmtStringType.Link:

						if (markdownString.Link != null)
							attrString.AddAttribute (UIStringAttributeKey.Link, markdownString.Link, range);

						attrString.AddAttributes (LinkStringAttributes, range);

						break;
					case MarkdownFmtStringType.Bold:

						attrString.AddAttributes (BoldStringAttributes, range);

						break;
					case MarkdownFmtStringType.Italic:

						attrString.AddAttributes (ItalicStringAttributes, range);

						break;
					case MarkdownFmtStringType.Strikethrough:

						attrString.AddAttributes (StrikeStringAttributes, range);

						break;
					case MarkdownFmtStringType.UnorderedList:

						//attrString.AddAttributes (LinkStringAttributes, range);

						break;
					case MarkdownFmtStringType.OrderedList:

						//attrString.AddAttributes (LinkStringAttributes, range);

						break;
					case MarkdownFmtStringType.Pre:

						attrString.AddAttributes (PreStringAttributes, range);

						break;
					case MarkdownFmtStringType.BlockQuote:

						//attrString.AddAttributes (MessageStringAttributes, range);

						break;
					case MarkdownFmtStringType.Image:

						if (markdownString.Link != null)
							attrString.AddAttribute (UIStringAttributeKey.Link, markdownString.Link, range);

						attrString.AddAttributes (LinkStringAttributes, range);

						break;
				}
			}

			return attrString;
		}


		public static MarkdownFmtString ToSlackLinkString (this string orig, MarkdownFmtStringType fmtStringType)
		{
			var markdownString = new MarkdownFmtString
			{
				Original = orig,
				FmtStringType = fmtStringType
			};

			switch (fmtStringType)
			{
				case MarkdownFmtStringType.Unknown:

					break;
				case MarkdownFmtStringType.Link:

					var linkStringArr = orig.Trim (linkTrim).Split (linkSplit, StringSplitOptions.RemoveEmptyEntries);

					markdownString.Display = linkStringArr [0];

					markdownString.Link = (linkStringArr.Length > 1) ? NSUrl.FromString (linkStringArr [1]) : null;

					break;
				case MarkdownFmtStringType.Bold:

					markdownString.Display = orig.Trim (boldTrim);

					break;
				case MarkdownFmtStringType.Italic:

					markdownString.Display = orig.Trim (italicTrim);

					break;
				case MarkdownFmtStringType.Strikethrough:

					markdownString.Display = orig.Trim (strikeTrim);

					break;
				case MarkdownFmtStringType.UnorderedList:

					markdownString.Display = orig.Replace (ulReplace.Item1, ulReplace.Item2);

					break;
				case MarkdownFmtStringType.OrderedList:

					markdownString.Display = orig;

					break;
				case MarkdownFmtStringType.Pre:

					markdownString.Display = orig.Trim (preTrim);

					break;
				case MarkdownFmtStringType.BlockQuote:

					markdownString.Display = orig;

					break;
				case MarkdownFmtStringType.Image:

					var imageStringArr = orig.Trim (imageTrim).Split (linkSplit, StringSplitOptions.RemoveEmptyEntries);

					markdownString.Display = imageStringArr [0];

					markdownString.Link = (imageStringArr.Length > 1) ? NSUrl.FromString (imageStringArr [1]) : null;

					break;
			}

			return markdownString;
		}


		#region UIStringAttributes

		public static UIStringAttributes MessageStringAttributes = new UIStringAttributes
		{
			Font = messageFont,
			ParagraphStyle = messageParagraphStyle,
			ForegroundColor = Colors.MessageColor
		};

		public static UIStringAttributes LinkStringAttributes = new UIStringAttributes
		{
			Font = messageFont,
			UnderlineStyle = NSUnderlineStyle.None,
			ParagraphStyle = messageParagraphStyle,
			ForegroundColor = Colors.MessageLinkColor
		};

		public static UIStringAttributes BoldStringAttributes = new UIStringAttributes
		{
			Font = messageFontBold,
			ParagraphStyle = messageParagraphStyle,
			ForegroundColor = Colors.MessageColor
		};

		public static UIStringAttributes ItalicStringAttributes = new UIStringAttributes
		{
			Font = messageFontItalic,
			ParagraphStyle = messageParagraphStyle,
			ForegroundColor = Colors.MessageColor
		};

		public static UIStringAttributes StrikeStringAttributes = new UIStringAttributes
		{
			Font = messageFont,
			StrikethroughStyle = NSUnderlineStyle.Single,
			ParagraphStyle = messageParagraphStyle,
			ForegroundColor = Colors.MessageColor
		};

		public static UIStringAttributes PreStringAttributes = new UIStringAttributes
		{
			Font = messageFontPre,
			ParagraphStyle = messageParagraphStyle,
			ForegroundColor = Colors.MessageColor,
			BackgroundColor = UIColor.LightGray
		};

		public static UIStringAttributes H1StringAttributes = new UIStringAttributes
		{
			Font = messageFont,
			ParagraphStyle = messageParagraphStyle,
			ForegroundColor = Colors.MessageColor
		};

		public static UIStringAttributes H2StringAttributes = new UIStringAttributes
		{
			Font = messageFont,
			ParagraphStyle = messageParagraphStyle,
			ForegroundColor = Colors.MessageColor
		};

		public static UIStringAttributes H3StringAttributes = new UIStringAttributes
		{
			Font = messageFont,
			ParagraphStyle = messageParagraphStyle,
			ForegroundColor = Colors.MessageColor
		};

		public static UIStringAttributes H4StringAttributes = new UIStringAttributes
		{
			Font = messageFont,
			ParagraphStyle = messageParagraphStyle,
			ForegroundColor = Colors.MessageColor
		};

		public static UIStringAttributes H5StringAttributes = new UIStringAttributes
		{
			Font = messageFont,
			ParagraphStyle = messageParagraphStyle,
			ForegroundColor = Colors.MessageColor
		};

		#endregion

	}
}

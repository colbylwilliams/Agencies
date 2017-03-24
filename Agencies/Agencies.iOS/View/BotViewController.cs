using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

using SlackHQ;

using Square.SocketRocket;

using Agencies.Bot;
using Agencies.Domain;


namespace Agencies.iOS
{
	public interface IAutoCompleteResult
	{
		string id { get; }
		string name { get; }
	}


	public partial class BotViewController : SlackTextViewController
	{
		UIWindow pipWindow;
		List<IAutoCompleteResult> searchResult = new List<IAutoCompleteResult> ();

		List<Message> Messages => BotClient.Shared.Messages;


		[Export ("initWithCoder:")]
		public BotViewController (NSCoder coder) : base (coder)
		{
			commonInit ();
		}


		public BotViewController (IntPtr handle) : base (handle)
		{
			commonInit ();
		}


		void commonInit ()
		{
			NSNotificationCenter.DefaultCenter.AddObserver (TableView, new Selector ("reloadData"), UIApplication.ContentSizeCategoryChangedNotification, null);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new Selector ("textInputbarDidMove:"), SlackTextInputbar.DidMoveNotification, null);
		}


		[Export ("tableViewStyleForCoder:")]
		static UITableViewStyle GetTableViewStyleForCoder (NSCoder decoder) => UITableViewStyle.Plain;


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

			TableView.RegisterClassForCellReuse (typeof (MessageBodyCell), MessageBodyCell.ReuseId);
			TableView.RegisterClassForCellReuse (typeof (MessageHeadCell), MessageHeadCell.ReuseId);

			AutoCompletionView.RegisterClassForCellReuse (typeof (MessageHeadCell), MessageHeadCell.AutoCompleteReuseId);

			//RegisterPrefixesForAutoCompletion (new [] { @"@", @"#", @":", @"+:", @"/" });

			//TextView.RegisterMarkdownFormattingSymbol (@"*", @"Bold");
			//TextView.RegisterMarkdownFormattingSymbol (@"_", @"Italics");
			//TextView.RegisterMarkdownFormattingSymbol (@"~", @"Strike");
			//TextView.RegisterMarkdownFormattingSymbol (@"`", @"Code");
			//TextView.RegisterMarkdownFormattingSymbol (@"```", @"Preformatted");
			//TextView.RegisterMarkdownFormattingSymbol (@">", @"Quote");
		}


		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			connectAllTheseEvents ();
		}


		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (!BotClient.Shared.Initialized)
			{
				Task.Run (async () => await BotClient.Shared.ConnectSocketAsync ());
			}
		}


		public override void ViewWillDisappear (bool animated)
		{
			disconnectAllTheseEvents ();

			base.ViewWillDisappear (animated);
		}


		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

			TableView.ContentInset = new UIEdgeInsets (0, 0, 0, 0);
		}


		#region Connect & Disonnect Events


		void connectAllTheseEvents ()
		{
			BotClient.Shared.ReadyStateChanged += handleBotClientReadyStateChanged;
			BotClient.Shared.MessagesCollectionChanged += handleBotClientMessagesCollectionChanged;
		}

		void disconnectAllTheseEvents ()
		{
			BotClient.Shared.ReadyStateChanged -= handleBotClientReadyStateChanged;
			BotClient.Shared.MessagesCollectionChanged -= handleBotClientMessagesCollectionChanged;
		}


		#endregion


		#region Action Methods


		void hideOrShowTextInputBar (object s, EventArgs e)
		{
			//var hide = TextInputbarHidden;

			SetTextInputbarHidden (!TextInputbarHidden, true);
		}


		void togglePIPWindow (object s, EventArgs e)
		{
			if (pipWindow == null)
			{
				showPIPWindow ();
			}
			else
			{
				hidePIPWindow ();
			}
		}


		void showPIPWindow ()
		{
			var frame = new CGRect (View.Frame.Width - 60, 0, 50, 50);
			frame.Y = TextInputbar.Frame.GetMinY () - 60;

			pipWindow = new UIWindow (frame)
			{
				BackgroundColor = UIColor.Black,
				Hidden = false,
				Alpha = 0
			};

			pipWindow.Layer.CornerRadius = 10;
			pipWindow.Layer.MasksToBounds = true;

			UIApplication.SharedApplication.KeyWindow.AddSubview (pipWindow);

			UIView.Animate (0.25, () => pipWindow.Alpha = 1.0f);
		}


		void hidePIPWindow ()
		{
			UIView.AnimateNotify (0.3f, () => pipWindow.Alpha = 0, delegate (bool _)
			{
				pipWindow.Hidden = true;
				pipWindow = null;
			});
		}


		[Export ("textInputbarDidMove:")]
		void textInputbarDidMove (NSNotification note)
		{
			if (pipWindow == null) return;

			CGRect frame = pipWindow.Frame;

			var origin = note.UserInfo.ObjectForKey (new NSString (@"origin")) as NSValue;

			frame.Y = origin.CGPointValue.Y - 60;

			pipWindow.Frame = frame;
		}

		#endregion


		#region Overriden Methods (Slack)


		public override bool IgnoreTextInputbarAdjustment => base.IgnoreTextInputbarAdjustment;

		public override bool ForceTextInputbarAdjustmentForResponder (UIResponder responder) => base.ForceTextInputbarAdjustmentForResponder (responder);

		public override void DidChangeKeyboardStatus (KeyboardStatus status) => base.DidChangeKeyboardStatus (status);

		public override void TextWillUpdate () => base.TextWillUpdate ();


		long timeStampCache;

		const long delayTicks = TimeSpan.TicksPerSecond * 3;

		public override void TextDidUpdate (bool animated)
		{
			var utcNowTicks = DateTime.UtcNow.Ticks;

			if (timeStampCache == 0 || utcNowTicks - timeStampCache > delayTicks)
			{
				timeStampCache = utcNowTicks;

				Task.Run (async () => await BotClient.Shared.SendUserTyping ());
			}

			base.TextDidUpdate (animated);
		}


		public override void DidPressLeftButton (NSObject sender)
		{
			Log.Debug ($"DidPressLeftButton");
			base.DidPressLeftButton (sender);
		}


		public override void DidPressRightButton (NSObject sender)
		{
			Log.Debug ($"DidPressRightButton : TextView.Text = {TextView.Text}");

			// Notifies the view controller when the right button's action has been triggered, manually or by using the keyboard return key.

			addNewMessage ();

			base.DidPressRightButton (sender);
		}


		void addNewMessage (bool send = true)
		{
			Log.Debug ($"addNewMessage {send}");

			// This little trick validates any pending auto-correction or auto-spelling just after hitting the 'Send' button
			TextView.RefreshFirstResponder ();

			var indexPath = NSIndexPath.FromRowSection (0, 0); //NSIndexPath.FromRowSection (row, section);
			var rowAnimation = Inverted ? UITableViewRowAnimation.Bottom : UITableViewRowAnimation.Top;
			var scrollPosition = Inverted ? UITableViewScrollPosition.Bottom : UITableViewScrollPosition.Top;

			//var sections = TableView.NumberOfSections ();

			TableView.BeginUpdates ();

			if ((send && BotClient.Shared.SendMessage (TextView.Text)) || !send)
			{
				TableView.InsertRows (new [] { NSIndexPath.FromRowSection (0, 0) }, rowAnimation);
			}

			TableView.EndUpdates ();

			var message = Messages.FirstOrDefault ();

			if (message != null)
			{
				TypingIndicatorView.RemoveUsername (message.Activity.From.Name);

				TableView.ScrollToRow (indexPath, scrollPosition, true);
			}

			// Fixes the cell from blinking (because of the transform, when using translucent cells)
			// See https://github.com/slackhq/SlackTextViewController/issues/94#issuecomment-69929927
			// TableView.ReloadRows (new [] { indexPath }, UITableViewRowAnimation.Automatic);
		}



		public override void DidPressArrowKey (UIKeyCommand sender) => base.DidPressArrowKey (sender);

		public override string KeyForTextCaching => NSBundle.MainBundle.BundleIdentifier;

		public override void DidPasteMediaContent (NSDictionary userInfo) => base.DidPasteMediaContent (userInfo);

		public override void WillRequestUndo () => base.WillRequestUndo ();

		public override void DidCommitTextEditing (NSObject sender) => base.DidCommitTextEditing (sender);

		public override void DidCancelTextEditing (NSObject sender) => base.DidCancelTextEditing (sender);

		public override bool CanPressRightButton => base.CanPressRightButton;

		public override void ShowAutoCompletionView (bool show) => base.ShowAutoCompletionView (show);

		public override bool AutoCompleting => base.AutoCompleting;

		//public override void DidChangeAutoCompletionPrefix (string prefix, string word)
		//{
		//	var wordIsEmpty = string.IsNullOrEmpty (word);

		//	IEnumerable<IAutoCompleteResult> array = null;

		//	searchResult = null;

		//	if (prefix.Equals (atStr))
		//	{
		//		array = wordIsEmpty ? BotClient.Shared.Users : BotClient.Shared.Users.Where (u => u.name.StartsWithIgnoreCase (word));
		//	}
		//	else if (prefix.Equals (hashStr) && !wordIsEmpty)
		//	{
		//		array = BotClient.Shared.Channels.Where (c => c.name.StartsWithIgnoreCase (word));
		//	}
		//	else if (prefix.Equals (colonStr) || prefix.Equals (plusColonStr) && !wordIsEmpty && word.Length > 1)
		//	{
		//		array = BotClient.Shared.Emojis.Where (e => e.name.StartsWithIgnoreCase (word));
		//	}
		//	else if (prefix.Equals (slashStr) && FoundPrefixRange.Location == 0)
		//	{
		//		array = wordIsEmpty ? BotClient.Shared.Commands : BotClient.Shared.Commands.Where (c => c.name.StartsWithIgnoreCase (word));
		//	}

		//	// array?.Sort ();

		//	searchResult = array?.OrderBy (i => i.name).ToList ();

		//	var show = searchResult?.Count > 0;

		//	ShowAutoCompletionView (show);
		//}


		public override nfloat HeightForAutoCompletionView {
			get {
				var cellHeight = AutoCompletionView.Delegate.GetHeightForRow (AutoCompletionView, NSIndexPath.FromRowSection (0, 0));

				return cellHeight * searchResult?.Count ?? 0;//  base.HeightForAutoCompletionView;
			}
		}

		#endregion


		#region UITableViewSource Methods

		[Export ("numberOfSectionsInTableView:")]
		public nint NumberOfSections (UITableView tableView) => 1;
		//=> tableView.Equals (TableView) ? MessageDataSource.SectionCount () : 1;


		public override nint RowsInSection (UITableView tableView, nint section)
			=> tableView.Equals (TableView) ? BotClient.Shared.Messages?.Count ?? 0 : (searchResult?.Count ?? 0);


		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			 => tableView.Equals (TableView) ? GetMessageCell (indexPath) : GetAutoCompleteCell (indexPath);


		MessageBodyCell GetMessageCell (NSIndexPath indexPath)
		{
			var message = Messages [indexPath.Row];

			if (message.Head)
			{
				var cell = TableView.DequeueReusableCell (MessageHeadCell.ReuseId, indexPath) as MessageHeadCell;

				cell.SetMessage (message);

				cell.IndexPath = indexPath;

				// Cells must inherit the table view's transform
				// This is very important, since the main table view may be inverted
				cell.Transform = TableView.Transform;

				return cell;
			}
			else
			{
				var cell = TableView.DequeueReusableCell (MessageBodyCell.ReuseId, indexPath) as MessageBodyCell;

				cell.SetMessage (message.AttributedText);

				cell.IndexPath = indexPath;

				cell.UsedForMessage = true;

				// Cells must inherit the table view's transform
				// This is very important, since the main table view may be inverted
				cell.Transform = TableView.Transform;

				return cell;
			}
		}


		MessageBodyCell GetAutoCompleteCell (NSIndexPath indexPath)
		{
			//Log.Debug ($"GetAutoCompleteCell = [{indexPath}]");

			var cell = AutoCompletionView.DequeueReusableCell (MessageHeadCell.AutoCompleteReuseId, indexPath) as MessageHeadCell;
			cell.IndexPath = indexPath;

			var text = searchResult [indexPath.Row].name;

			//if (FoundPrefix.Equals (hashStr))
			//{
			//	text = $"# {text}";
			//}
			//else if (FoundPrefix.Equals (colonStr) || FoundPrefix.Equals (plusColonStr))
			//{
			//	text = $":{text}:";
			//}

			cell.TitleLabel.Text = text;
			cell.SelectionStyle = UITableViewCellSelectionStyle.Default;

			return cell;
		}


		[Export ("tableView:heightForRowAtIndexPath:")]
		public nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			=> tableView.Equals (AutoCompletionView) ? MessageHeadCell.AutoCompleteHeight : getMessageHeight (indexPath, tableView.Frame.Width);


		nfloat getMessageHeight (NSIndexPath indexPath, nfloat width)
		{
			var row = indexPath.Row;

			var message = Messages [row];

			nfloat height = message.CellHeight;

			if (height > 0)
			{
				return height;
			}

			message.Head = row == Messages.Count - 1 || (row + 1 < Messages.Count) && (Messages [row + 1].Activity.From.Name != message.Activity.From.Name);

			width -= 25;

			if (string.IsNullOrEmpty (message?.Activity.Text)) return 0;

			var bodyBounds = message.AttributedText.GetBoundingRect (new CGSize (width, nfloat.MaxValue), NSStringDrawingOptions.UsesLineFragmentOrigin, null);

			height = bodyBounds.Height;

			height += 20.0f;

			var index = message.Activity.Text.IndexOf ("\r\n", StringComparison.Ordinal);

			while (index >= 0)
			{
				height += 20.0f;

				Log.Debug ("adding room");

				if (index < message.Activity.Text.Length - 1)
				{
					index = message.Activity.Text.IndexOf ("\r\n", index + 1, StringComparison.Ordinal);
				}
				else
				{
					index = -1;
				}
			}

			if (message.Head) height += 24.0f;

			message.CellHeight = height;

			return height;
		}



		[Export ("tableView:heightForFooterInSection:")]
		public nfloat GetHeightForFooter (UITableView tableView, nint section) => tableView.Equals (TableView) ? 60.0f : 0.0f;


		[Export ("tableView:willDisplayFooterView:forSection:")]
		public void WillDisplayFooterView (UITableView tableView, UIView footerView, nint section)
		{
			footerView.Transform = TableView.Transform;

			var footer = footerView as UITableViewHeaderFooterView;

			if (footer != null)
			{
				footer.ContentView.BackgroundColor = UIColor.White;
				footer.TextLabel.TextColor = UIColor.Gray;
				footer.TextLabel.TextAlignment = UITextAlignment.Center;
			}
		}


		//[Export ("tableView:titleForFooterInSection:")]
		//public string TitleForFooter (UITableView tableView, nint section)
		//	=> tableView.Equals (TableView) ? BotClient.Shared.Messages?.ElementAt ((int)section).Key.ToString ("MMM dd, yyyy") : null;


		//[Export ("tableView:didSelectRowAtIndexPath:")]
		//public void RowSelected (UITableView tableView, NSIndexPath indexPath)
		//{
		//	if (tableView.Equals (AutoCompletionView))
		//	{
		//		var item = searchResult [indexPath.Row].name;

		//		if ((FoundPrefix.Equals (atStr) && FoundPrefixRange.Location == 0) || (FoundPrefix.Equals (colonStr) || FoundPrefix.Equals (plusColonStr)))
		//		{
		//			item += colonStr;
		//		}

		//		item = $"{item} ";

		//		AcceptAutoCompletion (item, true);
		//	}
		//}

		#endregion


		#region UITextViewDelegate Methods

		[Export ("textViewShouldBeginEditing:")]
		public bool ShouldBeginEditing (UITextView textView) => true;


		[Export ("textViewShouldEndEditing:")]
		public bool ShouldEndEditing (UITextView textView) => true;


		public override bool ShouldChangeText (UITextView textView, NSRange range, string text) => base.ShouldChangeText (textView, range, text);

		public override bool ShouldOfferFormattingForSymbol (SlackTextView textView, string symbol) => base.ShouldOfferFormattingForSymbol (textView, symbol);

		public override bool ShouldInsertSuffixForFormattingWithSymbol (SlackTextView textView, string symbol, NSRange prefixRange) => base.ShouldInsertSuffixForFormattingWithSymbol (textView, symbol, prefixRange);


		#endregion


		void handleBotClientMessagesCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			Log.Debug ($"{e.Action}");

			BeginInvokeOnMainThread (() =>
			{
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						addNewMessage (false);
						break;
					case NotifyCollectionChangedAction.Remove:
						TableView.DeleteRows (new [] { NSIndexPath.FromIndex ((nuint)e.OldStartingIndex) }, UITableViewRowAnimation.None);
						break;
					case NotifyCollectionChangedAction.Replace:
						TableView.ReloadData ();
						break;
					case NotifyCollectionChangedAction.Reset:
						TableView.ReloadData ();
						TableView.SlackScrollToTop (false);
						break;
				}
			});
		}


		void handleBotClientReadyStateChanged (object sender, ReadyStateChangedEventArgs e)
		{
			Log.Debug ($"{e.ReadyState}");

			switch (e.ReadyState)
			{
				case ReadyState.Open:
					//BotClient.Shared.SendMessage ("Hello World");
					break;
			}
		}
	}
}

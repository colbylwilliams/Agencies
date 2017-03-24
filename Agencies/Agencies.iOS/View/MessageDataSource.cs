//using System;
//using System.Collections.Generic;
//using System.Linq;

//using CoreGraphics;
//using Foundation;

////using Nova.Slack.Events;
////using Nova.Utilities;
////using ServiceStack;
//using System.Threading.Tasks;


//using UIKit;
////using Nova.iOS;
//using Agencies.Bot;


//namespace Agencies.iOS
//{
//	public class RowIndex
//	{
//		public int Section { get; set; }

//		public int Item { get; set; }

//		public bool Date { get; set; }

//		public RowIndex (int section, int item, bool date)
//		{
//			Section = section;
//			Item = item;
//			Date = date;
//		}

//		public override string ToString ()
//		{
//			return string.Format ("[ RowIndex: Row={0} Item={1} Date={2}", Section.ToString ().PadRight (4), Item.ToString ().PadRight (4), Date.ToString ().PadRight (6));
//		}
//	}


//	public class MessageDataSource
//	{
//		BotClient botClient => BotClient.Shared;

//		//static LruHttpImageCache<string, UIImage> AvatarImageCache = LruHttpImageCache<string, UIImage>.Instance (LruImageCaches.Avatars, 30);

//		public Dictionary<decimal, NSAttributedString> AttributedStringCache = new Dictionary<decimal, NSAttributedString> ();

//		List<RowIndex> AdjustedIndexes { get; set; }

//#if __IOS__

//		public nint RowCount (nint section) => botClient?.Messages?.ElementAtOrDefault ((int)section).Value?.Count ?? 0;


//		public SeMessage GetMessage (NSIndexPath indexPath) => GetMessage (indexPath.Row, indexPath.Section);


//		public nfloat GetMessageHeight (NSIndexPath indexPath, nfloat width) => GetMessageHeight (GetMessage (indexPath.Row, indexPath.Section), width);


//		public nint SectionCount () => botClient?.Messages?.Count ?? 1;


//		public SeMessage GetMessage (int row, int section)
//		{
//			var sectionGroup = botClient.Messages.ElementAt (section).Value;

//			var message = sectionGroup.ElementAt (row).Value;

//			message.head = row == sectionGroup.Count - 1 || (row + 1 < sectionGroup.Count) && (sectionGroup.ElementAt (row + 1).Value.user != message.user);

//			if (!AttributedStringCache.ContainsKey (message.ts))
//				AttributedStringCache [message.ts] = message.text.GetMessageAttributedString ();

//			return message;
//		}


//		public void SetMessageHeadCellData (MessageHeadCell messageHeadCell, SeMessage message)
//		{
//			if (messageHeadCell == null)
//			{
//				Console.WriteLine ("Cell is null");
//			}

//			if (!message.head) return;

//			var cell = messageHeadCell as MessageHeadCell;

//			var key = cell?.SetMessage (message, AttributedStringCache [message.ts]) ?? 0;

//			var imageSize = 24 * UIScreen.MainScreen.Scale;

//			// Console.WriteLine (imageSize);

//			var avatarUrl = message.Owner?.profile?.image ((int)imageSize);

//			if (string.IsNullOrEmpty (avatarUrl))
//			{
//				cell?.SetAvatar (key, null);
//				return;
//			}

//			var avatarTask = AvatarImageCache [avatarUrl];

//			if (avatarTask == null)
//			{
//				avatarTask = getAvatarImage (avatarUrl);
//				AvatarImageCache [avatarUrl] = avatarTask;
//			}


//			if (avatarTask.IsCompleted)
//			{
//				cell?.SetAvatar (key, avatarTask.Result);
//				return;
//			}

//			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;

//			Task.Run (async () =>
//			{

//				var avatar = await avatarTask;

//				cell.BeginInvokeOnMainThread (() =>
//				{
//					Console.WriteLine ($"avatar == null : {avatar == null}  ::  cell?.SetAvatar : {cell?.SetAvatar (key, avatar)}");
//					//cell?.SetAvatar (key, avatar);
//					UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
//				});
//			});
//		}


//		async Task<UIImage> getAvatarImage (string avatarUrl)
//		{
//			var data = await botClient.GetAvatarData (avatarUrl);

//			return data?.GetImageFromBytes ();
//		}


//		// Dictionary<string, Task<UIImage>> avatarTasks = new Dictionary<string, Task<UIImage>> ();


//		public nfloat GetMessageHeight (SeMessage message, nfloat width)
//		{
//			if (message.height > 0) return message.height;

//			width -= 25;

//			// Console.WriteLine ($"GetMessageHeight ::: ts = {message.ts}   head = {message.head}   message.text = {message.text}");

//			if (string.IsNullOrEmpty (message?.text) || message.ts == 0) return 0;

//			var bodyBounds = AttributedStringCache [message.ts].BoundingRectWithSize (new CGSize (width, nfloat.MaxValue), NSStringDrawingOptions.UsesLineFragmentOrigin);

//			var height = bodyBounds.Height;

//			height += 20.0f;

//			if (message.head) height += 24.0f;

//			Console.WriteLine ($"GetMessageHeight :::  height = {height}  ts = {message.ts}   head = {message.head}   message.text = {message.text}");

//			message.height = (float)height;

//			return height;
//		}
//	}
//}
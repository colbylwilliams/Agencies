using System;
using Microsoft.Bot.Connector.DirectLine;

#if __IOS__
using Foundation;

using Agencies.iOS;
#endif

namespace Agencies.Domain
{
	public class Message : IComparable<Message>, IEquatable<Message>
	{
#if __IOS__
		public nfloat CellHeight { get; set; }
#elif __ANDROID__
		public float CellHeight { get; set; }
#endif

#if __IOS__
		NSAttributedString _attributedText;

		public NSAttributedString AttributedText => _attributedText ?? (_attributedText = Activity?.Text?.GetMessageAttributedString ());
#endif

		public bool Head { get; set; }

		public Activity Activity { get; private set; }

		// TODO: cache this and reset when activity is updated
		public DateTime? LocalTimeStamp => Activity.Timestamp?.ToLocalTime () ?? Activity.LocalTimestamp?.LocalDateTime;


		public Message (Activity activity)
		{
			Activity = activity;
		}


		public int CompareTo (Message other)
		{
			if (!string.IsNullOrEmpty (Activity?.Id) && !string.IsNullOrEmpty (other?.Activity?.Id))
			{
				return Activity.Id.CompareTo (other.Activity.Id);
			}

			return string.IsNullOrEmpty (Activity?.Id) ? 1 : -1;

			//var hasTimestamp = Activity?.Timestamp.HasValue ?? false;

			//var otherHasTimestamp = other?.Activity?.Timestamp.HasValue ?? false;

			//if (hasTimestamp && otherHasTimestamp)
			//{
			//	return Activity.Timestamp.Value.CompareTo (other.Activity.Timestamp.Value);
			//}

			//if (!hasTimestamp && !hasTimestamp)
			//{
			//	return 0;
			//}

			//return hasTimestamp ? 1 : -1;
		}


		public bool Equals (Message other)
		{
			// HACK: This is nasty, but I want to be able to compare the message based on timestamp
			//       However, even if I set the timestamp, the server re-sets it so they tend to be
			//       a couple seconds off.  The server also sets the Id, so I don't have that when
			//       originally creating the Activity.
			if (string.IsNullOrEmpty (Activity?.Id) || string.IsNullOrEmpty (other?.Activity?.Id))
			{
				if ((Activity?.LocalTimestamp.HasValue ?? false) && (other?.Activity?.LocalTimestamp.HasValue ?? false))
				{
					return Activity.LocalTimestamp.Value.Equals (Activity.LocalTimestamp.Value);
				}

				return ((Activity?.Text?.Equals (other?.Activity?.Text) ?? false) &&
						(Activity?.Timestamp?.Date.Equals (other?.Activity?.Timestamp?.Date) ?? false) &&
						(Activity?.Timestamp?.Hour.Equals (other?.Activity?.Timestamp?.Hour) ?? false) &&
						(Activity?.Timestamp?.Minute.Equals (other?.Activity?.Timestamp?.Minute) ?? false));
			}

			return Activity.Id.Equals (other.Activity.Id);
		}


		public void Update (Activity activity) => Activity = activity;
	}
}

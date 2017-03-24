using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.DirectLine;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Specialized;
using SettingsStudio;

#if __IOS__
using Square.SocketRocket;
using System.Collections.Generic;
using Foundation;
using Agencies.Domain;
#endif

namespace Agencies.Bot
{
	public class ReadyStateChangedEventArgs : EventArgs
	{
		public ReadyState ReadyState { get; set; }

		public ReadyStateChangedEventArgs (ReadyState readyState)
		{
			ReadyState = readyState;
		}
	}

	public class BotClient
	{
#if DEBUG
		string userName = "User";
		string userId = "default-user";
#endif
		//static string botId = "DigitalAgencies";

		static BotClient _shared;
		public static BotClient Shared => _shared ?? (_shared = new BotClient ());

		static DirectLineClient _directLineClient;
		static DirectLineClient directLineClient => _directLineClient ?? (_directLineClient = new DirectLineClient (Keys.Bot.DirectLineSecret));

		public WebSocket webSocket { get; set; }

		Conversation conversation;

		public bool Initialized => webSocket != null && webSocket.ReadyState == ReadyState.Open && conversation != null;

		public List<Message> Messages { get; set; } = new List<Message> ();


		BotClient ()
		{
		}

		public void Start ()
		{
		}

		public event EventHandler<ReadyStateChangedEventArgs> ReadyStateChanged;
		public event NotifyCollectionChangedEventHandler MessagesCollectionChanged;


		public async Task<bool> ConnectSocketAsync ()
		{
			//Settings.ConversationId = string.Empty;

			try
			{
				if (webSocket == null || webSocket.ReadyState == ReadyState.Closed)
				{
					if (string.IsNullOrEmpty (Settings.ConversationId))
					{
						conversation = await directLineClient.Conversations.StartConversationAsync ();

						if (!string.IsNullOrEmpty (conversation?.ConversationId))
						{
							Settings.ConversationId = conversation.ConversationId;
						}
					}
					else
					{
						conversation = await directLineClient.Conversations.ReconnectToConversationAsync (Settings.ConversationId);

						var activitySet = await directLineClient.Conversations.GetActivitiesAsync (conversation.ConversationId);

						handleNewActvitySet (activitySet, false);

						Messages.Sort ((x, y) => y.CompareTo (x));

						//foreach (var message in Messages)
						//	Log.Debug ($"Adding New Message: {message.Activity?.Id} : {message.Activity.Timestamp?.ToString ("O")} : {message.Activity.LocalTimestamp?.DateTime.ToString ("O")} : {message.Activity.Text}");

						MessagesCollectionChanged?.Invoke (this, new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Reset));
					}


					var url = conversation.StreamUrl;

					webSocket = new WebSocket (new NSUrl (url));

					webSocket.ReceivedMessage += handleWebSocketReceivedMessage;

					webSocket.ReceivedPong += (_, e) =>
					{
						Log.Info ($"[Socket Received Pong] {Environment.NewLine}");
					};

					webSocket.WebSocketClosed += (_, e) =>
					{
						Log.Info ($"[Socket Disconnected] Reason: {e.Reason}  Code: {e.Code}");
						ReadyStateChanged?.Invoke (this, new ReadyStateChangedEventArgs (webSocket.ReadyState));
					};

					webSocket.WebSocketFailed += (_, e) =>
					{
						Log.Info ($"[Socket Failed to Connect] Error: {e.Error?.LocalizedDescription}  Code: {e.Error?.Code}");
						ReadyStateChanged?.Invoke (this, new ReadyStateChangedEventArgs (webSocket.ReadyState));
					};

					webSocket.WebSocketOpened += (_, e) =>
					{
						Log.Info ($"[Socket Connected] {url}");
						ReadyStateChanged?.Invoke (this, new ReadyStateChangedEventArgs (webSocket.ReadyState));
					};

					Log.Info ($"[Socket Connecting...] {url}");

					webSocket.Open ();
				}
				else if (webSocket.ReadyState == ReadyState.Open)
				{
					Log.Info ($"[Socket Connecting...]");

					conversation = null;

					webSocket.Close ();
				}

				ReadyStateChanged?.Invoke (this, new ReadyStateChangedEventArgs (webSocket.ReadyState));

				return conversation != null;
			}
			catch (Exception ex)
			{
				Log.Error (ex.Message);
				return false;
			}
		}



		void handleWebSocketReceivedMessage (object sender, WebSocketReceivedMessageEventArgs e)
		{
			var message = e.Message.ToString ();

			//Log.Info ($"[Socket Message Received] {message}");

			var activitySet = JsonConvert.DeserializeObject<ActivitySet> (message);

			handleNewActvitySet (activitySet);
		}


		void handleNewActvitySet (ActivitySet activitySet, bool changedEvents = true)
		{
			var watermark = activitySet?.Watermark;

			var activities = activitySet?.Activities;

			if (activities != null)
			{
				foreach (var activity in activities)
				{
					switch (activity.Type)
					{
						case ActivityTypes.Message:

							var newMessage = new Message (activity);

							var message = Messages.FirstOrDefault (m => m.Equals (newMessage));

							if (message != null)
							{
								//Log.Debug ($"Updating Existing Message: {message.Activity.Id} : {activity.Timestamp?.ToString ("O")} : {activity.LocalTimestamp?.DateTime.ToString ("O")} : {activity.Text}");

								message.Update (activity);

								if (changedEvents)
								{
									Messages.Sort ((x, y) => y.CompareTo (x));

									var index = Messages.IndexOf (message);

									MessagesCollectionChanged?.Invoke (this, new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Replace, message, message, index));
								}
							}
							else
							{
								//Log.Debug ($"Adding New Message: {activity.Id} : {activity.Timestamp?.ToString ("O")} : {activity.LocalTimestamp?.DateTime.ToString ("O")} : {activity.Text}");

								Messages.Insert (0, newMessage);

								if (changedEvents)
								{
									//Messages.Sort ((x, y) => y.CompareTo (x));

									MessagesCollectionChanged?.Invoke (this, new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Add, message));
								}
							}

							break;
						case ActivityTypes.ContactRelationUpdate:
							break;
						case ActivityTypes.ConversationUpdate:
							break;
						case ActivityTypes.Typing:
							break;
						case ActivityTypes.Ping:
							break;
						case ActivityTypes.EndOfConversation:
							break;
						case ActivityTypes.Trigger:
							break;
					}
				}
			}
		}


		public async Task<bool> SendUserTyping ()
		{
			if (conversation == null)
			{
				throw new ArgumentNullException (nameof (conversation), "cannot be null to send message");
			}

			if (!Initialized) return false;


			Log.Debug ("Sending User Typing");

			var message = new Activity
			{
				From = new ChannelAccount (userId, userName),
				Type = ActivityTypes.Typing
			};


			/*var response = */
			await directLineClient.Conversations.PostActivityAsync (conversation.ConversationId, message);

			//Log.Debug ($"Message Sent - Response ID: {response?.Id}");

			return true;
		}


		public bool SendPing ()
		{
			if (!Initialized) return false;

			webSocket.SendPing ();

			return true;
		}


		public bool SendMessage (string text)
		{
			if (conversation == null)
			{
				throw new ArgumentNullException (nameof (conversation), "cannot be null to send message");
			}

			if (!Initialized) return false;


			Log.Debug ($"Sending Message: {text}");

			var activity = new Activity
			{
				From = new ChannelAccount (userId, userName),
				Text = text,
				Type = ActivityTypes.Message,
				LocalTimestamp = DateTimeOffset.Now,
				Timestamp = DateTime.UtcNow
			};

			var message = new Message (activity);

			//Log.Debug ($"Adding New Message: {activity.Timestamp?.ToString ("O")} : {activity.LocalTimestamp?.DateTime.ToString ("O")} : {activity.Text}");

			Messages.Insert (0, message);

			//Messages.Sort ((x, y) => y.CompareTo (x));

			Task.Run (async () =>
			{
				/*var response = */
				await directLineClient.Conversations.PostActivityAsync (conversation.ConversationId, activity);

				//Log.Debug ($"Message Sent - Response ID: {response?.Id}");
			});

			return true;
		}
	}
}

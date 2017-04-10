﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Bot.Connector.DirectLine;

using Newtonsoft.Json;

using SettingsStudio;

#if __IOS__
using Foundation;
using Square.SocketRocket;
#endif

namespace NomadCode.BotFramework
{
    public class ReadyStateChangedEventArgs : EventArgs
    {
        public ReadyState ReadyState { get; set; }

        public ReadyStateChangedEventArgs (ReadyState readyState) => ReadyState = readyState;
    }


    public partial class BotClient
    {
        static BotClient _shared;
        public static BotClient Shared => _shared ?? (_shared = new BotClient ());

        static DirectLineClient _directLineClient;
        DirectLineClient directLineClient => _directLineClient ?? (!string.IsNullOrEmpty (ConversationCredentials.Token) ? _directLineClient = new DirectLineClient (ConversationCredentials.Token) : throw new Exception ("must set initial client token"));


        Conversation conversation;

        #region Current User

        public static void ResetCurrentUser ()
        {
            CurrentUserId = string.Empty;
            CurrentUserName = string.Empty;
            CurrentUserEmail = string.Empty;
        }

        public static string CurrentUserId
        {
            get => Settings.CurrentUserId;
            set => Settings.CurrentUserId = value ?? string.Empty;
        }

        public static string CurrentUserName
        {
            get => Settings.CurrentUserName;
            set => Settings.CurrentUserName = value ?? string.Empty;
        }

        public static string CurrentUserEmail
        {
            get => Settings.CurrentUserEmail;
            set => Settings.CurrentUserEmail = value ?? string.Empty;
        }

        ChannelAccount currentUser => new ChannelAccount (CurrentUserId, CurrentUserName);

        public bool HasValidCurrentUser => !(string.IsNullOrWhiteSpace (CurrentUserId) || string.IsNullOrWhiteSpace (CurrentUserName));

        #endregion


        public WebSocket webSocket { get; set; }

        public bool Initialized => webSocket?.ReadyState == ReadyState.Open && HasValidCurrentUser && conversation != null;


        public List<Message> Messages { get; set; } = new List<Message> ();


        BotClient () { }


        public void Start () { }

        public void Reset ()
        {
            ResetCurrentUser ();
            webSocket = null;
            _directLineClient = null;
            conversation = null;
            removeItemFromKeychain ("bot");
            _conversationCredentials = (null, null);
        }


        public event EventHandler<Activity> UserTypingMessageReceived;
        public event EventHandler<ReadyStateChangedEventArgs> ReadyStateChanged;
        public event NotifyCollectionChangedEventHandler MessagesCollectionChanged;


        public void SaveConversationToken (string token, string conversationId = null)
        {
            if (!string.IsNullOrEmpty (token))
            {
                // todo: support more than one conversation
                removeItemFromKeychain ("bot");

                if (saveItemToKeychain ("bot", conversationId ?? "initialToken", token))
                {
                    _conversationCredentials = (null, null);
                }
            }
        }


        (string ConversationId, string Token) _conversationCredentials;

        (string ConversationId, string Token) ConversationCredentials
        {
            get
            {
                if (string.IsNullOrEmpty (_conversationCredentials.ConversationId) || string.IsNullOrEmpty (_conversationCredentials.Token))
                {
                    var token = getItemFromKeychain ("bot");

                    if (string.IsNullOrEmpty (token.Account) || string.IsNullOrEmpty (token.PrivateKey))
                    {
                        return (null, null);
                    }

                    _conversationCredentials = token;
                }

                return _conversationCredentials;
            }
        }


        public bool HasToken => !(string.IsNullOrEmpty (ConversationCredentials.ConversationId) || string.IsNullOrEmpty (ConversationCredentials.Token));


        public async Task<bool> ConnectSocketAsync ()
        {
            try
            {
                if (webSocket == null || webSocket.ReadyState == ReadyState.Closed)
                {
                    if (string.IsNullOrEmpty (ConversationCredentials.ConversationId))
                    {
                        throw new Exception ("must set initial client token");
                    }

                    if (Settings.ResetConversation || (HasToken && ConversationCredentials.ConversationId == "initialToken"))
                    {
                        Log.Debug ($"Starting new conversation...");

                        conversation = await directLineClient.Conversations.StartConversationAsync ();

                        if (!string.IsNullOrEmpty (conversation?.ConversationId))
                        {
                            //SaveConversationToken (conversation.Token, conversation.ConversationId);
                            SaveConversationToken (ConversationCredentials.Token, conversation.ConversationId);
                        }
                    }
                    else
                    {
                        Log.Debug ($"Reconnect to conversation {ConversationCredentials.ConversationId}...");

                        conversation = await directLineClient.Conversations.ReconnectToConversationAsync (ConversationCredentials.ConversationId);

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

                    webSocket.WebSocketClosed += handleWebSocketClosed;

                    webSocket.WebSocketFailed += handleWebSocketFailed;

                    webSocket.WebSocketOpened += handleWebSocketOpened;

                    webSocket.ReceivedPong += handleWebSocketReceivedPong;

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
                                //Log.Debug ($"Updating Existing Message: {activity.TextFormat} :: {activity.Text}");

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
                                //Log.Debug ($"Adding New Message: {activity.TextFormat} :: {activity.Text}");

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

                            if (activity?.From.Id != CurrentUserId)
                            {
                                UserTypingMessageReceived?.Invoke (this, activity);
                            }

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


        #region WebSocket Event Handlers

        void handleWebSocketReceivedMessage (object sender, WebSocketReceivedMessageEventArgs e)
        {
            var message = e.Message.ToString ();

            // Ignore empty messages 
            if (string.IsNullOrEmpty (message))
            {
                Log.Info ($"[Socket Message Received] Empty message, ignoring");

                return;
            }

            Log.Info ($"[Socket Message Received] \n{message}");

            var activitySet = JsonConvert.DeserializeObject<ActivitySet> (message);

            handleNewActvitySet (activitySet);
        }


        void handleWebSocketReceivedPong (object sender, WebSocketReceivedPongEventArgs e)
        {
            Log.Info ($"[Socket Received Pong] {Environment.NewLine}");
        }


        void handleWebSocketClosed (object sender, WebSocketClosedEventArgs e)
        {
            Log.Info ($"[Socket Disconnected] Reason: {e.Reason}  Code: {e.Code}");
            ReadyStateChanged?.Invoke (this, new ReadyStateChangedEventArgs (webSocket.ReadyState));
        }


        void handleWebSocketFailed (object sender, WebSocketFailedEventArgs e)
        {
            Log.Info ($"[Socket Failed to Connect] Error: {e.Error?.LocalizedDescription}  Code: {e.Error?.Code}");
            ReadyStateChanged?.Invoke (this, new ReadyStateChangedEventArgs (webSocket.ReadyState));

        }


        void handleWebSocketOpened (object sender, EventArgs e)
        {
            Log.Info ($"[Socket Connected] {conversation?.StreamUrl}");
            ReadyStateChanged?.Invoke (this, new ReadyStateChangedEventArgs (webSocket.ReadyState));
        }

        #endregion

        public bool SendMessage (string text)
        {
            var activity = new Activity
            {
                From = currentUser,
                Text = text,
                Type = ActivityTypes.Message,
                LocalTimestamp = DateTimeOffset.Now,
                Timestamp = DateTime.UtcNow
            };

            var message = new Message (activity);

            //Log.Debug ($"Adding New Message: {activity.Timestamp?.ToString ("O")} : {activity.LocalTimestamp?.DateTime.ToString ("O")} : {activity.Text}");

            var posted = postActivityAsync (activity);

            if (posted)
            {
                Messages.Insert (0, message);
            }

            return posted;
        }


        public bool SendUserTyping ()
        {
            Log.Debug ("Sending User Typing");

            var activity = new Activity
            {
                From = currentUser,
                Type = ActivityTypes.Typing
            };

            return postActivityAsync (activity, true);
        }


        bool postActivityAsync (Activity activity, bool ignoreFailure = false)
        {
            if (conversation == null)
            {
                if (ignoreFailure) return false;

                throw new ArgumentNullException (nameof (conversation), "cannot be null to send message");
            }

            if (!Initialized)
            {
                if (ignoreFailure) return false;

                throw new Exception ("client is not properly initialized");
            }

            Task.Run (async () =>
            {
                try
                {
                    await directLineClient.Conversations.PostActivityAsync (conversation.ConversationId, activity).ConfigureAwait (false);
                }
                catch (Exception ex)
                {
                    Log.Error (ex.Message);

                    if (!ignoreFailure)
                        throw;
                }
            });

            return true;
        }


        public bool SendPing ()
        {
            if (!Initialized) return false;

            webSocket.SendPing ();

            return true;
        }
    }
}

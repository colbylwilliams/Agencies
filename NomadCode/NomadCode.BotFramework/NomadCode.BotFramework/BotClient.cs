using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Bot.Connector.DirectLine;

using Newtonsoft.Json;

using SettingsStudio;

using Agencies.Shared;
using System.Net;
using Microsoft.Rest;

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
        DirectLineClient directLineClient => _directLineClient ?? (!string.IsNullOrEmpty (conversation?.Token) ? _directLineClient = new DirectLineClient (conversation.Token) : throw new Exception ("must set initial client token"));


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

        public static string ConversationId
        {
            get => Settings.ConversationId;
            set => Settings.ConversationId = value ?? string.Empty;
        }

        ChannelAccount currentUser => new ChannelAccount (CurrentUserId, CurrentUserName);

        public bool HasValidCurrentUser => !(string.IsNullOrWhiteSpace (CurrentUserId) || string.IsNullOrWhiteSpace (CurrentUserName));

        #endregion


        public WebSocket webSocket { get; set; }

        public bool Initialized => webSocket?.ReadyState == ReadyState.Open && HasValidCurrentUser && conversation != null;


        public List<Message> Messages { get; set; } = new List<Message> ();


        public event EventHandler<Activity> UserTypingMessageReceived;
        public event EventHandler<ReadyStateChangedEventArgs> ReadyStateChanged;
        public event NotifyCollectionChangedEventHandler MessagesCollectionChanged;


        bool attemptingReconnect;


        BotClient () { }


        public void Reset ()
        {
            ResetCurrentUser ();
            webSocket.Close ();
            webSocket = null;
            conversation = null;
            _directLineClient = null;
            Messages = new List<Message> ();
            MessagesCollectionChanged?.Invoke (this, new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Reset));
        }


        public async Task ConnectSocketAsync ()
        {
            try
            {
                if (webSocket == null || webSocket.ReadyState == ReadyState.Closed)
                {
                    if (Settings.ResetConversation)
                    {
                        Log.Info ("Resetting conversation...");

                        ConversationId = string.Empty;
                    }

                    if (!HasValidCurrentUser)
                    {
                        throw new InvalidOperationException ("BotClient.CurrentUserId and BotClient.CurrentUserName must have values before connecting");
                    }

                    Log.Info ("Getting conversation from server...");

                    conversation = await AgenciesClient.Shared.GetConversation (ConversationId);

                    // reset client so it'll pull new token
                    _directLineClient = null;

                    if (string.IsNullOrEmpty (conversation?.StreamUrl))
                    {
                        Log.Info ($"Starting new conversation...");

                        conversation = await directLineClient.Conversations.StartConversationAsync ();

                        if (!string.IsNullOrEmpty (conversation?.ConversationId))
                        {
                            ConversationId = conversation.ConversationId;
                        }
                    }
                    else
                    {
                        Log.Info ($"Reconnect to conversation {ConversationId}...");

                        conversation = await directLineClient.Conversations.ReconnectToConversationAsync (conversation.ConversationId);

                        var activitySet = await directLineClient.Conversations.GetActivitiesAsync (conversation.ConversationId);

                        handleNewActvitySet (activitySet, false);

                        Messages.Sort ((x, y) => y.CompareTo (x));

                        MessagesCollectionChanged?.Invoke (this, new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Reset));
                    }

                    if (!string.IsNullOrEmpty (conversation?.StreamUrl))
                    {
                        webSocket = new WebSocket (new NSUrl (conversation.StreamUrl));

                        webSocket.ReceivedMessage += handleWebSocketReceivedMessage;

                        webSocket.WebSocketClosed += handleWebSocketClosed;

                        webSocket.WebSocketFailed += handleWebSocketFailed;

                        webSocket.WebSocketOpened += handleWebSocketOpened;

                        webSocket.ReceivedPong += handleWebSocketReceivedPong;

                        Log.Info ($"[Socket Connecting...] {conversation.StreamUrl}");

                        webSocket.Open ();
                    }
                }
                else if (webSocket.ReadyState == ReadyState.Open)
                {
                    Log.Info ($"[Socket already open, refreshing token and reconnecting...]");

                    //attemptReconnect = true;

                    //webSocket.Close ();

                    //await ConnectSocketAsync ();
                }

                //ReadyStateChanged?.Invoke (this, new ReadyStateChangedEventArgs (webSocket.ReadyState));
            }
            //catch (HttpOperationException httpEx)
            //{
            //    Log.Error (httpEx.Message);

            //    if (httpEx.Response.StatusCode == HttpStatusCode.Forbidden && !attemptedReconnect)
            //    {
            //        Log.Info ("Attempting token refresh...");

            //        conversation = await AgenciesClient.Shared.GetConversation (conversation.ConversationId);

            //        attemptedReconnect = true;

            //        await ConnectSocketAsync ();
            //    }
            //}
            catch (Exception ex)
            {
                Log.Error (ex.Message);
            }
        }


        TaskCompletionSource<bool> closeSocketTcs;

        public async Task ResetWebsocketAsync ()
        {
            ReadyStateChanged?.Invoke (this, new ReadyStateChangedEventArgs (ReadyState.Closing));

            var closed = await closeWebsocketAsync ();

            Log.Debug ($"closed == {closed}");

            if (closed)
            {
                Log.Debug ("Reopening socket...\n");
                await ConnectSocketAsync ();
            }

            Task<bool> closeWebsocketAsync ()
            {
                if (!closeSocketTcs.IsNullFinishCanceledOrFaulted ())
                {
                    return closeSocketTcs.Task;
                }

                closeSocketTcs = new TaskCompletionSource<bool> ();


                webSocket.WebSocketClosed += handleWebSocketClosedAsync;


                webSocket.Close ();


                return closeSocketTcs.Task;
            }


            void handleWebSocketClosedAsync (object sender, WebSocketClosedEventArgs e)
            {
                webSocket.WebSocketClosed -= handleWebSocketClosedAsync;

                Log.Debug ($"e.Code: {e.Code}");
                Log.Debug ($"e.Reason: {e.Reason}");
                Log.Debug ($"e.WasClean: {e.WasClean}");

                if (!closeSocketTcs.TrySetResult (true))
                {
                    Log.Error ("Failed to set closeSocketTcs result");
                }
            }
        }


        void handleNewActvitySet (ActivitySet activitySet, bool changedEvents = true)
        {
            //var watermark = activitySet?.Watermark;

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

            //Log.Info ($"[Socket Message Received] \n{message}");

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

            //webSocket.ReceivedMessage -= handleWebSocketReceivedMessage;
            //webSocket.WebSocketClosed -= handleWebSocketClosed;
            //webSocket.WebSocketFailed -= handleWebSocketFailed;
            //webSocket.WebSocketOpened -= handleWebSocketOpened;
            //webSocket.ReceivedPong -= handleWebSocketReceivedPong;
            //if (attemptReconnect)
            //{
            //    attemptReconnect = false;

            //    Task.Run

            //}

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

            var posted = postActivityAsync (activity);

            if (posted)
            {
                Messages.Insert (0, message);
            }

            return posted;
        }


        public bool SendUserTyping ()
        {
            if (attemptingReconnect) return false;

            Log.Debug ("Sending User Typing");

            var activity = new Activity
            {
                From = currentUser,
                Type = ActivityTypes.Typing
            };

            return postActivityAsync (activity, true);
        }


        public void FuckupToken ()
        {
            var foo = @"uBCTlFDNvhY.dAA.MQBtAHIAOQB5AGsAVgBhAHEAawB2AEkANwBBAFQAQwB1ADMARQB5AE8AZQA.KJPywXKy0gE.YAWOEArEsMQ.U4jTITs6Y2z5at-5XnItwhqJP4mHUMIjiQ2m_2M0dGE";

            conversation.Token = foo;

            _directLineClient = null;
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

                Log.Error ("client is not properly initialized");
                return false;
                //throw new Exception ("client is not properly initialized");
            }

            Task.Run (async () =>
            {
                try
                {
                    if (attemptingReconnect)
                    {
                        Log.Debug ($"attemptingReconnect == {attemptingReconnect} - returning");
                        return;
                    }

                    await directLineClient.Conversations.PostActivityAsync (conversation.ConversationId, activity).ConfigureAwait (false);

                    attemptingReconnect = false;
                }
                catch (HttpOperationException httpEx)
                {
                    Log.Error (httpEx.Message);

                    if (httpEx.Response.StatusCode == HttpStatusCode.Forbidden && !attemptingReconnect)
                    {
                        Log.Debug ($"attemptingReconnect == {attemptingReconnect}");

                        attemptingReconnect = true;

                        await ResetWebsocketAsync ();

                        await directLineClient.Conversations.PostActivityAsync (conversation.ConversationId, activity).ConfigureAwait (false);

                        attemptingReconnect = false;
                    }

                    else throw;

                    //directLineClient
                    //attemptedReconnect = true;

                    //await ConnectSocketAsync ();
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

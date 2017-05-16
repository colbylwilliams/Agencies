using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

using Newtonsoft.Json;

namespace Agencies.Functions
{
	public static class BotMessages
	{
		static string _botDirectLineSecret;
		static string BotDirectLineSecret => _botDirectLineSecret ?? (_botDirectLineSecret = Environment.GetEnvironmentVariable ("MS_BotDirectLineSecret"));

		[FunctionName ("BotMessageHandler")]
		public static async Task<object> BotMessageHandler ([HttpTrigger (AuthorizationLevel.Anonymous, "post", Route = "bot/messages", WebHookType = "genericJson")]HttpRequestMessage req, TraceWriter log)
		{
			// Initialize the azure bot
			using (BotService.Initialize ())
			{
				// Deserialize the incoming activity
				string jsonContent = await req.Content.ReadAsStringAsync ();
				var activity = JsonConvert.DeserializeObject<Activity> (jsonContent);

				// authenticate incoming request and add activity.ServiceUrl to MicrosoftAppCredentials.TrustedHostNames
				// if request is authenticated
				if (!await BotService.Authenticator.TryAuthenticateAsync (req, new[] { activity }, CancellationToken.None))
				{
					return BotAuthenticator.GenerateUnauthorizedResponse (req);
				}

				if (activity != null)
				{

					if (activity.GetActivityType () == ActivityTypes.Message)
					{
						var typing = activity.CreateReply ();

						typing.Type = ActivityTypes.Typing;

						await activity.ClientForReply ().Conversations.ReplyToActivityAsync (typing);

						if (SimpleQnAMakerDialog.IsQuestion (activity.Text))
						{
							await Conversation.SendAsync (activity, () => new SimpleQnAMakerDialog ());
						}
						else
						{
							await Conversation.SendAsync (activity, () => new FaqDialog ());
						}
					}
					else
					{
						var reply = HandleSystemMessage (activity, log);

						if (reply != null)
						{
							await activity.ClientForReply ().Conversations.ReplyToActivityAsync (reply);
						}
					}
				}

				return req.CreateResponse (HttpStatusCode.Accepted);
			}
		}


		static Activity HandleSystemMessage (Activity activity, TraceWriter log)
		{
			switch (activity.Type)
			{
				case ActivityTypes.ConversationUpdate:
					// Handle conversation state changes, like members being added and removed
					// Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
					// Not available in all channels
					IConversationUpdateActivity update = activity;

					if (update.MembersAdded.Any ())
					{
						var reply = activity.CreateReply ();

						var newMembers = update.MembersAdded?.Where (t => t.Id != activity.Recipient.Id);

						foreach (var newMember in newMembers)
						{
							reply.Text = "Welcome";

							if (!string.IsNullOrEmpty (newMember.Name))
							{
								reply.Text += $" {newMember.Name}";
							}

							reply.Text += "!";

							return reply;
							//await activity.ClientForReply().Conversations.ReplyToActivityAsync (reply);
						}
					}
					break;
				case ActivityTypes.ContactRelationUpdate:
					// Handle add/remove from contact lists
					// Activity.From + Activity.Action represent what happened
					break;
				case ActivityTypes.DeleteUserData:
					// Implement user deletion here
					// If we handle user deletion, return a real message
					break;
				case ActivityTypes.Typing:
					// Handle knowing that the user is typing
					break;
				case ActivityTypes.Ping:

					break;
				default:
					log.Error ($"Unknown activity type ignored: {activity.GetActivityType ()}");
					break;
			}

			return null;
		}

		static ConnectorClient ClientForReply (this IActivity activity) => new ConnectorClient (new Uri (activity.ServiceUrl));
	}
}

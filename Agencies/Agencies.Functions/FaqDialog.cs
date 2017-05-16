using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;


namespace Agencies.Functions
{
	[Serializable]
	public class SimpleQnAMakerDialog : QnAMakerDialog
	{
		//Parameters to QnAMakerService are:
		//Compulsory: subscriptionKey, knowledgebaseId, 
		//Optional: defaultMessage, scoreThreshold[Range 0.0 – 1.0]
		public SimpleQnAMakerDialog () : base (new QnAMakerService (new QnAMakerAttribute (Utils.GetAppSetting ("QnASubscriptionKey"), Utils.GetAppSetting ("QnAKnowledgebaseId"), "No good match in FAQ.", 0.5)))
		{
		}
	}


	[Serializable]
	public class FaqDialog : IDialog<object>
	{
		const string HeroCard = "Hero card";
		const string ThumbnailCard = "Thumbnail card";
		const string ReceiptCard = "Receipt card";
		const string SigninCard = "Sign-in card";
		const string AnimationCard = "Animation card";
		const string VideoCard = "Video card";
		const string AudioCard = "Audio card";

		IEnumerable<string> options = new List<string> { HeroCard, ThumbnailCard, ReceiptCard, SigninCard, AnimationCard, VideoCard, AudioCard };


		public Task StartAsync (IDialogContext context)
		{
			context.Wait (MessageReceivedAsync);

			return Task.CompletedTask;
		}

		public async Task MessageReceivedAsync (IDialogContext context, IAwaitable<IMessageActivity> argument)
		{
			var message = await argument as Activity;

			//Call the QnAMaker Dialog if the message is a question.
			if (IsQuestion (message.Text))
			{
				await context.Forward (new SimpleQnAMakerDialog (), AfterQnA, message, CancellationToken.None);

				context.Wait (MessageReceivedAsync);
			}
			else if (options.Any (o => o.Equals (message.Text, StringComparison.Ordinal)))
			{
				await DisplaySelectedCard (context, message.Text);
			}
			else
			{
				PromptDialog.Choice (
					context,
					DisplaySelectedCard,
					options,
					"What card would like to test?",
					"Ooops, what you wrote is not a valid option, please try again",
					3,
					PromptStyle.PerLine);
			}
		}

		//Callback, after the QnAMaker Dialog returns a result.
		public Task AfterQnA (IDialogContext context, IAwaitable<object> argument)
		{
			context.Wait (MessageReceivedAsync);

			return Task.CompletedTask;
		}

		//Simple check if the message is a potential question.
		bool IsQuestion (string message)
		{
			//List of common question words
			List<string> questionWords = new List<string> { "who", "what", "why", "how", "when" };

			//Question word present in the message
			Regex questionPattern = new Regex (@"\b(" + string.Join ("|", questionWords.Select (Regex.Escape).ToArray ()) + @"\b)", RegexOptions.IgnoreCase);

			//Return true if a question word present, or the message ends with "?"
			return (questionPattern.IsMatch (message) || message.EndsWith ("?", StringComparison.OrdinalIgnoreCase));
		}


		public async Task DisplaySelectedCard (IDialogContext context, string selectedCard)
		{
			var message = context.MakeMessage ();

			var attachment = GetSelectedCard (selectedCard);

			message.Attachments.Add (attachment);

			await context.PostAsync (message);

			context.Wait (MessageReceivedAsync);
		}


		public async Task DisplaySelectedCard (IDialogContext context, IAwaitable<string> result)
		{
			var selectedCard = await result;

			var message = context.MakeMessage ();

			var attachment = GetSelectedCard (selectedCard);
			message.Attachments.Add (attachment);

			await context.PostAsync (message);

			context.Wait (this.MessageReceivedAsync);
		}


		static Attachment GetSelectedCard (string selectedCard)
		{
			switch (selectedCard)
			{
				case HeroCard:
					return GetHeroCard ();
				case ThumbnailCard:
					return GetThumbnailCard ();
				case ReceiptCard:
					return GetReceiptCard ();
				case SigninCard:
					return GetSigninCard ();
				case AnimationCard:
					return GetAnimationCard ();
				case VideoCard:
					return GetVideoCard ();
				case AudioCard:
					return GetAudioCard ();

				default:
					return GetHeroCard ();
			}
		}


		#region GetCards

		static Attachment GetHeroCard ()
		{
			var heroCard = new HeroCard
			{
				Title = "BotFramework Hero Card",
				Subtitle = "Your bots — wherever your users are talking",
				Text = "Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.",
				Images = new List<CardImage> { new CardImage ("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
				Buttons = new List<CardAction> { new CardAction (ActionTypes.OpenUrl, "Get Started", value: "https://docs.botframework.com/en-us/") }
			};

			return heroCard.ToAttachment ();
		}

		static Attachment GetThumbnailCard ()
		{
			var heroCard = new ThumbnailCard
			{
				Title = "BotFramework Thumbnail Card",
				Subtitle = "Your bots — wherever your users are talking",
				Text = "Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.",
				Images = new List<CardImage> { new CardImage ("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
				Buttons = new List<CardAction> { new CardAction (ActionTypes.OpenUrl, "Get Started", value: "https://docs.botframework.com/en-us/") }
			};

			return heroCard.ToAttachment ();
		}

		static Attachment GetReceiptCard ()
		{
			var receiptCard = new ReceiptCard
			{
				Title = "John Doe",
				Facts = new List<Fact> { new Fact ("Order Number", "1234"), new Fact ("Payment Method", "VISA 5555-****") },
				Items = new List<ReceiptItem>
				{
					new ReceiptItem("Data Transfer", price: "$ 38.45", quantity: "368", image: new CardImage(url: "https://github.com/amido/azure-vector-icons/raw/master/renders/traffic-manager.png")),
					new ReceiptItem("App Service", price: "$ 45.00", quantity: "720", image: new CardImage(url: "https://github.com/amido/azure-vector-icons/raw/master/renders/cloud-service.png")),
				},
				Tax = "$ 7.50",
				Total = "$ 90.95",
				Buttons = new List<CardAction>
				{
					new CardAction(
						ActionTypes.OpenUrl,
						"More information",
						"https://account.windowsazure.com/content/6.10.1.38-.8225.160809-1618/aux-pre/images/offer-icon-freetrial.png",
						"https://azure.microsoft.com/en-us/pricing/")
				}
			};

			return receiptCard.ToAttachment ();
		}

		static Attachment GetSigninCard ()
		{
			var signinCard = new SigninCard
			{
				Text = "BotFramework Sign-in Card",
				Buttons = new List<CardAction> { new CardAction (ActionTypes.Signin, "Sign-in", value: "https://login.microsoftonline.com/") }
			};

			return signinCard.ToAttachment ();
		}

		static Attachment GetAnimationCard ()
		{
			var animationCard = new AnimationCard
			{
				Title = "Microsoft Bot Framework",
				Subtitle = "Animation Card",
				Image = new ThumbnailUrl
				{
					Url = "https://docs.botframework.com/en-us/images/faq-overview/botframework_overview_july.png"
				},
				Media = new List<MediaUrl>
				{
					new MediaUrl()
					{
						Url = "http://i.giphy.com/Ki55RUbOV5njy.gif"
					}
				}
			};

			return animationCard.ToAttachment ();
		}

		static Attachment GetVideoCard ()
		{
			var videoCard = new VideoCard
			{
				Title = "Big Buck Bunny",
				Subtitle = "by the Blender Institute",
				Text = "Big Buck Bunny (code-named Peach) is a short computer-animated comedy film by the Blender Institute, part of the Blender Foundation. Like the foundation's previous film Elephants Dream, the film was made using Blender, a free software application for animation made by the same foundation. It was released as an open-source film under Creative Commons License Attribution 3.0.",
				Image = new ThumbnailUrl
				{
					Url = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c5/Big_buck_bunny_poster_big.jpg/220px-Big_buck_bunny_poster_big.jpg"
				},
				Media = new List<MediaUrl>
				{
					new MediaUrl()
					{
						Url = "http://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4"
					}
				},
				Buttons = new List<CardAction>
				{
					new CardAction()
					{
						Title = "Learn More",
						Type = ActionTypes.OpenUrl,
						Value = "https://peach.blender.org/"
					}
				}
			};

			return videoCard.ToAttachment ();
		}

		static Attachment GetAudioCard ()
		{
			var audioCard = new AudioCard
			{
				Title = "I am your father",
				Subtitle = "Star Wars: Episode V - The Empire Strikes Back",
				Text = "The Empire Strikes Back (also known as Star Wars: Episode V – The Empire Strikes Back) is a 1980 American epic space opera film directed by Irvin Kershner. Leigh Brackett and Lawrence Kasdan wrote the screenplay, with George Lucas writing the film's story and serving as executive producer. The second installment in the original Star Wars trilogy, it was produced by Gary Kurtz for Lucasfilm Ltd. and stars Mark Hamill, Harrison Ford, Carrie Fisher, Billy Dee Williams, Anthony Daniels, David Prowse, Kenny Baker, Peter Mayhew and Frank Oz.",
				Image = new ThumbnailUrl
				{
					Url = "https://upload.wikimedia.org/wikipedia/en/3/3c/SW_-_Empire_Strikes_Back.jpg"
				},
				Media = new List<MediaUrl>
				{
					new MediaUrl()
					{
						Url = "http://www.wavlist.com/movies/004/father.wav"
					}
				},
				Buttons = new List<CardAction>
				{
					new CardAction()
					{
						Title = "Read More",
						Type = ActionTypes.OpenUrl,
						Value = "https://en.wikipedia.org/wiki/The_Empire_Strikes_Back"
					}
				}
			};

			return audioCard.ToAttachment ();
		}

		#endregion
	}
}

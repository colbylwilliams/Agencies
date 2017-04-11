using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Agencies.AppService.Dialogs
{
	[Serializable]
	public class SimpleQnAMakerDialog : QnAMakerDialog
	{
		//Parameters to QnAMakerService are:
		//Compulsory: subscriptionKey, knowledgebaseId, 
		//Optional: defaultMessage, scoreThreshold[Range 0.0 – 1.0]
		public SimpleQnAMakerDialog() : base(new QnAMakerService(new QnAMakerAttribute("e9206a203f764120824fbe472155c15f", "7edc7883-308a-49a5-a2fb-3daa3480a194", "No good match in FAQ.", 0.5)))
		{
		}
	}

	/// <summary>
	/// Simple Dialog, that invokes the QnAMaker if the incoming message is a question
	/// </summary>
	[Serializable]
	public class FAQDialog : IDialog<object>
	{
		public Task StartAsync(IDialogContext context)
		{
			context.Wait(MessageReceivedAsync);

			return Task.CompletedTask;
		}

		public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
		{
			var message = await argument as Activity;

			if (message.Text == "HeroCard")
			{
				Activity replyToConversation = message.CreateReply("Should go to conversation, with a hero card");
				replyToConversation.Recipient = message.From;
				replyToConversation.Type = "message";
				replyToConversation.Attachments = new List<Attachment>();
				List<CardImage> cardImages = new List<CardImage>();
				cardImages.Add(new CardImage(url: "https://<ImageUrl1>"));
				cardImages.Add(new CardImage(url: "https://<ImageUrl2>"));
				List<CardAction> cardButtons = new List<CardAction>();
				CardAction plButton = new CardAction()
				{
					Value = "https://en.wikipedia.org/wiki/Pig_Latin",
					Type = "openUrl",
					Title = "WikiPedia Page"
				};
				cardButtons.Add(plButton);
				HeroCard plCard = new HeroCard()
				{
					Title = "I'm a hero card",
					Subtitle = "Pig Latin Wikipedia Page",
					Images = cardImages,
					Buttons = cardButtons
				};
				Attachment plAttachment = plCard.ToAttachment();
				replyToConversation.Attachments.Add(plAttachment);

				await context.PostAsync(replyToConversation);

			}
			else if (message.Text == "Thumbnail")
			{
				Activity replyToConversation = message.CreateReply("Should go to conversation, with a thumbnail card");
				replyToConversation.Recipient = message.From;
				replyToConversation.Type = "message";
				replyToConversation.Attachments = new List<Attachment>();
				List<CardImage> cardImages = new List<CardImage>();
				cardImages.Add(new CardImage(url: "https://<ImageUrl1>"));
				List<CardAction> cardButtons = new List<CardAction>();
				CardAction plButton = new CardAction()
				{
					Value = "https://en.wikipedia.org/wiki/Pig_Latin",
					Type = "openUrl",
					Title = "WikiPedia Page"
				};
				cardButtons.Add(plButton);
				ThumbnailCard plCard = new ThumbnailCard()
				{
					Title = "I'm a thumbnail card",
					Subtitle = "Pig Latin Wikipedia Page",
					Images = cardImages,
					Buttons = cardButtons
				};
				Attachment plAttachment = plCard.ToAttachment();
				replyToConversation.Attachments.Add(plAttachment);
				await context.PostAsync(replyToConversation);

			}
			else if (message.Text == "Receipt")
			{
				Activity replyToConversation = message.CreateReply("Receipt card");
				replyToConversation.Recipient = message.From;
				replyToConversation.Type = "message";
				replyToConversation.Attachments = new List<Attachment>();
				List<CardImage> cardImages = new List<CardImage>();
				cardImages.Add(new CardImage(url: "https://<ImageUrl1>"));
				List<CardAction> cardButtons = new List<CardAction>();
				CardAction plButton = new CardAction()
				{
					Value = "https://en.wikipedia.org/wiki/Pig_Latin",
					Type = "openUrl",
					Title = "WikiPedia Page"
				};
				cardButtons.Add(plButton);
				ReceiptItem lineItem1 = new ReceiptItem()
				{
					Title = "Pork Shoulder",
					Subtitle = "8 lbs",
					Text = null,
					Image = new CardImage(url: "https://<ImageUrl1>"),
					Price = "16.25",
					Quantity = "1",
					Tap = null
				};
				ReceiptItem lineItem2 = new ReceiptItem()
				{
					Title = "Bacon",
					Subtitle = "5 lbs",
					Text = null,
					Image = new CardImage(url: "https://<ImageUrl2>"),
					Price = "34.50",
					Quantity = "2",
					Tap = null
				};
				List<ReceiptItem> receiptList = new List<ReceiptItem>();
				receiptList.Add(lineItem1);
				receiptList.Add(lineItem2);
				ReceiptCard plCard = new ReceiptCard()
				{
					Title = "I'm a receipt card, isn't this bacon expensive?",
					Buttons = cardButtons,
					Items = receiptList,
					Total = "275.25",
					Tax = "27.52"
				};
				Attachment plAttachment = plCard.ToAttachment();
				replyToConversation.Attachments.Add(plAttachment);

				await context.PostAsync(replyToConversation);
			}
			else if (message.Text == "Signin")
			{
				Activity replyToConversation = message.CreateReply("Should go to conversation, sign-in card");
				replyToConversation.Recipient = message.From;
				replyToConversation.Type = "message";
				replyToConversation.Attachments = new List<Attachment>();
				List<CardAction> cardButtons = new List<CardAction>();
				CardAction plButton = new CardAction()
				{
					Value = "https://<OAuthSignInURL>",
					Type = "signin",
					Title = "Connect"
				};
				cardButtons.Add(plButton);
				SigninCard plCard = new SigninCard("You need to authorize me", new List<CardAction> { plButton });
				Attachment plAttachment = plCard.ToAttachment();
				replyToConversation.Attachments.Add(plAttachment);
				await context.PostAsync(replyToConversation);
			}
			else
			{
				//Call the QnAMaker Dialog if the message is a question.
				if (IsQuestion(message.Text))
				{
					await context.Forward(new SimpleQnAMakerDialog(), AfterQnA, message, CancellationToken.None);
				}
				else
					await context.PostAsync("This doesn't look like a question.");
			}

			context.Wait(MessageReceivedAsync);
		}

		//Callback, after the QnAMaker Dialog returns a result.
		public Task AfterQnA(IDialogContext context, IAwaitable<object> argument)
		{
			context.Wait(MessageReceivedAsync);

			return Task.CompletedTask;
		}

		//Simple check if the message is a potential question.
		private bool IsQuestion(string message)
		{
			//List of common question words
			List<string> questionWords = new List<string>() { "who", "what", "why", "how", "when" };

			//Question word present in the message
			Regex questionPattern = new Regex(@"\b(" + string.Join("|", questionWords.Select(Regex.Escape).ToArray()) + @"\b)", RegexOptions.IgnoreCase);

			//Return true if a question word present, or the message ends with "?"
			if (questionPattern.IsMatch(message) || message.EndsWith("?"))
				return true;
			else
				return false;
		}
	}
}
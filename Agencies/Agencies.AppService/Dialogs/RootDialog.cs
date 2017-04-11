using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;

namespace Agencies.AppService.Dialogs
{
	[Serializable]
	public class RootDialog : IDialog<object>
	{
		public Task StartAsync(IDialogContext context)
		{
			context.Wait(MessageReceivedAsync);

			return Task.CompletedTask;
		}

		private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
		{

			var message = await result as Activity;

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

				context.Wait(MessageReceivedAsync);
			}


			if (message.Text == "Thumbnail")
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

				context.Wait(MessageReceivedAsync);

			}


			if (message.Text == "Receipt")
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

				context.Wait(MessageReceivedAsync);
			}

			if (message.Text == "Signin")
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

				context.Wait(MessageReceivedAsync);

			}
			//var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);

			////var activity = await result as Activity;

			//// calculate something for us to return
			//int length = (activity.Text ?? string.Empty).Length;

			//// return our reply to the user
			//await context.PostAsync($"You sent {activity.Text} which was {length} characters");

			//context.Wait(MessageReceivedAsync);
		}
	}
}
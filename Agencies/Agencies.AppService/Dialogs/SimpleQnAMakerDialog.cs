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
		public async Task StartAsync(IDialogContext context)
		{
			context.Wait(MessageReceivedAsync);
		}

		public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
		{
			var message = await argument;

			//Call the QnAMaker Dialog if the message is a question.
			if (IsQuestion(message.Text))
			{
				await context.Forward(new SimpleQnAMakerDialog(), AfterQnA, message, CancellationToken.None);
			}
			else
				await context.PostAsync("This doesn't look like a question.");

			context.Wait(MessageReceivedAsync);
		}

		//Callback, after the QnAMaker Dialog returns a result.
		public async Task AfterQnA(IDialogContext context, IAwaitable<object> argument)
		{
			context.Wait(MessageReceivedAsync);
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
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HospitalChatbot.Forms;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using HospitalChatbot.Common;
using System.Configuration;

namespace HospitalChatbot.Dialogs
{
	[Serializable]
	public class SecurityDialog : IDialog<object>
	{
        private const string EmailOption = "Email";
        private const string MobileOption = "Mobile";
        public async Task StartAsync(IDialogContext context)
		{
            this.ShowOptions(context);                       
        }

        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { EmailOption, MobileOption }, "Identify yourself?", "Not a valid option", 3);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var option = await result;
                string optionSelected = option.ToString();

                switch (optionSelected)
                {
                    //case EmailOption:
                    //    context.Call(new EmailDialog(), this.ResumeAfterOptionDialog);
                    //    break;

                    //case MobileOption:
                    //    context.Call(new MobileDialog(), this.ResumeAfterOptionDialog);
                    //    break;

                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                context.Wait(this.MessageReceivedAsync);
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
        }

        public async Task EndTask(IDialogContext context, IAwaitable<object> result)
		{
			context.Done<object>(result);
		}

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            //var message = await result;
            this.ShowOptions(context);
        }


    }
}
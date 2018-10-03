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
using System.Net.Http;
using System.IO;

namespace HospitalChatbot.Dialogs
{
	[Serializable]
	public class BookAppointmentDialog : IDialog<object>
	{
        private string _otpGenerated = string.Empty;
		public async Task StartAsync(IDialogContext context)
		{
            await context.PostAsync("Have you identified your health problem? (Yes/No)");
            context.Wait(this.HealthProblemConfirmationCheck);
        }

        private async Task HealthProblemConfirmationCheck(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            if ((message.Text.ToLower().Contains("yes")) || message.Text.ToLower().Contains("y"))
            {
                context.Call<object>(new HealthProblemDialog(), this.ResumeAfterHealthProblemDialog);
            }
            else if ((message.Text.ToLower().Contains("no")) || message.Text.ToLower().Contains("n"))
            {
                
            }
            else
            {
                context.Done<object>(result);
            }
        }

        private async Task ResumeAfterHealthProblemDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                context.Call<object>(new DoctorDialog(), this.ResumeAfterDoctorDialog);
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
        }

        private async Task ResumeAfterDoctorDialog(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result;           
        }

    }
}
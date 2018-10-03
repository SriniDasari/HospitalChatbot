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
	public class AppointmentDoctorDialog : IDialog<object>
	{
        private const string BookAppointmentOption = "Book Appointment";
        private const string CancelAppointmentOption = "Cancel Appointment";
        private const string RescheduleAppointmentOption = "Reschedule Appointment";
        public async Task StartAsync(IDialogContext context)
		{
            this.ShowOptions(context);                       
        }

        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { BookAppointmentOption, CancelAppointmentOption, RescheduleAppointmentOption }, "How can i help you in this?", "Not a valid option", 3);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var option = await result;
                string optionSelected = option.ToString();

                switch (optionSelected)
                {
                    case BookAppointmentOption:
                        context.Call(new BookAppointmentDialog(), this.ResumeAfterOptionDialog);
                        break;

                    case CancelAppointmentOption:
                        context.Call(new CancelAppointmentDialog(), this.ResumeAfterOptionDialog);
                        break;

                    case RescheduleAppointmentOption:
                        context.Call(new RescheduleAppointmentDialog(), this.ResumeAfterOptionDialog);
                        break;
                        
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
               this.ShowOptions(context);
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
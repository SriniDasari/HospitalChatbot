using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace HospitalChatbot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string AppointmentDoctorOption = "Appointment for Doctor Consultation";
        private const string AppointmentHealthCheckOption = "Appointment for preventive Health checkup";
        private const string PaymentOption = "Payment OPD services/ Doctor Consultation";
        private const string OPDDepositOption = "OPD Deposit";
        private const string ReportsOption = "Online Reports";
        private const string OnlineHealthCheckFormOption = "Online healthcheckup Medical History Form";
        private const string EnquiryOption = "Enquiry";
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(StartChat);

            return Task.CompletedTask;
        }

        private async Task StartChat(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result as Activity;

            if ((message.Text.ToLower().Contains("hi")) || message.Text.ToLower().Contains("hello"))
            {
                await context.PostAsync("Welcome! May I know your name.");
                //context.Call<object>(new HomeDialog(), AfterHomeDialogIsDone);
                context.Wait(this.HomeStart);
            }
        }

        private async Task HomeStart(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result as Activity;
            await context.PostAsync($"Hello { message.Text }!");
            this.ShowOptions(context);
            //context.Wait(this.MessageReceivedInvoiceStatusCheck);
        }
        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { AppointmentDoctorOption, AppointmentHealthCheckOption, PaymentOption, OPDDepositOption, ReportsOption, OnlineHealthCheckFormOption, EnquiryOption }, "What can i assist you with?", "Not a valid option", 3);
        }
        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var option = await result;
                string optionSelected = option.ToString();

                switch (optionSelected)
                {
                    case AppointmentDoctorOption:
                        context.Call(new AppointmentDoctorDialog(), this.ResumeAfterOptionDialog);
                        break;
                    case AppointmentHealthCheckOption:
                        context.Call(new AppointmentHealthCheckDialog(), this.ResumeAfterOptionDialog);
                        break;
                    case PaymentOption:
                        context.Call(new PaymentDialog(), this.ResumeAfterOptionDialog);
                        break;
                    case OPDDepositOption:
                        context.Call(new OPDDepositDialog(), this.ResumeAfterOptionDialog);
                        break;
                    case ReportsOption:
                        context.Call(new ReportsDialog(), this.ResumeAfterOptionDialog);
                        break;
                    case OnlineHealthCheckFormOption:
                        context.Call(new OnlineHealthCheckFormDialog(), this.ResumeAfterOptionDialog);
                        break;
                    case EnquiryOption:
                        context.Call(new EnquiryDialog(), this.ResumeAfterOptionDialog);
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

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            //var message = await result;
            this.ShowOptions(context);
        }
    }
}
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
//using Twilio;
//using Twilio.Rest.Api.V2010.Account;
//using Twilio.Types;
using HospitalChatbot.Repository;
using System.Net;
using System.Text;

namespace HospitalChatbot.Dialogs
{
    [Serializable]
    public class MobileDialog : IDialog<object>
    {
        private string _otpGenerated = string.Empty;
        private string _mobileNumber = string.Empty;
        public async Task StartAsync(IDialogContext context)
        {
            var identityFormDialog = FormDialog.FromForm(this.BuildIdentityForm, FormOptions.PromptInStart);
            context.Call(identityFormDialog, this.ResumeDialog);
        }

        public async Task EndTask(IDialogContext context, IAwaitable<object> result)
        {
            context.Done<object>(result);
        }

        private async Task ResumeDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                context.Wait(this.MessageReceivedProcessOTPAsync);
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
        }



        private IForm<IdentityForm> BuildIdentityForm()
        {

            OnCompletionAsyncDelegate<IdentityForm> processIdentity = async (context, state) =>
            {
                _mobileNumber = state.Mobile;
                var deliveryStatus = await SendOTP(_mobileNumber);
                string status = string.Empty;
                //if (deliveryStatus.Equals(System.Net.HttpStatusCode.Accepted) || deliveryStatus.StatusCode.Equals(System.Net.HttpStatusCode.Created))
                if (!string.IsNullOrEmpty(deliveryStatus))
                {
                    status = String.Format("Enter 4 digit OTP received on your Mobile {0}", _mobileNumber);
                }
                else
                {
                    status = String.Format("System error in delivering OTP, Status Code :{0}", deliveryStatus);
                }
                //var status = "Sent back with comments 'Amount Mismatch'." + "\r\n" + "Kindly log on to Invoice Portal and revise the Invoice.'";
                await context.PostAsync($"{ status}");
            };

            return new FormBuilder<IdentityForm>()
                .Field(nameof(IdentityForm.Mobile))
                .OnCompletion(processIdentity)
                .Build();
        }

        private async Task MessageReceivedProcessOTPAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            string otpReceived = message.Text;
            string vendorName = CommonDAL.GetVendorName(_mobileNumber);
            // ***** uncomment below for validating OTP on Live *****

            if (otpReceived.Equals(this._otpGenerated))
            {
                context.PrivateConversationData.SetValue<string>("MobileNumber", _mobileNumber);
                await context.PostAsync($"Welcome { vendorName}, you are now successfully validated!");
                context.Call<object>(new SecurityDialog(), this.AfterInvoiceDialogIsDone);
            }
            else
            {
                await context.PostAsync("Please enter valid OTP.");
            }

            // ***** uncomment above for validating OTP on Live *****

            // **** dummy otp validation comment below before go live**** 

            //context.PrivateConversationData.SetValue<string>("MobileNumber", _mobileNumber);
            //await context.PostAsync($"Welcome { vendorName}, you are now successfully validated!");
            //context.Call(new InvoiceDialog(), this.AfterInvoiceDialogIsDone);

            // **** dummy otp validation comment above before go live**** 
        }

        private async Task AfterInvoiceDialogIsDone(IDialogContext context, IAwaitable<object> result)
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
        private async Task<string> SendOTP(string mobileNo)
        {

            ////***** uncomment below for live*****

            //string otp = OTP.Create();
            //this._otpGenerated = otp;
            ////return await PostMobileMessageTwilio(mobileNo, otp);
            //return await PostMobileMessage(mobileNo, otp);

            ////***** uncomment above for live*****

            //***** dummy response-- comment below for live*****
            this._otpGenerated = "5404";
            return "otpreceived";

            //***** dummy response-- comment above for live*****
        }

        //private async Task<string> PostMobileMessageTwilio(string mobileNo, string otp)
        //{
        //    var accountSid = Convert.ToString(ConfigurationManager.AppSettings["TwilioMobile_AccountSid"]);
        //    var authToken = Convert.ToString(ConfigurationManager.AppSettings["TwilioMobile_AuthToken"]);

        //    TwilioClient.Init(accountSid, authToken);

        //    var to = new PhoneNumber("+91" + mobileNo);
        //    var message = MessageResource.Create(
        //        to,
        //        from: new PhoneNumber("+12319998025"),
        //        body: string.Format("OTP for Chatbot is: {0}", otp));

        //    return message.Sid;
        //}



        private async Task<string> PostMobileMessage(string mobileNo, string otp)
        {
            string accountID = "CI00203668";
            string password = "yQb1Ooa6";
            string email = "sgdasari@gmail.com";
            // https://redoxygen.net/
            string message = string.Format("OTP for Chatbot is: {0}", otp);
            #if SKIP_OTP
                return 0;
            #else
            WebClient Client = new WebClient();
            String RequestURL, RequestData;

            RequestURL = "https://redoxygen.net/sms.dll?Action=SendSMS";

            RequestData = "AccountId=" + accountID
                + "&Email=" + HttpUtility.UrlEncode(email)
                + "&Password=" + HttpUtility.UrlEncode(password)
                + "&Recipient=" + HttpUtility.UrlEncode("+91" + mobileNo)
                + "&Message=" + HttpUtility.UrlEncode(message);

            byte[] PostData = Encoding.ASCII.GetBytes(RequestData);
            byte[] Response = Client.UploadData(RequestURL, PostData);

            String Result = Encoding.ASCII.GetString(Response);
            int ResultCode = System.Convert.ToInt32(Result.Substring(0, 4));

            return Convert.ToString(ResultCode);
#endif

        }

    }
}
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
	public class ReportsDialog : IDialog<object>
	{
        private string _otpGenerated = string.Empty;
		public async Task StartAsync(IDialogContext context)
		{
            await context.PostAsync("Sorry!!! You need to wait. Still work in progress...");
            context.Done<object>("");

        }       
       
    }
}
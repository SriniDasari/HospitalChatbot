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
using HospitalChatbot.Model;
using System.Linq;

namespace HospitalChatbot.Dialogs
{
	[Serializable]
	public class HealthProblemDialog : IDialog<object>
	{
        
		public async Task StartAsync(IDialogContext context)
		{
            List<HealthProblem> healthProblemList = FetchHealthProblems();
            //PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { "Kidney Disease", "Arthritis", "Hepatitis" }, "Please select your health problem?", "Not a valid option", 3);            
            PromptDialog.Choice(context, this.OnOptionSelected, healthProblemList.Select(h => h.HealthProblemId).ToArray(), "Please select your health problem?", "Not a valid option", 3, descriptions: healthProblemList.Select(h => h.HealthProblemName).ToArray());

        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<int> result)
        {
            try
            {
                var option = await result;
                string optionSelected = option.ToString();
                context.PrivateConversationData.SetValue<string>("HealthProblemSelected", optionSelected);
                context.Done<object>("");
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");
                context.Done<object>("");
            }
        }

        private List<HealthProblem> FetchHealthProblems()
        {
            return new List<HealthProblem>()
            {
                new HealthProblem(){ HealthProblemId = 1, HealthProblemName = "Kidney Disease" },
                new HealthProblem(){ HealthProblemId = 2, HealthProblemName = "Arthritis" },
                new HealthProblem(){ HealthProblemId = 3, HealthProblemName = "Hepatitis" }
            };

        }

    }
}
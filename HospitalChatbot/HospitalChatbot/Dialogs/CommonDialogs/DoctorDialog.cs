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
	public class DoctorDialog : IDialog<object>
	{
        string _healthProblemSelectedId = string.Empty;
        public async Task StartAsync(IDialogContext context)
		{
            
            context.PrivateConversationData.TryGetValue<string>("HealthProblemSelected", out _healthProblemSelectedId);

            List<Doctor> doctorList = FetchDoctorDetails();
            //PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { "Kidney Disease", "Arthritis", "Hepatitis" }, "Please select your health problem?", "Not a valid option", 3);            
            PromptDialog.Choice(context, this.OnOptionSelected, doctorList.Select(h => h.DoctorId).ToArray(), "Please select your health problem?", "Not a valid option", 3, descriptions: doctorList.Select(h => h.DoctorName).ToArray());
        }

        private List<Doctor> FetchDoctorDetails()
        {
            // Check if _healthProblemSelectedId has value then fetch the doctor details based on _healthProblemSelectedId otherwise top 5.
            return new List<Doctor>()
            {
                new Doctor(){ DoctorId = 1, DoctorName = "Dr Priyesh Patel", Address = "Hinduja Hospital, Chembur", Experience = "4 Yrs", Qualifications = "MBBS, DNB", SpecializationId = 1, SpokenLanguages = "English, Hindi, Marathi, Gujrathi" },
                new Doctor(){ DoctorId = 2, DoctorName = "Dr Gautam Nene", Address = "Hinduja Hospital, Bandra", Experience = "10 Yrs", Qualifications = "MBBS, MS", SpecializationId = 2, SpokenLanguages = "English, Hindi, Marathi" },
                new Doctor(){ DoctorId = 3, DoctorName = "Dr Akshay Kale", Address = "Hinduja Hospital, Tardeo", Experience = "17 Yrs", Qualifications = "MBBS, MS(Ortho)", SpecializationId = 3, SpokenLanguages = "English, Hindi, Marathi" },
            };

        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<int> result)
        {
            try
            {
                var option = await result;
                string optionSelected = option.ToString();
                context.PrivateConversationData.SetValue<string>("DoctorSelected", optionSelected);
                context.Done<object>("");
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");
                context.Done<object>("");
            }
        }
    }
}
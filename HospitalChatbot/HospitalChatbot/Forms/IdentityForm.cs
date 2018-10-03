using System;
using Microsoft.Bot.Builder.FormFlow;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalChatbot.Forms
{

	[Serializable]
	public class IdentityForm
	{
		[Prompt("Enter your first name: {||}")]
		public string FirstName;

        [Prompt("Enter your last name: {||}")]
        public string LastName;

        [Prompt("Enter your mobile number: {||}")]
        public string Mobile;       
	}
}
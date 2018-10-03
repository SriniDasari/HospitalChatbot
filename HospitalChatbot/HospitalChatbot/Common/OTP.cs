using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace HospitalChatbot.Common
{
    public class OTP
    {
        public static string Create()
        {
            string otpLength = Convert.ToString(ConfigurationManager.AppSettings["OTPLength"]);
            // declare array string to generate random string with combination of numbers
            char[] charArr = "0123456789".ToCharArray();
            string otp = string.Empty;
            Random objran = new Random();
            int noofcharacters = Convert.ToInt32(otpLength);
            for (int i = 0; i < noofcharacters; i++)
            {
                //It will not allow repetition of characters
                int pos = objran.Next(1, charArr.Length);
                if (!otp.Contains(charArr.GetValue(pos).ToString()))
                    otp += charArr.GetValue(pos);
                else
                    i--;
            }

            return otp;
        }

    }
}
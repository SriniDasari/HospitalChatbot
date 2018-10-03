using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace HospitalChatbot.Common
{
    public class DateTimeParser
    {
        public static string ConvertDateTimeToDate(DateTime? dateTimeObj)
        {
            try
            {              
                
                var datetime = Convert.ToDateTime(dateTimeObj);
                var date = datetime.Date;
                return Convert.ToString(date.ToString("d"));
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}
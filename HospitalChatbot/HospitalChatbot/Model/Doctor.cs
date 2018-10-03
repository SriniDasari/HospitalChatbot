using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalChatbot.Model
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int SpecializationId { get; set; }
        public string Qualifications { get; set; }
        public string Experience { get; set; }
        public string Address { get; set; }
        public string SpokenLanguages { get; set; }
    }

}
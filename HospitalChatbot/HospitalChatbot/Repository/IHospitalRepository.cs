using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HospitalChatbot.Model;
namespace HospitalChatbot.Repository
{
    public interface IHospitalRepository
    {
        List<HealthProblem> GetHealthProblems();
        List<Doctor> GetDoctors(string Id);
        
    }
}
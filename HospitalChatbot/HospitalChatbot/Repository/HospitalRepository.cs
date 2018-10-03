using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using HospitalChatbot.Model;
using Dapper;
namespace HospitalChatbot.Repository
{    
    public class HospitalRepository : IHospitalRepository
    {
        private IDbConnection _db = new SqlConnection(ConfigurationManager.ConnectionStrings["ChatbotDBConnection"].ConnectionString);

        public List<Doctor> GetDoctors(string id)
        {
            return this._db.Query<Doctor>("SELECT TOP 5 * FROM Doctor WHERE SpecializationId = @id ORDER BY Experience DESC", new { @id = id }).ToList();
        }

        public List<HealthProblem> GetHealthProblems()
        {
            throw new NotImplementedException();
        }        

    }
}
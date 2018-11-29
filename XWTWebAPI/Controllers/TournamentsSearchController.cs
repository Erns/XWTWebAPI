using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XWTWebAPI.Classes;
using XWTWebAPI.Models;

namespace XWTWebAPI.Controllers
{
    public class TournamentsSearchController : ApiController
    {

        // GET api/values/5 (READ)
        public string Get(int userid)
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                return "Validation fail";
            }

            //No luck with FromBody within a GET
            return "";

        }

        // POST api/values (CREATE)
        public string Post([FromBody]string value)
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                return "Validation fail";
            }

            List<TournamentMain> returnTournaments = new List<TournamentMain>();

            try
            {
                TournamentMain result = JsonConvert.DeserializeObject<TournamentMain>(JsonConvert.DeserializeObject(value).ToString());

                //using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
                //{
                //    sqlConn.Open();

                //    using (SqlCommand sqlCmd = new SqlCommand("dbo.spTournamentsSearch_GET", sqlConn))
                //    {
                //        sqlConn.Open();
                //        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                //        //sqlCmd.Parameters.AddWithValue("@UserAccountId", userid);
 //               @TournamentName VarChar(200) = ''
	//, @TournamentDate smalldatetime = NULL
                //        using (SqlDataReader sqlReader = sqlCmd.ExecuteReader())
                //        {
                //            while (sqlReader.Read())
                //            {
                //                returnTournaments.Add(new TournamentMain(
                //                    sqlReader.GetInt32(sqlReader.GetOrdinal("Id")),
                //sqlReader.GetString(sqlReader.GetOrdinal("Name")),
                //                    sqlReader.GetDateTime(sqlReader.GetOrdinal("StartDate")),
                //                    sqlReader.GetInt32(sqlReader.GetOrdinal("MaxPoints")),
                //                    sqlReader.GetInt32(sqlReader.GetOrdinal("RoundTimeLength")),
                //                    sqlReader.GetBoolean(sqlReader.GetOrdinal("PublicSearch"))
                //                ));
                //            }
                //        }
                //    }
                //}

                return "hit";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return JsonConvert.SerializeObject(returnTournaments);
        }

        // PUT api/values/5 (UPDATE)
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5 (DELETE)
        public void Delete(int id)
        {
        }

    }
}

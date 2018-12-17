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
    public class TournamentsRoundsTimerController : ApiController
    {
        // GET api/values (READ)
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5 (READ)
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values (CREATE)
        public string Post([FromBody]string value)
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                return "Validation fail";
            }

            try
            {
                TournamentMainRound round = JsonConvert.DeserializeObject<TournamentMainRound>(JsonConvert.DeserializeObject(value).ToString());

                using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
                {
                    sqlConn.Open();

                    using (SqlCommand sqlCmd = new SqlCommand("dbo.spTournamentRoundTimer_UPDATE", sqlConn))
                    {
                        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@RoundId", round.Id);
                        sqlCmd.Parameters.AddWithValue("@RoundTimeEnd", round.RoundTimeEnd);

                        using (SqlDataReader sqlReader = sqlCmd.ExecuteReader())
                        {
                            while (sqlReader.Read())
                            {
                                TournamentsController tourns = new TournamentsController();

                                //returnTournaments.Add(JsonConvert.DeserializeObject<TournamentMain>(JsonConvert.DeserializeObject(tourns.Get(sqlReader.GetInt32(sqlReader.GetOrdinal("UserAccountId")), sqlReader.GetInt32(sqlReader.GetOrdinal("Id")))).ToString()));
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "POST: SUCCESS";
        }

        // PUT api/values/5 (UPDATE)
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5 (DELETE)
        public string Delete(int id)
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                return "Validation fail";
            }

            try
            {

                using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
                {
                    sqlConn.Open();

                    using (SqlCommand sqlCmd = new SqlCommand("dbo.spTournamentRoundTimer_DELETE", sqlConn))
                    {
                        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@RoundId", id);
                        sqlCmd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "DELETE: SUCCESS";

        }

    }
}

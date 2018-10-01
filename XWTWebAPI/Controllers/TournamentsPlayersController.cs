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
    public class TournamentsPlayersController : ApiController
    {
        // GET api/values (READ)
        public IEnumerable<string> Get()
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                return new string[] { "Validation fail"};
            }

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5 (READ)
        public string Get(int userid, int id)
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                return "Validation fail";
            }

            return "value";
        }

        // POST api/values (CREATE)
        public string Post([FromBody]string value)
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                return "Validation fail";
            }
            return "post";
        }

        // PUT api/values/5 (UPDATE)
        public string Put(int userid, int id, [FromBody]string value)
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                return "Validation fail";
            }

            TournamentMain tournament = JsonConvert.DeserializeObject<TournamentMain>(JsonConvert.DeserializeObject(value).ToString());

            using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
            {
                
                //Delete all players currently linked to tournament

                //Insert players
                foreach (TournamentMainPlayer player in tournament.Players)
                {
                    using (SqlCommand sqlCmd = new SqlCommand("dbo.", sqlConn))
                    {
                        sqlConn.Open();
                        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@UserAccountId", userid);
                        sqlCmd.Parameters.AddWithValue("@TournamentId", tournament.Id);
                        sqlCmd.Parameters.AddWithValue("@Id", player.Id);
                        sqlCmd.Parameters.AddWithValue("@Name", player.PlayerName);

                        sqlCmd.ExecuteNonQuery();
                    }
                }
                
               
            }

            return "Put";
        }

        // DELETE api/values/5 (DELETE)
        public string Delete(int id)
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                return "Validation fail";
            }
            return "Delete";

        }

    }
}

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

            try
            {
                TournamentMain result = JsonConvert.DeserializeObject<TournamentMain>(JsonConvert.DeserializeObject(value).ToString());

                using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
                {
                    int intCount = 0;
                    sqlConn.Open();

                    if (result.Id > 0)
                    {
                        foreach (TournamentMainPlayer player in result.Players)
                        {
                            using (SqlCommand sqlCmd = new SqlCommand("dbo.spTournamentsPlayers_UPDATEINSERT", sqlConn))
                            {
                                //TODO:  Add logic to stored proc to search for an id if playerId exist for tournament to update instead of insert
                                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                                sqlCmd.Parameters.AddWithValue("@Id", player.Id);
                                sqlCmd.Parameters.AddWithValue("@TournamentId", id);
                                sqlCmd.Parameters.AddWithValue("@PlayerId", player.PlayerId);
                                sqlCmd.Parameters.AddWithValue("@OpponentIds", player.OpponentIdsBlobbed);
                                sqlCmd.Parameters.AddWithValue("@PlayerName", player.PlayerName);
                                sqlCmd.Parameters.AddWithValue("@Active", player.Active);
                                sqlCmd.Parameters.AddWithValue("@Bye", player.Bye);
                                sqlCmd.Parameters.AddWithValue("@ByeCount", player.ByeCount);
                                sqlCmd.Parameters.AddWithValue("@RoundsPlayed", player.RoundsPlayed);
                                sqlCmd.Parameters.AddWithValue("@Rank", player.Rank);
                                sqlCmd.Parameters.AddWithValue("@Score", player.Score);
                                sqlCmd.Parameters.AddWithValue("@MOV", player.MOV);
                                sqlCmd.Parameters.AddWithValue("@SOS", player.SOS);

                                sqlCmd.ExecuteNonQuery();

                                intCount++;
                            }
                        }
                    }

                    return "PUT Success:  Updated " + intCount;

                }

            }
            catch (Exception ex)
            {
                return ex.Message;
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

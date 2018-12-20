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
    public class TournamentsController : ApiController
    {
        List<TournamentMain> tournaments = new List<TournamentMain>();
        Utilities oUtl = new Utilities();

        // GET api/values
        public string Get(int userid)
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                return "Validation fail";
            }

            tournaments.Clear();
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
                {
                    using (SqlCommand sqlCmd = new SqlCommand("dbo.spTournaments_GET", sqlConn))
                    {
                        sqlConn.Open();
                        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@UserAccountId", userid);
                        using (SqlDataReader sqlReader = sqlCmd.ExecuteReader())
                        {
                            while (sqlReader.Read())
                            {
                                tournaments.Add(new TournamentMain(
                                    sqlReader.GetInt32(sqlReader.GetOrdinal("Id")),
                                    sqlReader.GetString(sqlReader.GetOrdinal("Name")),
                                    sqlReader.GetDateTime(sqlReader.GetOrdinal("StartDate")),
                                    sqlReader.GetInt32(sqlReader.GetOrdinal("MaxPoints")),
                                    sqlReader.GetInt32(sqlReader.GetOrdinal("RoundTimeLength")),
                                    sqlReader.GetBoolean(sqlReader.GetOrdinal("PublicSearch"))
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return JsonConvert.SerializeObject(tournaments);
        }

        // GET api/values/5 (READ)
        public string Get(int userid, int id)
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                return "Validation fail";
            }

            tournaments.Clear();
            try
            {
                //Grab the main tournament information
                using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
                {
                    sqlConn.Open();

                    using (SqlCommand sqlCmd = new SqlCommand("dbo.spTournaments_GET", sqlConn))
                    {
                        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@UserAccountId", userid);
                        sqlCmd.Parameters.AddWithValue("@TournamentId", id);
                        using (SqlDataReader sqlReader = sqlCmd.ExecuteReader())
                        {
                            while (sqlReader.Read())
                            {
                                tournaments.Add(new TournamentMain(
                                    sqlReader.GetInt32(sqlReader.GetOrdinal("Id")),
                                    sqlReader.GetString(sqlReader.GetOrdinal("Name")),
                                    sqlReader.GetDateTime(sqlReader.GetOrdinal("StartDate")),
                                    sqlReader.GetInt32(sqlReader.GetOrdinal("MaxPoints")),
                                    sqlReader.GetInt32(sqlReader.GetOrdinal("RoundTimeLength")),
                                    sqlReader.GetBoolean(sqlReader.GetOrdinal("PublicSearch"))
                                ));
                            }
                        }
                    }

                    oUtl.GetAllTournamentInfo(sqlConn, ref tournaments);                 
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return JsonConvert.SerializeObject(tournaments);
        }

        // POST api/values (CREATE)
        public string Post(int userid, [FromBody]string value)
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                return "Validation fail";
            }

            return "";
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
                    sqlConn.Open();

                    using (SqlCommand sqlCmd = new SqlCommand("dbo.spTournaments_UPDATEINSERT", sqlConn))
                    {
                        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@Id", id);
                        sqlCmd.Parameters.AddWithValue("@UserAccountId", userid);
                        sqlCmd.Parameters.AddWithValue("@Name", result.Name);
                        sqlCmd.Parameters.AddWithValue("@StartDate", result.StartDate);
                        sqlCmd.Parameters.AddWithValue("@MaxPoints", result.MaxPoints);
                        sqlCmd.Parameters.AddWithValue("@RoundTimeLength", result.RoundTimeLength);
                        sqlCmd.Parameters.AddWithValue("@PublicSearch", result.PublicSearch);

                        sqlCmd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "PUT Success";
        }

        // DELETE api/values/5 (DELETE)
        public void Delete(int userid, int id)
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                //return "Validation fail";
            }

        }

    }
}

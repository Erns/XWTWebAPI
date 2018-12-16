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

            List<TournamentMain> returnTournaments = new List<TournamentMain>();

            //Get all the tournaments the user is registered for
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
                {
                    sqlConn.Open();

                    using (SqlCommand sqlCmd = new SqlCommand("dbo.spTournamentsSearch_GET_REGISTERED", sqlConn))
                    {
                        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@UserAccountId", userid);

                        using (SqlDataReader sqlReader = sqlCmd.ExecuteReader())
                        {
                            while (sqlReader.Read())
                            {
                                returnTournaments.Add(new TournamentMain(
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


            return JsonConvert.SerializeObject(returnTournaments);

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
                //Searching and getting any public tournaments we find that match the criteria

                TournamentMain result = JsonConvert.DeserializeObject<TournamentMain>(JsonConvert.DeserializeObject(value).ToString());

                using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
                {
                    sqlConn.Open();

                    using (SqlCommand sqlCmd = new SqlCommand("dbo.spTournamentsSearch_GET", sqlConn))
                    {
                        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@TournamentName", result.Name);
                        sqlCmd.Parameters.AddWithValue("@TournamentDate", result.StartDate);

                        using (SqlDataReader sqlReader = sqlCmd.ExecuteReader())
                        {
                            while (sqlReader.Read())
                            {
                                //returnTournaments.Add(new TournamentMain(
                                //    sqlReader.GetInt32(sqlReader.GetOrdinal("Id")),
                                //    sqlReader.GetString(sqlReader.GetOrdinal("Name")),
                                //    sqlReader.GetDateTime(sqlReader.GetOrdinal("StartDate")),
                                //    sqlReader.GetInt32(sqlReader.GetOrdinal("MaxPoints")),
                                //    sqlReader.GetInt32(sqlReader.GetOrdinal("RoundTimeLength")),
                                //    sqlReader.GetBoolean(sqlReader.GetOrdinal("PublicSearch"))
                                //));

                                TournamentsController tourns = new TournamentsController();

                                returnTournaments.Add(JsonConvert.DeserializeObject<TournamentMain>(JsonConvert.DeserializeObject(tourns.Get(sqlReader.GetInt32(sqlReader.GetOrdinal("UserAccountId")), sqlReader.GetInt32(sqlReader.GetOrdinal("Id")))).ToString()));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return JsonConvert.SerializeObject(returnTournaments);
        }

        // PUT api/values/5 (UPDATE)
        public string Put(int userid, int id)
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

                    using (SqlCommand sqlCmd = new SqlCommand("dbo.spTournamentsSearch_REGISTER", sqlConn))
                    {
                        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@UserAccountId", userid);
                        sqlCmd.Parameters.AddWithValue("@TournamentId", id);
                        sqlCmd.ExecuteNonQuery();

                        return "PUT: Success";
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // DELETE api/values/5 (DELETE)
        public void Delete(int id)
        {
        }

    }
}

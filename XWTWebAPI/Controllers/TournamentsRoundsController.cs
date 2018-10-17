using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XWTWebAPI.Classes;
using XWTWebAPI.Models;

namespace XWTWebAPI.Controllers
{
    public class TournamentsRoundsController : ApiController
    {
        // GET api/values (READ)
        public IEnumerable<string> Get()
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                return new string[] { "Validation fail" };
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
        public string Post(int userid, int id, [FromBody]string value)
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

                    //Create new round
                    using (SqlCommand sqlCmd = new SqlCommand("dbo.spTournamentsRounds_UPDATEINSERT", sqlConn))
                    {
                        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@TournamentId", round.TournamentId);
                        sqlCmd.Parameters.AddWithValue("@Number", round.Number);
                        sqlCmd.Parameters.AddWithValue("@Swiss", round.Swiss);
                        //sqlCmd.Parameters.AddWithValue("@RoundTimeEnd", round.RoundTimeEnd); Creating round, so no need to pass this at all

                        SqlParameter outputParameter = new SqlParameter("@Id", SqlDbType.Int);
                        outputParameter.Value = id;
                        outputParameter.Direction = ParameterDirection.InputOutput;

                        sqlCmd.Parameters.Add(outputParameter);

                        //Grab the new ID, if applicable
                        using (SqlDataReader sqlRdr = sqlCmd.ExecuteReader())
                        {
                            id = Convert.ToInt32(outputParameter.Value);
                        }
                    }


                    //Create/update table data sent in
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Id", typeof(int));
                    dt.Columns.Add("RoundId", typeof(int));
                    dt.Columns.Add("Number", typeof(int));
                    dt.Columns.Add("TableName", typeof(string));
                    dt.Columns.Add("ScoreTied", typeof(bool));
                    dt.Columns.Add("Bye", typeof(bool));
                    dt.Columns.Add("Player1Id", typeof(int));
                    dt.Columns.Add("Player1Name", typeof(string));
                    dt.Columns.Add("Player1Score", typeof(int));
                    dt.Columns.Add("Player1Winner", typeof(bool));
                    dt.Columns.Add("Player2Id", typeof(int));
                    dt.Columns.Add("Player2Name", typeof(string));
                    dt.Columns.Add("Player2Score", typeof(int));
                    dt.Columns.Add("Player2Winner", typeof(bool));

                    foreach (TournamentMainRoundTable table in round.Tables)
                    {
                        dt.Rows.Add(table.Id, table.RoundId, table.Number, table.TableName, table.ScoreTied, table.Bye
                            , table.Player1Id, table.Player1Name, table.Player1Score, table.Player1Winner
                            , table.Player2Id, table.Player2Name, table.Player2Score, table.Player2Winner);
                    }

                    using (SqlCommand sqlCmd = new SqlCommand("dbo.spTournamentsRoundsTable_UPDATEINSERT_DT", sqlConn))
                    {
                        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@RoundId", id);
                        sqlCmd.Parameters.Add("@TableDataTable", SqlDbType.Structured).Value = dt;
                        sqlCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "POST: Success";

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
                //TournamentMainRound round = JsonConvert.DeserializeObject<TournamentMainRound>(JsonConvert.DeserializeObject(value).ToString());
                TournamentMainRoundTable table = JsonConvert.DeserializeObject<TournamentMainRoundTable>(JsonConvert.DeserializeObject(value).ToString());

                using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
                {
                    sqlConn.Open();

                    ////Create new round
                    //using (SqlCommand sqlCmd = new SqlCommand("dbo.spTournamentsRounds_UPDATEINSERT", sqlConn))
                    //{
                    //    sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    //    sqlCmd.Parameters.AddWithValue("@TournamentId", round.TournamentId);
                    //    sqlCmd.Parameters.AddWithValue("@Number", round.Number);
                    //    sqlCmd.Parameters.AddWithValue("@Swiss", round.Swiss);
                    //    sqlCmd.Parameters.AddWithValue("@RoundTimeEnd", round.RoundTimeEnd);

                    //    SqlParameter outputParameter = new SqlParameter("@Id", SqlDbType.Int);
                    //    outputParameter.Value = id;
                    //    outputParameter.Direction = ParameterDirection.InputOutput;

                    //    sqlCmd.Parameters.Add(outputParameter);

                    //    //Grab the new ID, if applicable
                    //    using (SqlDataReader sqlRdr = sqlCmd.ExecuteReader())
                    //    {
                    //        id = Convert.ToInt32(outputParameter.Value);
                    //    }
                    //}


                    //Create/update table data sent in
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Id", typeof(int));
                    dt.Columns.Add("RoundId", typeof(int));
                    dt.Columns.Add("Number", typeof(int));
                    dt.Columns.Add("TableName", typeof(string));
                    dt.Columns.Add("ScoreTied", typeof(bool));
                    dt.Columns.Add("Bye", typeof(bool));
                    dt.Columns.Add("Player1Id", typeof(int));
                    dt.Columns.Add("Player1Name", typeof(string));
                    dt.Columns.Add("Player1Score", typeof(int));
                    dt.Columns.Add("Player1Winner", typeof(bool));
                    dt.Columns.Add("Player2Id", typeof(int));
                    dt.Columns.Add("Player2Name", typeof(string));
                    dt.Columns.Add("Player2Score", typeof(int));
                    dt.Columns.Add("Player2Winner", typeof(bool));

                    //foreach (TournamentMainRoundTable table in round.Tables)
                    //{
                        dt.Rows.Add(table.Id, table.RoundId, table.Number, table.TableName, table.ScoreTied, table.Bye
                            , table.Player1Id, table.Player1Name, table.Player1Score, table.Player1Winner
                            , table.Player2Id, table.Player2Name, table.Player2Score, table.Player2Winner);
                    //}

                    using (SqlCommand sqlCmd = new SqlCommand("dbo.spTournamentsRoundsTable_UPDATEINSERT_DT", sqlConn))
                    {
                        sqlConn.Open();
                        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@RoundId", id);
                        sqlCmd.Parameters.Add("@TableDataTable", SqlDbType.Structured).Value = dt;
                        sqlCmd.ExecuteNonQuery();

                    }
                } 
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "PUT: Success";
        }

        // DELETE api/values/5 (DELETE)
        public string Delete(int userid, int id)
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                return "Validation fail";
            }

            using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
            {
                sqlConn.Open();

                //Create new round
                using (SqlCommand sqlCmd = new SqlCommand("dbo.spTournamentsRounds_DELETE", sqlConn))
                {
                    sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@RoundId", id);
                    sqlCmd.ExecuteNonQuery();
                }
            }

            return "DELETE: Succcess";
        }

    }
}

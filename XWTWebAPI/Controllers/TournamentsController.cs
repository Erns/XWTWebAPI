﻿using Newtonsoft.Json;
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
                                    sqlReader.GetInt32(sqlReader.GetOrdinal("Id"))
                                    , sqlReader.GetString(sqlReader.GetOrdinal("Name"))
                                    , sqlReader.GetDateTime(sqlReader.GetOrdinal("StartDate"))
                                    , sqlReader.GetInt32(sqlReader.GetOrdinal("MaxPoints"))
                                    , sqlReader.GetInt32(sqlReader.GetOrdinal("RoundTimeLength"))
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
                                    sqlReader.GetInt32(sqlReader.GetOrdinal("Id"))
                                    , sqlReader.GetString(sqlReader.GetOrdinal("Name"))
                                    , sqlReader.GetDateTime(sqlReader.GetOrdinal("StartDate"))
                                    , sqlReader.GetInt32(sqlReader.GetOrdinal("MaxPoints"))
                                    , sqlReader.GetInt32(sqlReader.GetOrdinal("RoundTimeLength"))
                                ));
                            }
                        }
                    }

                    //Grab all the players associated with this tournament
                    //TODO ensure we're grabbing all players, not just active.  Getting error going into a tournament with 8 players in first rounds, and 4 in the current 
                    foreach (TournamentMain newTournament in tournaments)
                    {
                        using (SqlCommand sqlCmd = new SqlCommand("dbo.spTournamentsPlayers_GET", sqlConn))
                        {
                            sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                            sqlCmd.Parameters.AddWithValue("@TournamentId", newTournament.Id);
                            using (SqlDataReader sqlReader = sqlCmd.ExecuteReader())
                            {
                                while (sqlReader.Read())
                                {
                                    TournamentMainPlayer player = new TournamentMainPlayer
                                    {
                                        Id = sqlReader.GetInt32(sqlReader.GetOrdinal("Id")),
                                        TournamentId = sqlReader.GetInt32(sqlReader.GetOrdinal("TournamentId")),
                                        PlayerId = sqlReader.GetInt32(sqlReader.GetOrdinal("PlayerId")),
                                        OpponentIdsBlobbed = sqlReader.GetString(sqlReader.GetOrdinal("OpponentIds")),
                                        PlayerName = sqlReader.GetString(sqlReader.GetOrdinal("PlayerName")),
                                        Active = sqlReader.GetBoolean(sqlReader.GetOrdinal("Active")),
                                        Bye = sqlReader.GetBoolean(sqlReader.GetOrdinal("Bye")),
                                        ByeCount = sqlReader.GetInt32(sqlReader.GetOrdinal("ByeCount")),
                                        RoundsPlayed = sqlReader.GetInt32(sqlReader.GetOrdinal("RoundsPlayed")),
                                        Rank = sqlReader.GetInt32(sqlReader.GetOrdinal("Rank")),
                                        Score = sqlReader.GetInt32(sqlReader.GetOrdinal("Score")),
                                        MOV = sqlReader.GetInt32(sqlReader.GetOrdinal("MOV")),
                                        SOS = sqlReader.GetDecimal(sqlReader.GetOrdinal("SOS"))
                                    };

                                    newTournament.Players.Add(player);
                                }
                            }
                        }

                        using (SqlCommand sqlCmd = new SqlCommand("dbo.spTournamentsRounds_GET", sqlConn))
                        {
                            sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                            sqlCmd.Parameters.AddWithValue("@TournamentId", newTournament.Id);
                            using (SqlDataReader sqlReader = sqlCmd.ExecuteReader())
                            {
                                while (sqlReader.Read())
                                {
                                    TournamentMainRound round = new TournamentMainRound
                                    {
                                        Id = sqlReader.GetInt32(sqlReader.GetOrdinal("Id")),
                                        TournamentId = sqlReader.GetInt32(sqlReader.GetOrdinal("TournamentId")),
                                        Number = sqlReader.GetInt32(sqlReader.GetOrdinal("Number")),
                                        Swiss = sqlReader.GetBoolean(sqlReader.GetOrdinal("Swiss"))
                                    };

                                    if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("RoundTimeEnd")))
                                    {
                                        round.RoundTimeEnd = sqlReader.GetDateTime(sqlReader.GetOrdinal("RoundTimeEnd"));
                                    }

                                    newTournament.Rounds.Add(round);
                                }
                            }
                        }

                        foreach (TournamentMainRound round in newTournament.Rounds)
                        {

                            using (SqlCommand sqlCmd = new SqlCommand("dbo.spTournamentsRoundsTables_GET", sqlConn))
                            {
                                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                                sqlCmd.Parameters.AddWithValue("@RoundId", round.Id);
                                using (SqlDataReader sqlReader = sqlCmd.ExecuteReader())
                                {
                                    while (sqlReader.Read())
                                    {
                                        TournamentMainRoundTable table = new TournamentMainRoundTable
                                        {
                                            Id = sqlReader.GetInt32(sqlReader.GetOrdinal("Id")),
                                            RoundId = sqlReader.GetInt32(sqlReader.GetOrdinal("RoundId")),
                                            Number = sqlReader.GetInt32(sqlReader.GetOrdinal("Number")),
                                            TableName = sqlReader.GetString(sqlReader.GetOrdinal("TableName")),
                                            ScoreTied = sqlReader.GetBoolean(sqlReader.GetOrdinal("ScoreTied")),
                                            Bye = sqlReader.GetBoolean(sqlReader.GetOrdinal("Bye")),
                                            Player1Id = sqlReader.GetInt32(sqlReader.GetOrdinal("Player1Id")),
                                            Player1Name = sqlReader.GetString(sqlReader.GetOrdinal("Player1Name")),
                                            Player1Winner = sqlReader.GetBoolean(sqlReader.GetOrdinal("Player1Winner")),
                                            Player1Score = sqlReader.GetInt32(sqlReader.GetOrdinal("Player1Score")),
                                            Player2Id = sqlReader.GetInt32(sqlReader.GetOrdinal("Player2Id")),
                                            Player2Name = sqlReader.GetString(sqlReader.GetOrdinal("Player2Name")),
                                            Player2Winner = sqlReader.GetBoolean(sqlReader.GetOrdinal("Player2Winner")),
                                            Player2Score = sqlReader.GetInt32(sqlReader.GetOrdinal("Player2Score"))
                                        };
                                        round.Tables.Add(table);
                                    }
                                }
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

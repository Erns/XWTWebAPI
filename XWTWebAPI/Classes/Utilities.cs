using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using XWTWebAPI.Models;

namespace XWTWebAPI.Classes
{
    public class Utilities
    {
        public static bool IsValidated(System.Net.Http.Headers.HttpRequestHeaders headers)
        {

            bool blnReturn = false;

            try
            {
            
                //Verify that the username and API key passed through request are valid
                if (headers.Authorization != null)
                {
                    string authenticationToken = headers.Authorization.Parameter;
                    string decodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken));
                    string[] usernamePasswordArray = decodedAuthenticationToken.Split(':');
                    string username = usernamePasswordArray[0];
                    string password = usernamePasswordArray[1];

                    using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
                    {
                        using (SqlCommand sqlCmd = new SqlCommand("dbo.spUserAccounts_VALIDATE", sqlConn))
                        {
                            sqlConn.Open();
                            sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                            sqlCmd.Parameters.AddWithValue("@UserName", username);
                            sqlCmd.Parameters.AddWithValue("@PasswordAPI", password);

                            SqlParameter outputParameter = new SqlParameter("@Id", SqlDbType.Int);
                            outputParameter.Direction = ParameterDirection.Output;

                            sqlCmd.Parameters.Add(outputParameter);

                            int id = 0;
                            using (SqlDataReader sqlRdr = sqlCmd.ExecuteReader())
                            {
                                id = Convert.ToInt32(outputParameter.Value);
                            }

                            if (id > 0) blnReturn = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //return ex.Message;
                Console.Write(string.Format("XWTWebAPI.Utilities.IsValidated{0}Error:{1}", Environment.NewLine, ex.Message));
                blnReturn = false;
            }

            return blnReturn;
        }

        public void GetAllTournamentInfo(SqlConnection sqlConn, ref List<TournamentMain>returnTournaments)
        {

            //Grab all the players associated with this tournament
            foreach (TournamentMain newTournament in returnTournaments)
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
                                SOS = sqlReader.GetDecimal(sqlReader.GetOrdinal("SOS")),
                                API_UserAccountId = sqlReader.GetInt32(sqlReader.GetOrdinal("UserAccountId"))
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
}
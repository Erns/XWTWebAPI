﻿using Newtonsoft.Json;
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
    public class PlayersController : ApiController
    {
        List<Player> players = new List<Player>();
       
        // GET api/values
        public string Get()
        {
            var test = JsonConvert.SerializeObject(players);

            return "Need ID";
        }

        // GET api/values/5 (READ)
        public string Get(int userid)
        {

            if (!Utilities.IsValidated(Request.Headers))
            {
                return "Validation fail";
            }

            players.Clear();
            using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
            {
                using (SqlCommand sqlCmd = new SqlCommand("dbo.spPlayers_GET", sqlConn))
                {
                    sqlConn.Open();
                    sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@UserAccountId", userid);
                    using (SqlDataReader sqlReader = sqlCmd.ExecuteReader())
                    {
                        while (sqlReader.Read())
                        {
                            players.Add(new Player(
                                sqlReader.GetInt32(sqlReader.GetOrdinal("Id"))
                                , sqlReader.GetString(sqlReader.GetOrdinal("Name"))
                                , sqlReader.GetString(sqlReader.GetOrdinal("Email"))
                                , sqlReader.GetString(sqlReader.GetOrdinal("Group"))
                            ));
                        }
                    }
                }
            }

            return JsonConvert.SerializeObject(players);
        }

        // POST api/values (CREATE)
        public string Post([FromBody]string value)
        {
            try
            {
                if (!Utilities.IsValidated(Request.Headers))
                {
                    return "Validation fail";
                }

                List<Player> result = JsonConvert.DeserializeObject<List<Player>>(JsonConvert.DeserializeObject(value).ToString());

                Console.Write("Test");
            }
            catch(Exception ex)
            {
                return ex.Message;
            }

            return "POST Success";
        }

        // PUT api/values/5 (UPDATE)
        public string Put(int userid, [FromBody]string value)
        {
            try
            {
                if (!Utilities.IsValidated(Request.Headers))
                {
                    return "Validation fail";
                }

                List<Player> result = JsonConvert.DeserializeObject<List<Player>>(JsonConvert.DeserializeObject(value).ToString());

                DataTable dt = new DataTable();
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Email", typeof(string));
                dt.Columns.Add("Group", typeof(string));
                dt.Columns.Add("Active", typeof(bool));
                dt.Columns.Add("DateDeleted", typeof(DateTime));


                foreach (Player player in result)
                {
                    dt.Rows.Add(player.Id, player.Name, player.Email, player.Group, player.Active, player.DateDeleted);
                }

                using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
                {
                    using (SqlCommand sqlCmd = new SqlCommand("dbo.spPlayers_UPDATEINSERT_DT", sqlConn))
                    {
                        sqlConn.Open();
                        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@UserAccountId", userid);
                        sqlCmd.Parameters.Add("@PlayersDataTable", SqlDbType.Structured).Value = dt;

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
        public void Delete(int id)
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                //return "Validation fail";
            }
        }

    }
}

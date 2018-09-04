using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XWTWebAPI.Models;

namespace XWTWebAPI.Controllers
{
    public class PlayersController : ApiController
    {
        List<Player> players = new List<Player>();
       
        private void setupTest()
        {
            players.Clear();
            players.Add(new Player(1, "Test 1", ""));
            players.Add(new Player(2, "test 2", "asdf@asdf.com"));
            players.Add(new Player(3, "test 3", "", "Sparta"));
        }

        // GET api/values
        public string Get()
        {
            setupTest();
            var test = JsonConvert.SerializeObject(players);

            return "Need ID";
        }

        // GET api/values/5
        public string Get(int id)
        {

            players.Clear();
            using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
            {
                using (SqlCommand sqlCmd = new SqlCommand("dbo.spPlayers_GET", sqlConn))
                {
                    sqlConn.Open();
                    sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@UserAccountId", id);
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

        // POST api/values
        public void Post([FromBody]string value)
        {
            List<Player> result = JsonConvert.DeserializeObject<List<Player>>(value);

        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
            List<Player> result = JsonConvert.DeserializeObject<List<Player>>(value);

        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

    }
}

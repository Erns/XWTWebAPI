using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XWTWebAPI.Models;

namespace XWTWebAPI.Controllers
{
    public class UserAccountController : ApiController
    {
        // GET api/values (READ)
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5 (READ)
        public string Get([FromBody]string value)
        {
            UserAccount result = JsonConvert.DeserializeObject<UserAccount>(JsonConvert.DeserializeObject(value).ToString());

            using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
            {
                sqlConn.Open();

                //Check if user account exist
                using (SqlCommand sqlCmd = new SqlCommand("dbo.spUserAccount_VALIDATE", sqlConn))
                {
                    sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@UserName", result.UserName);
                    sqlCmd.Parameters.AddWithValue("@Password", result.Password);

                    using (SqlDataReader sqlRdr = sqlCmd.ExecuteReader())
                    {
                        if (sqlRdr.HasRows)
                        {
                            return "GET: TRUE";
                        }
                    }
                }

            }
            return "GET: FALSE";
        }

        // POST api/values (CREATE)
        public string Post([FromBody]string value)
        {

            UserAccount result = JsonConvert.DeserializeObject<UserAccount>(JsonConvert.DeserializeObject(value).ToString());

            using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
            {
                sqlConn.Open();

                //Check if user account exist
                using (SqlCommand sqlCmd = new SqlCommand("dbo.spUserAccount_EXISTS", sqlConn))
                {
                    sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@UserName", result.UserName);
                    sqlCmd.Parameters.AddWithValue("@Email", result.Email);

                    using (SqlDataReader sqlRdr = sqlCmd.ExecuteReader())
                    {
                        if (sqlRdr.HasRows)
                        {
                            return "POST: User Account Already Exists!";
                        }
                    }
                }

                //Create user
                using (SqlCommand sqlCmd = new SqlCommand("dbo.spUserAccount_INSERT", sqlConn))
                {
                    sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@Name", result.Name);
                    sqlCmd.Parameters.AddWithValue("@Password", result.Password); //Should already be hashed on the client side
                    sqlCmd.Parameters.AddWithValue("@UserName", result.UserName);
                    sqlCmd.Parameters.AddWithValue("@Email", result.Email);
                    sqlCmd.ExecuteNonQuery();
                }
            }

            return "POST: Success";

        }

        // PUT api/values/5 (UPDATE)
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5 (DELETE)
        public void Delete(int id)
        {
        }

    }
}

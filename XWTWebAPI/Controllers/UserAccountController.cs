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
        //// GET api/values (READ)
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/values/5 (READ) (GET doesn't appear to accept [FromBody], so passing as a string instead)
        public string Get(string value)
        {
            try
            {          
                UserAccount result = JsonConvert.DeserializeObject<UserAccount>(JsonConvert.DeserializeObject(value).ToString());

                using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
                {
                    sqlConn.Open();

                    int intUserId = 0;

                    //Check if user account exist
                    using (SqlCommand sqlCmd = new SqlCommand("dbo.spUserAccounts_VALIDATE", sqlConn))
                    {
                        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@UserName", result.UserName);
                        sqlCmd.Parameters.AddWithValue("@Password", result.Password);

                        SqlParameter outputParameter = new SqlParameter("@Id", SqlDbType.Int);
                        outputParameter.Direction = ParameterDirection.Output;

                        sqlCmd.Parameters.Add(outputParameter);

                        using (SqlDataReader sqlRdr = sqlCmd.ExecuteReader())
                        {
                            intUserId = Convert.ToInt32(outputParameter.Value);
                        }
                    }

                    if (intUserId == 0)
                    {
                        return "GET: FALSE UserId = 0";
                    }
                    else
                    {
                        UserAccount user = new UserAccount();

                        //Get the basic user info
                        using (SqlCommand sqlCmd = new SqlCommand("dbo.spUserAccounts_GETBASIC", sqlConn))
                        {
                            sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                            sqlCmd.Parameters.AddWithValue("@Id", intUserId);
                            sqlCmd.Parameters.AddWithValue("@Password", result.Password);

                            using (SqlDataReader sqlRdr = sqlCmd.ExecuteReader())
                            {
                                if (sqlRdr.Read())
                                {
                                    user.Id = sqlRdr.GetInt32(sqlRdr.GetOrdinal("Id"));
                                    user.UserName = sqlRdr.GetString(sqlRdr.GetOrdinal("UserName"));
                                    user.Email = sqlRdr.GetString(sqlRdr.GetOrdinal("Email"));
                                    return JsonConvert.SerializeObject(user);
                                }
                            }
                        }
                    }

                }
                return "GET: FALSE";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // POST api/values (CREATE)
        public string Post([FromBody]string value)
        {

            try
            {            
                UserAccount result = JsonConvert.DeserializeObject<UserAccount>(JsonConvert.DeserializeObject(value).ToString());

                using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
                {
                    sqlConn.Open();

                    //Check if user account exist
                    using (SqlCommand sqlCmd = new SqlCommand("dbo.spUserAccounts_EXISTS", sqlConn))
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
                    using (SqlCommand sqlCmd = new SqlCommand("dbo.spUserAccounts_UPDATEINSERT", sqlConn))
                    {
                        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@Id", "0");
                        sqlCmd.Parameters.AddWithValue("@UserName", result.UserName);
                        sqlCmd.Parameters.AddWithValue("@Password", result.Password); //Should already be hashed on the client side
                        sqlCmd.Parameters.AddWithValue("@Email", result.Email);
                        sqlCmd.ExecuteNonQuery();
                    }
                }

                return "POST: Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
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

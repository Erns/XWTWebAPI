using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

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
    }
}
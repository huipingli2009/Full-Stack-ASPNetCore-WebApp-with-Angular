using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PHO_WebApp.Models;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;


namespace PHO_WebApp.DataAccessLayer
{
    public class LoginDAL
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        public int? GetUserLogin(string username, string password)
        {
            int? returnValue = null;

            SqlCommand com = new SqlCommand("spGetActiveLogin", con);
            com.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterUserName= new SqlParameter("@UserName", SqlDbType.NVarChar, 50);
            parameterUserName.Value = username;
            com.Parameters.Add(parameterUserName);

            SqlParameter parameterPassword = new SqlParameter("@Password", SqlDbType.NVarChar, 50);
            parameterPassword.Value = password;
            com.Parameters.Add(parameterPassword);

            //com.Parameters.AddWithValue("@UserName", username);
            //com.Parameters.AddWithValue("@Password", password);

            //Output Parameter
            SqlParameter parameterUserId = new SqlParameter("@LoginId", SqlDbType.Int, 4);
            parameterUserId.Direction = System.Data.ParameterDirection.Output;
            com.Parameters.Add(parameterUserId);

            con.Open();
            com.ExecuteNonQuery();
            con.Close();
            if (parameterUserId != null)
            {
                returnValue = int.Parse(parameterUserId.Value.ToString());
            }

            return returnValue;
        }

        public UserDetails GetPersonLoginForLoginId(int loginId)
        {
            UserDetails returnObject = null;
                        
            SqlCommand com = new SqlCommand("spGetPersonLoginForLoginId", con);
            com.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterLoginId = new SqlParameter("@LoginId", SqlDbType.Int);
            parameterLoginId.Value = loginId;
            com.Parameters.Add(parameterLoginId);

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();

            da.Fill(ds);

            //Fill Model
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows != null)
            {
                returnObject = new UserDetails();
                returnObject.Id = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                returnObject.PersonId = int.Parse(ds.Tables[0].Rows[0][1].ToString());
                returnObject.UserName = ds.Tables[0].Rows[0][2].ToString();
            }

            return returnObject;
        }

    }
}
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

        public int GetUserLogin(string username, string password)
        {
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
            SqlParameter parameterUserId = new SqlParameter("@UserId", SqlDbType.Int, 4);
            parameterUserId.Direction = System.Data.ParameterDirection.Output;
            com.Parameters.Add(parameterUserId);

            con.Open();
            com.ExecuteNonQuery();
            con.Close();
            return int.Parse(parameterUserId.Value.ToString());            
        }

    }
}
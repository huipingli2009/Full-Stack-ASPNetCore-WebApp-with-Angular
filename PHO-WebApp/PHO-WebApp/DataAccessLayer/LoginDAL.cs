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

        public int? GetUserLogin(string username)
        {
            int? returnValue = null;

            SqlCommand com = new SqlCommand("spGetActiveLogin", con);
            com.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterUserName= new SqlParameter("@UserName", SqlDbType.NVarChar, 50);
            parameterUserName.Value = username;
            com.Parameters.Add(parameterUserName);

            //SqlParameter parameterPassword = new SqlParameter("@Password", SqlDbType.NVarChar, 50);
            //parameterPassword.Value = password;
            //com.Parameters.Add(parameterPassword);

            //com.Parameters.AddWithValue("@UserName", username);
            //com.Parameters.AddWithValue("@Password", password);

            //Output Parameter
            SqlParameter parameterUserId = new SqlParameter("@LoginId", SqlDbType.Int, 4);
            parameterUserId.Direction = System.Data.ParameterDirection.Output;
            com.Parameters.Add(parameterUserId);

            con.Open();
            com.ExecuteNonQuery();
            con.Close();

            if (parameterUserId != null && parameterUserId.Value != null && !string.IsNullOrWhiteSpace(parameterUserId.Value.ToString()))
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
                returnObject = CreateUserDetailsModel(ds.Tables[0].Rows[0]);
            }

            return returnObject;
        }



        public UserDetails CreateUserDetailsModel()
        {
            UserDetails ud = new UserDetails();
            
            ud.AllStaffTypes = GetStaffTypes();
            ud.StaffTypeId = -1;
            return ud;
        }

        public UserDetails CreateUserDetailsModel(DataRow dr)
        {
            UserDetails ud = new UserDetails();
            if (dr["LoginId"] != null && !string.IsNullOrWhiteSpace(dr["LoginId"].ToString()))
            {
                ud.LoginId = int.Parse(dr["LoginId"].ToString());
            }
            if (dr["PersonId"] != null && !string.IsNullOrWhiteSpace(dr["PersonId"].ToString()))
            {
                ud.StaffId = int.Parse(dr["PersonId"].ToString());
            }

            ud.UserName = dr["UserName"].ToString();
            ud.Password = dr["Password"].ToString();

            if (dr["StaffTypeId"] != null && !string.IsNullOrWhiteSpace(dr["StaffTypeId"].ToString()))
            {
                ud.StaffTypeId = int.Parse(dr["StaffTypeId"].ToString());
            }

            ud.AllStaffTypes = GetStaffTypes();

            return ud;
        }


        public List<StaffType> GetStaffTypes()
        {
            List<StaffType> returnObject = null;

            SqlCommand com = new SqlCommand("spGetStaffTypes", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            //Cohorts = ds.Tables[0].
            da.Fill(ds);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (returnObject == null)
                {
                    returnObject = new List<StaffType>();
                }
                StaffType c = CreateStaffTypeModel(ds.Tables[0].Rows[i]);
                returnObject.Add(c);
            }

            return returnObject;
        }

        public StaffType CreateStaffTypeModel(DataRow dr)
        {
            StaffType c = new StaffType();
            if (dr["Id"] != null && !string.IsNullOrWhiteSpace(dr["Id"].ToString()))
            {
                c.Id = int.Parse(dr["Id"].ToString());
            }
            c.Name = dr["Name"].ToString();

            return c;
        }

    }
}
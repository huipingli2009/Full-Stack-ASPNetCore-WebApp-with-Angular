using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using PHO_WebApp.Models;


namespace PHO_WebApp.DataAccessLayer
{
    public class AuditDAL
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);


        public void LogAudit(UserDetails user, string controller, string action, string message)
        {
            string session = null;
            string username = null;
            int login = -1;

            SqlCommand com = new SqlCommand("spInsertAuditLog", con);
            com.CommandType = CommandType.StoredProcedure;

            if (user != null)
            {
                if (user.SessionId != null)
                {
                    session = user.SessionId;
                }
                if (user.UserName != null)
                {
                    username = user.UserName;
                }
                login = user.LoginId;

            }

            // Add Parameters to SPROC
            SqlParameter parameterAuditType = new SqlParameter("@AuditType", SqlDbType.NVarChar, 5);
            parameterAuditType.Value = "AUDIT";
            com.Parameters.Add(parameterAuditType);

            SqlParameter parameterUserName = new SqlParameter("@UserName", SqlDbType.NVarChar, 50);
            parameterUserName.Value = username;
            if (username == null)
            {
                parameterUserName.Value = DBNull.Value;
            }
            com.Parameters.Add(parameterUserName);

            SqlParameter parameterSession = new SqlParameter("@SessionID", SqlDbType.NVarChar, 50);
            parameterSession.Value = session;
            if (session == null)
            {
                parameterSession.Value = DBNull.Value;
            }
            com.Parameters.Add(parameterSession);

            SqlParameter parameterLogin = new SqlParameter("@LoginId", SqlDbType.Int);
            parameterLogin.Value = login;
            if (login < 0)
            {
                parameterLogin.Value = DBNull.Value;
            }
            com.Parameters.Add(parameterLogin);

            SqlParameter parameterAction = new SqlParameter("@Action", SqlDbType.NVarChar, 50);
            parameterAction.Value = action;
            com.Parameters.Add(parameterAction);

            SqlParameter parameterController = new SqlParameter("@Controller", SqlDbType.NVarChar, 50);
            parameterController.Value = controller;
            com.Parameters.Add(parameterController);

            SqlParameter parameterMessage = new SqlParameter("@Message", SqlDbType.NVarChar, 500);
            parameterMessage.Value = message;
            com.Parameters.Add(parameterMessage);

            //LEave these null, error only fields
            SqlParameter parameterInnerMessage = new SqlParameter("@InnerMessage", SqlDbType.NVarChar, 500);
            parameterInnerMessage.Value = DBNull.Value;
            com.Parameters.Add(parameterInnerMessage);

            SqlParameter parameterStackTrace = new SqlParameter("@StackTrace", SqlDbType.NVarChar, 500);
            parameterStackTrace.Value = DBNull.Value;
            com.Parameters.Add(parameterStackTrace);

            con.Open();
            com.ExecuteNonQuery();
            con.Close();
            con.Dispose();
        }

        public void LogError(UserDetails user, string controller, string action, string message, string innerMessage, string stackTrace)
        {
            string session = null;
            string username = null;
            int login = -1;

            SqlCommand com = new SqlCommand("spInsertAuditLog", con);
            com.CommandType = CommandType.StoredProcedure;

            if (user != null)
            {
                if (user.SessionId != null)
                {
                    session = user.SessionId;
                }
                if (user.UserName != null)
                {
                    username = user.UserName;
                }
                login = user.LoginId;

            }

            // Add Parameters to SPROC
            SqlParameter parameterAuditType = new SqlParameter("@AuditType", SqlDbType.NVarChar, 5);
            parameterAuditType.Value = "ERROR";
            com.Parameters.Add(parameterAuditType);


            SqlParameter parameterUserName = new SqlParameter("@UserName", SqlDbType.NVarChar, 50);
            parameterUserName.Value = username;
            if (username == null)
            {
                parameterUserName.Value = DBNull.Value;
            }
            com.Parameters.Add(parameterUserName);

            SqlParameter parameterSession = new SqlParameter("@SessionID", SqlDbType.NVarChar, 50);
            parameterSession.Value = session;
            if (session == null)
            {
                parameterSession.Value = DBNull.Value;
            }
            com.Parameters.Add(parameterSession);

            SqlParameter parameterLogin = new SqlParameter("@LoginId", SqlDbType.Int);
            parameterLogin.Value = login;
            if (login < 0)
            {
                parameterLogin.Value = DBNull.Value;
            }
            com.Parameters.Add(parameterLogin);

            SqlParameter parameterAction = new SqlParameter("@Action", SqlDbType.NVarChar, 50);
            parameterAction.Value = action;
            if (string.IsNullOrEmpty(action))
            {
                parameterAction.Value = DBNull.Value;
            }
            com.Parameters.Add(parameterAction);

            SqlParameter parameterController = new SqlParameter("@Controller", SqlDbType.NVarChar, 50);
            parameterController.Value = controller;
            if (string.IsNullOrEmpty(controller))
            {
                parameterController.Value = DBNull.Value;
            }
            com.Parameters.Add(parameterController);

            SqlParameter parameterMessage = new SqlParameter("@Message", SqlDbType.NVarChar, 500);
            parameterMessage.Value = message;
            if (string.IsNullOrEmpty(message))
            {
                parameterMessage.Value = DBNull.Value;
            }
            com.Parameters.Add(parameterMessage);

            //LEave these null, error only fields
            SqlParameter parameterInnerMessage = new SqlParameter("@InnerMessage", SqlDbType.NVarChar, 500);
            parameterInnerMessage.Value = innerMessage;
            if (string.IsNullOrEmpty(innerMessage))
            {
                parameterInnerMessage.Value = DBNull.Value;
            }
            com.Parameters.Add(parameterInnerMessage);

            SqlParameter parameterStackTrace = new SqlParameter("@StackTrace", SqlDbType.NVarChar, 500);
            parameterStackTrace.Value = stackTrace;
            if (string.IsNullOrEmpty(stackTrace))
            {
                parameterInnerMessage.Value = DBNull.Value;
            }
            com.Parameters.Add(parameterStackTrace);

            con.Open();
            com.ExecuteNonQuery();
            con.Close();
            con.Dispose();
        }
    }
}
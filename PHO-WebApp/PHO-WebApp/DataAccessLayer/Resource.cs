using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using PHO_WebApp.Models;
using System.Security.Cryptography;
using PHO_WebApp.ViewModel;

namespace PHO_WebApp.DataAccessLayer
{   
    public class Resource
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
        //public List<Files> getPracticeResourceFiles(int loginId)
        //{
        //    List<Files> PracticeResoourceFiles = new List<Files>();

        //    SqlCommand com = new SqlCommand("spGetResourceFiles", con);
        //    com.CommandType = CommandType.StoredProcedure;

        //    SqlParameter parameterLoginId = new SqlParameter("@LoginID", SqlDbType.Int);
        //    parameterLoginId.Value = loginId;
        //    com.Parameters.Add(parameterLoginId);           

        //    SqlDataAdapter da = new SqlDataAdapter(com);
        //    DataSet ds = new DataSet();

        //    da.Fill(ds);

        //    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //    {
        //        Files file = CreatePracticeREsourceFilesModel(ds.Tables[0].Rows[i]);
        //        PracticeResoourceFiles.Add(file);
        //    }

        //    //PracticeResoourceFiles.OrderByDescending()
        //    return PracticeResoourceFiles;
        //}
        //public List<Files> getPracticeResourceFiles(int loginId, string topFilter, string tagFilter, string searchBox)
        public List<Files> getPracticeResourceFiles(int loginId, string topfilter, string searchBox, string folder, string subfolder)
        {
            List<Files> PracticeResoourceFiles = new List<Files>();

            if (string.IsNullOrWhiteSpace(topfilter))
            {
                topfilter = "All Files";
            }

            //if (string.IsNullOrWhiteSpace(tagFilter))
            //{
            //    tagFilter = "All";
            //}

            if (string.IsNullOrWhiteSpace(searchBox))
            {
                searchBox = "All";
            }

            if (string.IsNullOrWhiteSpace(folder))
            {
                folder = "All";
            }

            if (string.IsNullOrWhiteSpace(subfolder))
            {
                subfolder = "All";
            }

            SqlCommand com = new SqlCommand("spGetResourceFiles", con);
            com.CommandType = CommandType.StoredProcedure;

            SqlParameter parameterLoginId = new SqlParameter("@LoginID", SqlDbType.Int);
            parameterLoginId.Value = loginId;
            com.Parameters.Add(parameterLoginId);

            //SqlParameter parameterTopFilter = new SqlParameter("@TopFilter", SqlDbType.NVarChar);
            //parameterTopFilter.Value = topFilter;
            //com.Parameters.Add(parameterTopFilter);

            //SqlParameter parameterTagFilter = new SqlParameter("@TagFilter", SqlDbType.NVarChar);
            //parameterTagFilter.Value = tagFilter;
            //com.Parameters.Add(parameterTagFilter);

            SqlParameter parameterSearchBox = new SqlParameter("@SearchBox", SqlDbType.NVarChar);
            parameterSearchBox.Value = searchBox;
            com.Parameters.Add(parameterSearchBox);

            SqlParameter parameterTopFilter = new SqlParameter("@TopFilter", SqlDbType.NVarChar);
            parameterTopFilter.Value = topfilter;
            com.Parameters.Add(parameterTopFilter);

            SqlParameter parameterfolder = new SqlParameter("@Folder", SqlDbType.NVarChar);
            parameterfolder.Value = folder;
            com.Parameters.Add(parameterfolder);

            SqlParameter parametersubfolder = new SqlParameter("@SubFolder", SqlDbType.NVarChar);
            parametersubfolder.Value = subfolder;
            com.Parameters.Add(parametersubfolder);

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();

            da.Fill(ds);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Files file = CreatePracticeREsourceFilesModel(ds.Tables[0].Rows[i]);
                PracticeResoourceFiles.Add(file);
            }

            //PracticeResoourceFiles.OrderByDescending()
            return PracticeResoourceFiles;
        }

        public Files CreatePracticeREsourceFilesModel(DataRow dr)
        {
            Files s = new Files();

            s.ResourceId = int.Parse(dr["ResourceId"].ToString());

            if (dr["FileName"].ToString() != null && !string.IsNullOrWhiteSpace(dr["FileName"].ToString()))
            {
                s.FileName = dr["FileName"].ToString();
            }
            if (dr["FileUrl"].ToString() != null && !string.IsNullOrWhiteSpace(dr["FileUrl"].ToString()))
            {
                s.FileUrl = dr["FileURL"].ToString();
            }
            if (dr["Folder"].ToString()!= null && !string.IsNullOrWhiteSpace(dr["Folder"].ToString()))
            {
                s.Folder = dr["Folder"].ToString();
            }

            if (dr["SubFolder"].ToString() != null && !string.IsNullOrWhiteSpace(dr["SubFolder"].ToString()))
            {
                s.SubFolder = dr["SubFolder"].ToString();
            }                     

            if (dr["AdminURL"].ToString() != null && !string.IsNullOrWhiteSpace(dr["AdminURL"].ToString()))
            {
                s.AdminURL = dr["AdminURL"].ToString();
            }            

            if (dr["PermissionToId"].ToString() != null && !string.IsNullOrWhiteSpace(dr["PermissionToId"].ToString()))
            {
                s.PermissionToId = dr["PermissionToId"].ToString();
            }

            if (dr["PermissionToName"].ToString() != null && !string.IsNullOrWhiteSpace(dr["PermissionToName"].ToString()))
            {
                s.PermissionToName = dr["PermissionToName"].ToString();
            }

            if (dr["Tags"].ToString() != null && !string.IsNullOrWhiteSpace(dr["Tags"].ToString()))
            {
                s.Tags = dr["Tags"].ToString();
            }

            if (dr["Favorite"].ToString() != null && !string.IsNullOrWhiteSpace(dr["Favorite"].ToString()))
            {
                s.Favorite = dr["Favorite"].ToString();
            }

            if (dr["ActionId"].ToString() != null && !string.IsNullOrWhiteSpace(dr["ActionId"].ToString()))
            {
                s.ActionId = int.Parse(dr["ActionId"].ToString());
            }

            if (dr["FIleSize"].ToString() != null && !string.IsNullOrWhiteSpace(dr["FIleSize"].ToString()))
            {
                s.FileSize = dr["FIleSize"].ToString();
            }

            if (dr["DateModified"].ToString() != null && !string.IsNullOrWhiteSpace(dr["DateModified"].ToString()))
            {
                s.DateModified = DateTime.Parse(dr["DateModified"].ToString());
            }

            //if (dr["Id"] != null && !string.IsNullOrWhiteSpace(dr["Id"].ToString()))
            //{
            //    s.Id = SharedLogic.ParseNumeric(dr["Id"].ToString());
            //}

            //if (dr["StaffTypeId"] != null && !string.IsNullOrWhiteSpace(dr["StaffTypeId"].ToString()))
            //{
            //    s.StaffTypeId = SharedLogic.ParseNumeric(dr["StaffTypeId"].ToString());
            //}     

            return s;
        }

    }
}
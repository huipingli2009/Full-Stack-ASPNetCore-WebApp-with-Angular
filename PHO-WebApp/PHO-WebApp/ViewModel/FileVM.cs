using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PHO_WebApp.Models;
using PHO_WebApp.DataAccessLayer;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Text;

namespace PHO_WebApp.ViewModel
{
    public class FileVM
    {
        private SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
        DataAccessLayer.Resource FileResource = new Resource();
        public Files file { get; set; }
        public List<Files> FileList { get; set; }
        public UserDetails UserLogin { get; set; }
        public FileVM()
        {
            file = new Files();
            FileList = new List<Files>();
            //FileList = 
            UserLogin = new UserDetails();
        }
        //public FileVM GetFiles()
        //{
        //    Resource files = new Resource();
        //    FileVM fvm = new FileVM();

        //    fvm.FileList = files.getPracticeResourceFiles(UserLogin.LoginId);
        //    return fvm;
        //}
        public FileVM GetFiles(string topfilter, string searchBox, string folder, string subfolder)
        {
            Resource files = new Resource();
            FileVM fvm = new FileVM();

            fvm.FileList = files.getPracticeResourceFiles(UserLogin.LoginId, topfilter, searchBox, folder, subfolder);
            return fvm;
        }
    }
}
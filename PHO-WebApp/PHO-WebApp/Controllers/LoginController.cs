using PHO_WebApp.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace PHO_WebApp.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login

        DataAccessLayer.LoginDAL userLogin = new LoginDAL();

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection fc)
        {
            int id = userLogin.GetUserLogin(fc["UserName"], fc["Password"]);
           
            if (id > 0)
            {
                Session["username"] = fc["UserName"].ToString();
                return RedirectToAction("GetPatients", "Patient");
                //return RedirectToAction("DataDictionary", "Home");
            }
            else
            {
                ViewData["message"] = "Login failed!";
            }
      
            return View();
        }
    }
}
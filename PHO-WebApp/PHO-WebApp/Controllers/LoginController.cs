using PHO_WebApp.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using PHO_WebApp.Models;

namespace PHO_WebApp.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login

        DataAccessLayer.LoginDAL userLogin = new LoginDAL();

        public ActionResult Login()
        {
            if (Session["UserDetails"] != null)
            {
                return View("Login", (Models.UserDetails)Session["UserDetails"]);
            }
            else
            {
                return View("Login");
            }
        }
        [HttpPost]
        public ActionResult Login(UserDetails userDetails)
        {
            if (ModelState.IsValid)
            {
                int? id = userLogin.GetUserLogin(userDetails.UserName, userDetails.Password);

                if (id.HasValue && id.Value > 0)
                {
                    userDetails = userLogin.GetPersonLoginForLoginId(id.Value);
                    Session["UserDetails"] = userDetails;
                    return RedirectToAction("DataDictionary", "Home", new { area = "Home" });
                }
                else
                {
                    ViewData["message"] = "Login failed!";
                }
            }
      
            return View(userDetails);
        }

        public ActionResult Logout()
        {
            Session["UserDetails"] = null;
            return RedirectToAction("Index", "Home");
        }

        [ChildActionOnly]
        public ActionResult LoggedInAs()
        {
            if (Session["UserDetails"] != null)
            {
                return PartialView("LoggedInAs", (Models.UserDetails)Session["UserDetails"]);
            }

            return PartialView("LoggedInAs");
        }

        [ChildActionOnly]
        public ActionResult LoginForm()
        {
            if (Session["UserDetails"] != null)
            {
                return PartialView("LoginForm", (Models.UserDetails)Session["UserDetails"]);
            }
            else
            {
                return PartialView("LoginForm");
            }
        }
    }
}
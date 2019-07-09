using PHO_WebApp.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using PHO_WebApp.Models;
using System.Security.Cryptography;

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
        public ActionResult SubmitLoginPartial(string username, string password)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int? id = userLogin.GetUserLogin(username);

                    if (id.HasValue && id.Value > 0)
                    {
                        UserDetails savedUserDetails = userLogin.GetPersonLoginForLoginId(id.Value);
                        VerifyPassword(savedUserDetails.Password, password);
                        Session["UserDetails"] = savedUserDetails;
                        //return RedirectToAction("Index", "Home", new { area = "Home" });
                    }
                    else
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    return Json(new { Success = false });
                }

            }

            return Json(new { Success = true });
        }

        public ActionResult OpenLogin()
        {
            return PartialView("LoginDialog");
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

        private string HashAndSaltPassword(string plainTextPassword)
        {
            string ReturnValue = string.Empty;

            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(plainTextPassword, salt, 10000);

            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];

            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            string savedPasswordHash = Convert.ToBase64String(hashBytes);

            if (!string.IsNullOrWhiteSpace(savedPasswordHash))
            {
                ReturnValue = savedPasswordHash;
            }

            return ReturnValue;
        }

        private bool VerifyPassword(string savedPasswordHash, string enteredPlainTextPassword)
        {
            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(enteredPlainTextPassword, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    throw new UnauthorizedAccessException();
            }
            
            return true;
        }
    }
}
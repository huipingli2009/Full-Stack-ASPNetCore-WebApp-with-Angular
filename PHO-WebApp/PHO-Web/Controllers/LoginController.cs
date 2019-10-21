using PHO_WebApp.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Data;
using PHO_WebApp.Models;
using System.Security.Cryptography;

namespace PHO_WebApp.Controllers
{
    public class LoginController : BaseController
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
                        savedUserDetails.SessionId = this.Session.SessionID;
                        Session["UserId"] = id;
                        Session["UserDetails"] = savedUserDetails;
                        SharedLogic.LogAudit(savedUserDetails, "LoginController", "SubmitLoginPartial", "Successful login. Username: " + savedUserDetails.UserName);
                    }
                    else
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    SharedLogic.LogAudit(null, "LoginController", "SubmitLoginPartial", "User unsuccessfully attempted to login. Bad password / login combination.");
                    return Json(new { Success = false });
                }
                //catch (Exception ex)
                //{
                //    SharedLogic.LogError(null, "LoginController", "SubmitLoginPartial", ex);
                //    return Json(new { Success = false });
                //}

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

        // GET: Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Login/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserDetails model)
        {
            if (ModelState.IsValid)
            {
                if(model.LoginId > 0)  //update 
                {
                    //update method here
                }
                else
                {                    
                    userLogin.RegisterUser(model);
                }
            }
            else
            {

            }
            //return View();
            return RedirectToAction("Index", "Home");            
        }

        //public async Task<ActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

        //            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
        //            // Send an email with this link
        //            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

        //            return RedirectToAction("Index", "Home");
        //        }
        //        AddErrors(result);
        //    }

            // If we got this far, something failed, redisplay form
            //return View(model);
        }

        ////
        //// GET: /Account/ConfirmEmail
        //[AllowAnonymous]
        //public async Task<ActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }
        //    var result = await UserManager.ConfirmEmailAsync(userId, code);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
 }
   
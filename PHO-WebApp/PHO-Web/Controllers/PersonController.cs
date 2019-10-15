using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using PHO_WebApp.DataAccessLayer;
//test

namespace PHO_WebApp.Controllers
{
    public class PersonController : BaseController
    {
        private PHOPersonEntities1 db = new PHOPersonEntities1();
        // GET: Person
        public ActionResult Patients()
        {
            return View(db.Patients.ToList());
        }

        //public ActionResult Edit( int id)
        //{
        //    return View(Context.)
        //}
    }
}
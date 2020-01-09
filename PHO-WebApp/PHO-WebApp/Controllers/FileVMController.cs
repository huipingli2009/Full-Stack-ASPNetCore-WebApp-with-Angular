using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PHO_WebApp.ViewModel;

namespace PHO_WebApp.Controllers
{
    public class FileVMController : BaseController
    {
        // GET: FileVM
        //public ActionResult Index()
        //{
        //    FileVM fvm = new FileVM();
        //    fvm.UserLogin = SavedUserDetails;

        //    return View(fvm.GetFiles());
        //}
        public ActionResult Index(string searchString)
        {
            //@ViewData["searchString"] = searchString;

            //if (!String.IsNullOrEmpty(@ViewData["searchString"].ToString()))
            //{
            //    searchString = @ViewData["searchString"].ToString();               
            //}
            //if (@ViewData["searchString"].ToString())
            ////searchString = @ViewData["searchString"].ToString();
            //searchString = "TOP 100";
            FileVM fvm = new FileVM();
            fvm.UserLogin = SavedUserDetails;

            return View(fvm.GetFiles(searchString));
        }

        // GET: FileVM/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FileVM/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FileVM/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: FileVM/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FileVM/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: FileVM/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FileVM/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

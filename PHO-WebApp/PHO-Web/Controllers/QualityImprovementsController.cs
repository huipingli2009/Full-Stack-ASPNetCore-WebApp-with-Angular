using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using PHO_WebApp.Models;
using PHO_WebApp.DataAccessLayer;

namespace PHO_WebApp.Controllers
{
    public class QualityImprovementsController : Controller
    {
        private QualityContext db = new QualityContext();

        DataAccessLayer.QualityImprovementDAL QIDAL = new QualityImprovementDAL();

        // GET: QualityImprovements
        public ActionResult Index()
        {
            int userId = 2;
            QualityImprovement QI = QIDAL.getAllQualityImprovementsForPractice(userId);
            //return View(db.QualityImprovements.ToList());
            return View(QI);
        }

        // GET: QualityImprovements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QualityImprovement qualityImprovement = db.QualityImprovements.Find(id);
            if (qualityImprovement == null) 
            {
                return HttpNotFound();
            }
            return View(qualityImprovement);
        }

        // GET: QualityImprovements/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QualityImprovements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Desc,InitiativeId")] QualityImprovement qualityImprovement)
        {
            if (ModelState.IsValid)
            {
                db.QualityImprovements.Add(qualityImprovement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(qualityImprovement);
        }

        // GET: QualityImprovements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QualityImprovement qualityImprovement = db.QualityImprovements.Find(id);
            if (qualityImprovement == null)
            {
                return HttpNotFound();
            }
            return View(qualityImprovement);
        }

        // POST: QualityImprovements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Desc,InitiativeId")] QualityImprovement qualityImprovement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(qualityImprovement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(qualityImprovement);
        }

        // GET: QualityImprovements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QualityImprovement qualityImprovement = db.QualityImprovements.Find(id);
            if (qualityImprovement == null)
            {
                return HttpNotFound();
            }
            return View(qualityImprovement);
        }

        // POST: QualityImprovements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QualityImprovement qualityImprovement = db.QualityImprovements.Find(id);
            db.QualityImprovements.Remove(qualityImprovement);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult PracticeDashboard()
        {
            int userId = 2;
            QualityImprovement QI = QIDAL.getAllQualityImprovementsForPractice(userId);
            return View("PracticeDashboard", QI);
        }
    }
}

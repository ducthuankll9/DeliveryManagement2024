using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DeliveryManagement.Models;

namespace DeliveryManagement.Areas.Drivers.Controllers
{
    public class LinehaulsController : Controller
    {
        private DeliveryDatabaseEntities db = new DeliveryDatabaseEntities();

        // GET: Drivers/Linehauls
        public ActionResult Index()
        {
            var linehauls = db.Linehauls.Include(l => l.Staff).Include(l => l.Staff1).Include(l => l.Vehicle);
            return View(linehauls.ToList());
        }







        // GET: Drivers/Linehauls/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Linehaul linehaul = db.Linehauls.Find(id);
            if (linehaul == null)
            {
                return HttpNotFound();
            }
            return View(linehaul);
        }

        // GET: Drivers/Linehauls/Create
        public ActionResult Create()
        {
            ViewBag.Driver = new SelectList(db.Staffs, "StaffID", "Password");
            ViewBag.Operator = new SelectList(db.Staffs, "StaffID", "Password");
            ViewBag.VehicleNumber = new SelectList(db.Vehicles, "VehicleNumber", "Type");
            return View();
        }

        // POST: Drivers/Linehauls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LinehaulID,VehicleNumber,NumberOfPackage,NumberOfOrder,Seal,Operator,Driver")] Linehaul linehaul)
        {
            if (ModelState.IsValid)
            {
                db.Linehauls.Add(linehaul);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Driver = new SelectList(db.Staffs, "StaffID", "Password", linehaul.Driver);
            ViewBag.Operator = new SelectList(db.Staffs, "StaffID", "Password", linehaul.Operator);
            ViewBag.VehicleNumber = new SelectList(db.Vehicles, "VehicleNumber", "Type", linehaul.VehicleNumber);
            return View(linehaul);
        }

        // GET: Drivers/Linehauls/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Linehaul linehaul = db.Linehauls.Find(id);
            if (linehaul == null)
            {
                return HttpNotFound();
            }
            ViewBag.Driver = new SelectList(db.Staffs, "StaffID", "Password", linehaul.Driver);
            ViewBag.Operator = new SelectList(db.Staffs, "StaffID", "Password", linehaul.Operator);
            ViewBag.VehicleNumber = new SelectList(db.Vehicles, "VehicleNumber", "Type", linehaul.VehicleNumber);
            return View(linehaul);
        }

        // POST: Drivers/Linehauls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LinehaulID,VehicleNumber,NumberOfPackage,NumberOfOrder,Seal,Operator,Driver")] Linehaul linehaul)
        {
            if (ModelState.IsValid)
            {
                db.Entry(linehaul).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Driver = new SelectList(db.Staffs, "StaffID", "Password", linehaul.Driver);
            ViewBag.Operator = new SelectList(db.Staffs, "StaffID", "Password", linehaul.Operator);
            ViewBag.VehicleNumber = new SelectList(db.Vehicles, "VehicleNumber", "Type", linehaul.VehicleNumber);
            return View(linehaul);
        }

        // GET: Drivers/Linehauls/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Linehaul linehaul = db.Linehauls.Find(id);
            if (linehaul == null)
            {
                return HttpNotFound();
            }
            return View(linehaul);
        }

        // POST: Drivers/Linehauls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Linehaul linehaul = db.Linehauls.Find(id);
            db.Linehauls.Remove(linehaul);
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
    }
}

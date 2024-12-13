using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DeliveryManagement.Models;

namespace DeliveryManagement.Areas.Orders.Controllers
{
    public class DemoController : Controller
    {
        private DeliveryDatabaseEntities db = new DeliveryDatabaseEntities();

        // GET: Orders/Demo
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Staff).Include(o => o.Station).Include(o => o.Station1).Include(o => o.Station2).Include(o => o.Station3);
            return View(orders.ToList());
        }

        // GET: Orders/Demo/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Demo/Create
        public ActionResult Create()
        {
            ViewBag.Creator = new SelectList(db.Staffs, "StaffID", "Password");
            ViewBag.CurrentStationID = new SelectList(db.Stations, "StationID", "StationName");
            ViewBag.FirstStation = new SelectList(db.Stations, "StationID", "StationName");
            ViewBag.LastStation = new SelectList(db.Stations, "StationID", "StationName");
            ViewBag.Transit = new SelectList(db.Stations, "StationID", "StationName");
            return View();
        }

        // POST: Orders/Demo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,SenderName,SenderAddress,SenderPhone,SenderEmail,ReceiverName,ReceiverAddress,ReceiverPhone,FirstStation,Transit,LastStation,Fee,Paid,TotalWeight,CurrentStationID,Creator,OrderPrice,OnDelivering")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Creator = new SelectList(db.Staffs, "StaffID", "Password", order.Creator);
            ViewBag.CurrentStationID = new SelectList(db.Stations, "StationID", "StationName", order.CurrentStationID);
            ViewBag.FirstStation = new SelectList(db.Stations, "StationID", "StationName", order.FirstStation);
            ViewBag.LastStation = new SelectList(db.Stations, "StationID", "StationName", order.LastStation);
            ViewBag.Transit = new SelectList(db.Stations, "StationID", "StationName", order.Transit);
            return View(order);
        }

        // GET: Orders/Demo/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.Creator = new SelectList(db.Staffs, "StaffID", "Password", order.Creator);
            ViewBag.CurrentStationID = new SelectList(db.Stations, "StationID", "StationName", order.CurrentStationID);
            ViewBag.FirstStation = new SelectList(db.Stations, "StationID", "StationName", order.FirstStation);
            ViewBag.LastStation = new SelectList(db.Stations, "StationID", "StationName", order.LastStation);
            ViewBag.Transit = new SelectList(db.Stations, "StationID", "StationName", order.Transit);
            return View(order);
        }

        // POST: Orders/Demo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,SenderName,SenderAddress,SenderPhone,SenderEmail,ReceiverName,ReceiverAddress,ReceiverPhone,FirstStation,Transit,LastStation,Fee,Paid,TotalWeight,CurrentStationID,Creator,OrderPrice,OnDelivering")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Creator = new SelectList(db.Staffs, "StaffID", "Password", order.Creator);
            ViewBag.CurrentStationID = new SelectList(db.Stations, "StationID", "StationName", order.CurrentStationID);
            ViewBag.FirstStation = new SelectList(db.Stations, "StationID", "StationName", order.FirstStation);
            ViewBag.LastStation = new SelectList(db.Stations, "StationID", "StationName", order.LastStation);
            ViewBag.Transit = new SelectList(db.Stations, "StationID", "StationName", order.Transit);
            return View(order);
        }

        // GET: Orders/Demo/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Demo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DeliveryManagement.Models;

namespace DeliveryManagement.Areas.Admin.Controllers
{
    public class ShippingRatesController : Controller
    {
        private DeliveryDatabaseEntities db = new DeliveryDatabaseEntities();

        // GET: Admin/ShippingRates
        public ActionResult Index()
        {
            if (Session["StaffID"] == null || !(bool)Session["IsAdmin"])
            {
                TempData["Error"] = "Đăng nhập không hợp lệ, hãy đăng nhập lại.";
                return RedirectToAction("Login", "Home", new { area = "" });
            }

            var shippingRates = db.ShippingRates.Include(s => s.Station).Include(s => s.Station1);
            return View(shippingRates.ToList());
        }

        // GET: Admin/ShippingRates/Create
        public ActionResult Create()
        {
            if (Session["StaffID"] == null || !(bool)Session["IsAdmin"])
            {
                TempData["Error"] = "Đăng nhập không hợp lệ, hãy đăng nhập lại.";
                return RedirectToAction("Login", "Home", new { area = "" });
            }

            ViewBag.ReceivingStation = new SelectList(db.Stations, "StationID", "StationName");
            ViewBag.SendingStation = new SelectList(db.Stations, "StationID", "StationName");
            return View();
        }

        // POST: Admin/ShippingRates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SendingStation,ReceivingStation,MinPrice,PricePerKg,MinPriceForHVO,PricePerKgForHVO")] ShippingRate shippingRate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.ShippingRates.Add(shippingRate);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                ViewBag.Error = "Không thể thêm, tuyến đường này đã tồn tại";
            }
            
            ViewBag.ReceivingStation = new SelectList(db.Stations, "StationID", "StationName", shippingRate.ReceivingStation);
            ViewBag.SendingStation = new SelectList(db.Stations, "StationID", "StationName", shippingRate.SendingStation);
            return View(shippingRate);
        }

        // GET: Admin/ShippingRates/Edit/5
        public ActionResult Edit(string SendingID, string ReceivingID)
        {
            if (Session["StaffID"] == null || !(bool)Session["IsAdmin"])
            {
                TempData["Error"] = "Đăng nhập không hợp lệ, hãy đăng nhập lại.";
                return RedirectToAction("Login", "Home", new { area = "" });
            }

            if (String.IsNullOrEmpty(SendingID) || String.IsNullOrEmpty(ReceivingID))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingRate shippingRate = db.ShippingRates.FirstOrDefault(s => s.SendingStation.Contains(SendingID) && s.ReceivingStation.Contains(ReceivingID));
            if (shippingRate == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReceivingStation = new SelectList(db.Stations, "StationID", "StationName", shippingRate.ReceivingStation);
            ViewBag.SendingStation = new SelectList(db.Stations, "StationID", "StationName", shippingRate.SendingStation);
            return View(shippingRate);
        }

        // POST: Admin/ShippingRates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SendingStation,ReceivingStation,MinPrice,PricePerKg,MinPriceForHVO,PricePerKgForHVO")] ShippingRate shippingRate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(shippingRate).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi! Ngoại lệ sảy ra khi cập nhật:\n" + ex.ToString();
            }

            shippingRate.Station1 = db.Stations.FirstOrDefault(s => s.StationID == shippingRate.SendingStation);
            shippingRate.Station = db.Stations.FirstOrDefault(s => s.StationID == shippingRate.ReceivingStation);
            ViewBag.ReceivingStation = new SelectList(db.Stations, "StationID", "StationName", shippingRate.ReceivingStation);
            ViewBag.SendingStation = new SelectList(db.Stations, "StationID", "StationName", shippingRate.SendingStation);
            return View(shippingRate);
        }

        // GET: Admin/ShippingRates/Delete/5
        public ActionResult Delete(string SendingID, string ReceivingID)
        {
            if (Session["StaffID"] == null || !(bool)Session["IsAdmin"])
            {
                TempData["Error"] = "Đăng nhập không hợp lệ, hãy đăng nhập lại.";
                return RedirectToAction("Login", "Home", new { area = "" });
            }

            if (String.IsNullOrEmpty(SendingID) || String.IsNullOrEmpty(ReceivingID))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingRate shippingRate = db.ShippingRates.FirstOrDefault(s => s.SendingStation.Contains(SendingID) && s.ReceivingStation.Contains(ReceivingID));
            if (shippingRate == null)
            {
                return HttpNotFound();
            }
            return View(shippingRate);
        }

        // POST: Admin/ShippingRates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string SendingID, string ReceivingID)
        {
            ShippingRate shippingRate = db.ShippingRates.FirstOrDefault(s => s.SendingStation.Contains(SendingID) && s.ReceivingStation.Contains(ReceivingID));
            try
            {
                db.ShippingRates.Remove(shippingRate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể xóa do tuyến đường này đã được sử dụng";
                return View(shippingRate);
            }
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

using System;
using System.Collections;
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
    public class StationsController : Controller
    {
        private DeliveryDatabaseEntities db = new DeliveryDatabaseEntities();

        // GET: Admin/Stations
        public ActionResult Index()
        {
            if (Session["StaffID"] == null || !(bool)Session["IsAdmin"])
            {
                TempData["Error"] = "Đăng nhập không hợp lệ, hãy đăng nhập lại.";
                return RedirectToAction("Login", "Home", new { area = "" });
            }

            var listStations = db.Stations.Where(s => !string.IsNullOrEmpty(s.StationID.Trim()));

            return View(listStations);
        }

        // GET: Admin/Stations/Details/5
        public ActionResult Details(string id)
        {
            if (Session["StaffID"] == null || !(bool)Session["IsAdmin"])
            {
                TempData["Error"] = "Đăng nhập không hợp lệ, hãy đăng nhập lại.";
                return RedirectToAction("Login", "Home", new { area = "" });
            }

            if (String.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Station station = db.Stations.Find(id);
            if (station == null)
            {
                return HttpNotFound();
            }
            return View(station);
        }

        // GET: Admin/Stations/Create
        public ActionResult Create()
        {
            if (Session["StaffID"] == null || !(bool)Session["IsAdmin"])
            {
                TempData["Error"] = "Đăng nhập không hợp lệ, hãy đăng nhập lại.";
                return RedirectToAction("Login", "Home", new { area = "" });
            }

            return View();
        }

        // POST: Admin/Stations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StationID,StationName,Address,IsStation,IsAdmin,IsDriver")] Station station)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Stations.Add(station);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(station);
                }
            }
            catch
            {
                return View(station);
            }
        }

        // GET: Admin/Stations/Edit/5
        public ActionResult Edit(string id)
        {
            if (Session["StaffID"] == null || !(bool)Session["IsAdmin"])
            {
                TempData["Error"] = "Đăng nhập không hợp lệ, hãy đăng nhập lại.";
                return RedirectToAction("Login", "Home", new { area = "" });
            }

            if (String.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Station station = db.Stations.Find(id);
            if (station == null)
            {
                return HttpNotFound();
            }
            return View(station);
        }

        // POST: Admin/Stations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StationID,StationName,Address,IsStation,IsAdmin,IsDriver")] Station station)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(station).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(station);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi! Ngoại lệ sảy ra khi cập nhật:\n" + ex.ToString();
                return View(station);
            }
        }

        // GET: Admin/Stations/Delete/5
        public ActionResult Delete(string id)
        {
            if (Session["StaffID"] == null || !(bool)Session["IsAdmin"])
            {
                TempData["Error"] = "Đăng nhập không hợp lệ, hãy đăng nhập lại.";
                return RedirectToAction("Login", "Home", new { area = "" });
            }

            if (String.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Station station = db.Stations.Find(id);
            if (station == null)
            {
                return HttpNotFound();
            }
            return View(station);
        }

        // POST: Admin/Stations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Station station = db.Stations.Find(id);
            try
            {
                db.Stations.Remove(station);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Error = "Không thể xóa đơn vị này do đơn vị đã có lịch sử hoạt động trong hệ thống.";
                return View(station);
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

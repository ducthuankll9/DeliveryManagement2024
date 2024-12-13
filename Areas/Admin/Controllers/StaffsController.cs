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
    public class StaffsController : Controller
    {
        private DeliveryDatabaseEntities db = new DeliveryDatabaseEntities();

        // GET: Admin/Staffs
        public ActionResult Index()
        {
            //TODO: remove
            Session["DARKLAYOUT"] = "NO";
            Session["UserName"] = "UserNameTEST";

            var staffs = db.Staffs.Include(s => s.Station);
            return View(staffs.ToList());
        }

        // GET: Admin/Staffs/Details/5
        public ActionResult Details(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // GET: Admin/Staffs/Create
        public ActionResult Create()
        {
            Staff newStaff = new Staff();
            String strYear = DateTime.Now.Year.ToString();

            //generate new ID
            String newID = "NV";
            var listStaffs = db.Staffs.Include(i => i.Station);
            listStaffs = listStaffs.Where(w => w.StaffID.Contains(strYear)).OrderByDescending(o => o.StaffID);
            if (listStaffs.Count() > 0)
            {
                String strId = listStaffs.FirstOrDefault().StaffID.Substring(2);
                int intId;
                if (Int32.TryParse(strId, out intId))
                {
                    intId++;
                }
                newID += intId;
            }
            else
            {
                newID += strYear + "0001";
            }

            //check new ID
            Staff checkStaff = db.Staffs.Find(newID);
            if (checkStaff == null)
            {
                newStaff.StaffID = newID;
            }
            else
            {
                ViewBag.Error = "Lỗi không xác định khi tự động khởi tạo mã nhân viên mới. Vui lòng tạo thủ công";
                newStaff.StaffID = "NV" + strYear;
            }

            ViewBag.StationID = new SelectList(db.Stations, "StationID", "StationName");
            return View(newStaff);
        }

        // POST: Admin/Staffs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StaffID,Password,Fullname,Male,Birthday,StationID")] Staff staff)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Staffs.Add(staff);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "Lỗi khi thu thập dữ liệu";
                    ViewBag.StationID = new SelectList(db.Stations, "StationID", "StationName");
                    return View(staff);
                }
            }
            catch
            {
                ViewBag.Error = "Lỗi! Không thể thêm nhân viên với mã " + staff.StaffID + ". Hãy thử lại với mã khác";
                ViewBag.StationID = new SelectList(db.Stations, "StationID", "StationName");
                return View(staff);
            }
        }

        // GET: Admin/Staffs/Edit/5
        public ActionResult Edit(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            ViewBag.StationID = new SelectList(db.Stations, "StationID", "StationName", staff.StationID);
            return View(staff);
        }

        // POST: Admin/Staffs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StaffID,Password,Fullname,Male,Birthday,StationID")] Staff staff)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(staff).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.Error = "Lỗi khi thu thập dữ liệu";
                ViewBag.StationID = new SelectList(db.Stations, "StationID", "StationName", staff.StationID);
                return View(staff);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi! Ngoại lệ sảy ra khi cập nhật:\n" + ex.ToString();
                ViewBag.StationID = new SelectList(db.Stations, "StationID", "StationName", staff.StationID);
                return View(staff);
            }
        }

        // GET: Admin/Staffs/Delete/5
        public ActionResult Delete(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // POST: Admin/Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Staff staff = db.Staffs.Find(id);
            try
            {
                //TODO: Add delete logic here
                db.Staffs.Remove(staff);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Đã sảy ra lỗi khi xóa nhân viên " + staff.StaffID + "/" + staff.Fullname + ". Hãy thử lại sau\n" + ex.Message;
                return View(staff);
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DeliveryManagement.Helper;
using DeliveryManagement.Models;

namespace DeliveryManagement.Areas.Drivers.Controllers
{
    public class LinehaulsController : Controller
    {
        private DeliveryDatabaseEntities db = new DeliveryDatabaseEntities();

        // GET: Drivers/Linehauls
        public ActionResult Index()
        {
            if (Session["StaffID"] == null || !(bool)Session["IsDriver"])
            {
                TempData["Error"] = "Đăng nhập không hợp lệ, hãy đăng nhập lại.";
                return RedirectToAction("Login", "Home", new { area = "" });
            }

            string userID = Session["StaffID"].ToString();

            var linehauls = db.Linehauls.Include(l => l.Staff).Include(l => l.Staff1).Include(l => l.Vehicle).Where(l => l.Driver.Contains(userID)).OrderByDescending(l => l.LinehaulID); ;
            return View(linehauls.ToList());
        }

        public ActionResult ReceiveLinehaul()
        {
            if (Session["StaffID"] == null || !(bool)Session["IsDriver"])
            {
                TempData["Error"] = "Đăng nhập không hợp lệ, hãy đăng nhập lại.";
                return RedirectToAction("Login", "Home", new { area = "" });
            }

            var linehauls = db.Linehauls.Include(l => l.Staff).Include(l => l.Staff1).Include(l => l.Vehicle).Where(l => !string.IsNullOrEmpty(l.Seal) && string.IsNullOrEmpty(l.Driver)).OrderByDescending(l => l.LinehaulID);
            return View(linehauls.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReceiveLinehaul(string LinehaulID)
        {
            Linehaul check = db.Linehauls.Find(LinehaulID);
            if(check == null)
            {
                TempData["Error"] = "Mã chuyến xe không đúng, hãy kiểm tra lại";
            }
            else if(string.IsNullOrWhiteSpace(check.Seal))
            {     
                TempData["Error"] = "Chuyến xe " + LinehaulID + " chưa được khóa niêm phong, hãy thử lại sau";
            }
            else if (!string.IsNullOrWhiteSpace(check.Driver))
            {
                Staff driver = db.Staffs.Find(check.Driver);
                TempData["Error"] = "Chuyến xe " + LinehaulID + " đã được nhận bởi tài xế [" + driver.StaffID + "/" + driver.Fullname + "], bạn không thể nhận chuyến này";
            }
            else
            {
                string userID = Session["StaffID"] + "";
                try
                {
                    string sql = "UPDATE [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TableLinehaul + "] "
                                            + "SET " + Constants.DB_Linehaul_Driver + " = @Value1 "
                                            + "WHERE " + Constants.DB_Linehaul_ID + " = @ValueID";
                    int rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                        new SqlParameter("@Value1", userID),
                                        new SqlParameter("@ValueID", LinehaulID));

                    return RedirectToAction("Index");
                }
                catch
                {
                    TempData["Error"] = "Lỗi máy chủ! Nhận chuyến xe không thành công, hãy thử lại.";
                }
            }

            var linehauls = db.Linehauls.Include(l => l.Staff).Include(l => l.Staff1).Include(l => l.Vehicle).Where(l => !string.IsNullOrEmpty(l.Seal) && string.IsNullOrEmpty(l.Driver)).OrderByDescending(l => l.LinehaulID);
            return View(linehauls.ToList());
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

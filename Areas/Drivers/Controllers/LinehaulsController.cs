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
                    int rowsAffected = db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql,
                                        new SqlParameter("@Value1", userID),
                                        new SqlParameter("@ValueID", LinehaulID));

                    // Add History
                    string currentStationId = Session["StationID"].ToString();
                    var listOrder = db.LINEHAUL_ORDER.Where(o => o.LinehaulID.Contains(LinehaulID));
                    foreach(var order in listOrder)
                    {
                        Order_Status checkStt = db.Order_Status.FirstOrDefault(s => s.OrderID.Contains(order.OrderID) && s.StationID.Contains(currentStationId) && s.StatusID.Contains(Constants.Value_Status_Transiting));
                        if (checkStt == null)
                        {
                            //Order_Status status = new Order_Status(order.OrderID, Constants.Value_Status_Transiting, currentStationId, System.DateTime.Now);
                            try
                            {
                                //db.Order_Status.Add(status);
                                //db.SaveChanges();
                                sql = "INSERT INTO [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TableOrderStatus + "] "
                                    + "([OrderID] ,[StatusID] ,[StationID] ,[Time]) "
                                    + "VALUES ( @ValueID1, @ValueID2, @ValueID3, @Value1 ) ";
                                rowsAffected = db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql,
                                                    new SqlParameter("@ValueID1", order.OrderID),                       // OrderID
                                                    new SqlParameter("@ValueID2", Constants.Value_Status_Transiting),   // StatusID
                                                    new SqlParameter("@ValueID3", currentStationId),                    // StationID
                                                    new SqlParameter("@Value1", System.DateTime.Now)                    // Time
                                                    );
                            }
                            catch
                            {
                                TempData["Error"] = "Chưa thể cập nhật lịch sử trạng thái \"Transiting\" của đơn hàng " + order.OrderID;
                            }
                        }
                    }
                    var listPackages = db.Linehaul_Package.Where(p => p.LinehaulID.Contains(LinehaulID));
                    foreach (var package in listPackages)
                    {
                        var listOrderPackages = db.Package_Order.Where(o => o.PackageID.Contains(package.PackageID));
                        foreach(var order in listOrderPackages)
                        {
                            Order_Status checkStt = db.Order_Status.FirstOrDefault(s => s.OrderID.Contains(order.OrderID) && s.StationID.Contains(currentStationId) && s.StatusID.Contains(Constants.Value_Status_Transiting));
                            if (checkStt == null)
                            {
                                //Order_Status status = new Order_Status(order.OrderID, Constants.Value_Status_Transiting, currentStationId, System.DateTime.Now);
                                try
                                {
                                    //db.Order_Status.Add(status);
                                    //db.SaveChanges();
                                    sql = "INSERT INTO [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TableOrderStatus + "] "
                                    + "([OrderID] ,[StatusID] ,[StationID] ,[Time]) "
                                    + "VALUES ( @ValueID1, @ValueID2, @ValueID3, @Value1 ) ";
                                    rowsAffected = db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql,
                                                        new SqlParameter("@ValueID1", order.OrderID),                       // OrderID
                                                        new SqlParameter("@ValueID2", Constants.Value_Status_Transiting),   // StatusID
                                                        new SqlParameter("@ValueID3", currentStationId),                    // StationID
                                                        new SqlParameter("@Value1", System.DateTime.Now)                    // Time
                                                        );
                                }
                                catch
                                {
                                    TempData["Error"] = "Chưa thể cập nhật lịch sử trạng thái \"Transiting\" của đơn hàng " + order.OrderID;
                                }
                            }
                        }
                    }

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

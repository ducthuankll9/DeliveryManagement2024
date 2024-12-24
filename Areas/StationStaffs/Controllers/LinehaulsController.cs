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

namespace DeliveryManagement.Areas.StationStaffs.Controllers
{
    public class LinehaulsController : Controller
    {
        private DeliveryDatabaseEntities db = new DeliveryDatabaseEntities();

        // GET: StationStaffs/Linehauls
        public ActionResult Index()
        {
            var linehauls = db.Linehauls.Include(l => l.Staff).Include(l => l.Staff1).Include(l => l.Vehicle);
            return View(linehauls.ToList());
        }

        // GET: StationStaffs/Linehauls/Details/5
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

        // GET: StationStaffs/Linehauls/Create
        public ActionResult Create()
        {
            //ViewBag.Driver = new SelectList(db.Staffs, "StaffID", "Password");
            //ViewBag.Operator = new SelectList(db.Staffs, "StaffID", "Password");
            //ViewBag.VehicleNumber = new SelectList(db.Vehicles, "VehicleNumber", "Type");
            string userID = Session["StaffID"].ToString();
            string newID;

            Random random = new Random();
            bool completeAutoGen = false;
            do
            {
                // generate new ID
                string strY = DateTime.Now.Year.ToString();
                string strM = DateTime.Now.Month.ToString("00");
                string strD = DateTime.Now.Day.ToString("00");
                string strH = DateTime.Now.Hour.ToString("00");
                string strMM = DateTime.Now.Minute.ToString("00");
                string frontID = "L" + strY + strM + strD + strH + strMM;
                newID = frontID + random.Next(100).ToString("00");    //add a random number from 00 to 99

                if(db.Linehauls.Find(newID) == null)
                {
                    try
                    {
                        string sql = "INSERT INTO [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TableLinehaul + "] "
                            + "([LinehaulID] ,[VehicleNumber] ,[NumberOfPackage] ,[NumberOfOrder] ,[Seal] ,[Operator] ,[Driver]) "
                            + "VALUES ( @ValueID, @Value1, @Value2, @Value3, @Value4, @Value5, @Value6 ) ";
                        int rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                            new SqlParameter("@ValueID", newID),            // LinehaulID
                                            new SqlParameter("@Value1", " "),               // VehicleNumber
                                            new SqlParameter("@Value2", 0),                 // NumberOfPackage
                                            new SqlParameter("@Value3", 0),                 // NumberOfOrder
                                            new SqlParameter("@Value4", " "),               // Seal
                                            new SqlParameter("@Value5", userID),            // Operator
                                            new SqlParameter("@Value6", Constants.Value_Staff_Default)   // Driver
                                            );
                    }
                    catch
                    {
                        TempData["Error"] = "Đã sảy ra lỗi khi truy cập máy chủ! Hãy tải lại trang!";
                    }
                    completeAutoGen = true;
                }

            } while (!completeAutoGen);

            return RedirectToAction("LinehaulInfo", new { id = newID });
        }

        // GET
        public ActionResult LinehaulInfo(string id)
        {
            Linehaul_Package linehaul_package;
            Linehaul linehaul = db.Linehauls.Find(id);
            if(linehaul != null)
            {
                ViewBag.NumberOfOrder = linehaul.NumberOfOrder;
                ViewBag.NumberOfPackage = linehaul.NumberOfPackage;
                ViewBag.Seal = linehaul.Seal.Trim();

                linehaul_package = new Linehaul_Package(id);
            }
            else 
            {
                ViewBag.NumberOfOrder = 0;
                ViewBag.NumberOfPackage = 0;
                ViewBag.Seal = "";

                linehaul_package = new Linehaul_Package();
            }

            return View(linehaul_package);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinehaulInfo([Bind(Include = "LinehaulID,PackageID,AddTime")] Linehaul_Package l_package, [Bind(Include = "LinehaulID,OrderID,AddTime")] LINEHAUL_ORDER l_order)
        {
            ViewBag.Seal = "";

            Linehaul linehaul = db.Linehauls.Find(l_package.LinehaulID);
            if(linehaul == null)
            {
                TempData["Error"] = "Lỗi! Chuyến xe này đã bị xóa khỏi máy chủ.";
                return RedirectToAction("Index");
            }
            else
            {
                if (l_package.PackageID.StartsWith("P"))
                {
                    //input is packageID
                    Package checkP = db.Packages.Find(l_package.PackageID);
                    if (checkP != null)
                    {
                        if(checkP.StatusID.Trim() == Constants.Value_Status_Packed || checkP.StatusID.Trim() == Constants.Value_Status_Completed)
                        {
                            l_package.AddTime = DateTime.Now;
                            Linehaul_Package checkLP = db.Linehaul_Package.FirstOrDefault(lp => lp.LinehaulID.Contains(l_package.LinehaulID) && lp.PackageID.Contains(l_package.PackageID));
                            if (checkLP == null)
                            {
                                try
                                {
                                    //db.Linehaul_Package.Add(l_package);
                                    //db.SaveChanges();

                                    //insert data
                                    string sql = "INSERT INTO [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TableLinehaulPackage + "] "
                                            + "([LinehaulID] ,[PackageID] ,[AddTime]) "
                                            + "VALUES ( @ValueID1, @ValueID2, @Value1 ) ";
                                    int rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                                        new SqlParameter("@ValueID1", l_package.LinehaulID),        // LinehaulID
                                                        new SqlParameter("@ValueID2", l_package.PackageID),         // PackageID
                                                        new SqlParameter("@Value1", DateTime.Now));                 // AddTime

                                    // update number of package
                                    sql = "UPDATE [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TableLinehaul + "] "
                                            + "SET " + Constants.DB_Linehaul_NumberOfPackage + " = @Value1 "
                                            + "WHERE " + Constants.DB_Linehaul_ID + " = @ValueID";
                                    rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                                        new SqlParameter("@Value1", linehaul.NumberOfPackage + 1),
                                                        new SqlParameter("@ValueID", l_package.LinehaulID));

                                    ViewBag.NumberOfPackage = linehaul.NumberOfPackage + 1;
                                    TempData["Success"] = "Thêm data thành công!";
                                    return RedirectToAction("LinehaulInfo", new { id = l_package.LinehaulID });
                                }
                                catch
                                {
                                    TempData["Error"] = "Lỗi! Kết nối máy chủ lỗi, không thể cập nhật dữ liệu vừa rồi.";
                                }
                            }
                            else
                            {
                                TempData["Success"] = "MÃ GÓI HÀNG này đã có trên chuyến xe này rồi";
                            }
                        }
                        else
                        {
                            TempData["Error"] = "Lỗi! Gói [" + l_package.PackageID + "] vẫn đang mở để thêm đơn hàng, hãy <hoàn thành> gói hàng trước.";
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Lỗi! Mã [" + l_package.PackageID + "] không đúng, hãy kiểm tra lại.";
                    }
                }
                else
                {
                    //input is orderID
                    Order checkOd = db.Orders.Find(l_order.OrderID);
                    if (checkOd != null)
                    {
                        if (checkOd.OnDelivering)
                        {
                            l_order.AddTime = DateTime.Now;
                            LINEHAUL_ORDER check = db.LINEHAUL_ORDER.FirstOrDefault(lo => lo.LinehaulID.Contains(l_order.LinehaulID) && lo.OrderID.Contains(l_order.OrderID));
                            if (check == null)
                            {
                                try
                                {
                                    //db.LINEHAUL_ORDER.Add(l_order);
                                    //db.SaveChanges();

                                    //insert data
                                    string sql = "INSERT INTO [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TableLinehaulOrder + "] "
                                            + "([LinehaulID] ,[OrderID] ,[AddTime]) "
                                            + "VALUES ( @ValueID1, @ValueID2, @Value1 ) ";
                                    int rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                                        new SqlParameter("@ValueID1", l_order.LinehaulID),      // LinehaulID
                                                        new SqlParameter("@ValueID2", l_order.OrderID),         // OrderID
                                                        new SqlParameter("@Value1", DateTime.Now));             // AddTime

                                    // update number of order
                                    sql = "UPDATE [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TableLinehaul + "] "
                                            + "SET " + Constants.DB_Linehaul_NumberOfOrder + " = @Value1 "
                                            + "WHERE " + Constants.DB_Linehaul_ID + " = @ValueID";
                                    rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                                        new SqlParameter("@Value1", linehaul.NumberOfOrder + 1),
                                                        new SqlParameter("@ValueID", l_package.LinehaulID));

                                    ViewBag.NumberOfOrder = linehaul.NumberOfOrder + 1;
                                    TempData["Success"] = "Thêm data thành công!";
                                    return RedirectToAction("LinehaulInfo", new { id = l_package.LinehaulID });
                                }
                                catch
                                {
                                    TempData["Error"] = "Lỗi! Kết nối máy chủ lỗi, không thể cập nhật dữ liệu vừa rồi.";
                                }
                            }
                            else
                            {
                                TempData["Success"] = "MÃ ĐƠN HÀNG này đã có trên chuyến xe này rồi";
                            }
                        }
                        else
                        {
                            TempData["Error"] = "Lỗi! Đơn hàng [" + l_order.OrderID + "] chưa hoàn tất nhập thông tin, không thể đi cùng chuyến xe này.";
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Lỗi! Mã [" + l_order.OrderID + "] không đúng, hãy kiểm tra lại.";
                    }
                }

                ViewBag.NumberOfOrder = linehaul.NumberOfOrder;
                ViewBag.NumberOfPackage = linehaul.NumberOfPackage;
                return View(l_package);
            }
        }

        public PartialViewResult _ShortLinehaulDetails(string linehaulId)
        {
            Linehaul linehaul = db.Linehauls.Find(linehaulId);
            //Linehaul linehaul = db.Linehauls.Include(l => l.Staff).Include(l => l.Staff1).Include(l => l.Vehicle).FirstOrDefault(l => l.LinehaulID.Contains(linehaulId));
            if (linehaul != null)
            {
                return PartialView(linehaul);
            }
            else
            {
                return PartialView(new Linehaul());
            }
        }

        //GET
        public ActionResult CompleteLinehaul(string id)
        {
            Linehaul linehaul = db.Linehauls.Find(id);
            if (linehaul == null)
            {
                TempData["Error"] = "Lỗi! Chuyến xe này đã bị xóa khỏi máy chủ.";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.VehicleNumber = new SelectList(db.Vehicles, "VehicleNumber", "VehicleNumber", linehaul.VehicleNumber);
                return View(linehaul);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompleteLinehaul([Bind(Include = "LinehaulID,VehicleNumber,Seal")] Linehaul linehaul )
        {
            Linehaul check = db.Linehauls.Find(linehaul.LinehaulID);
            if(check == null)
            {
                TempData["Error"] = "Lỗi! Chuyến xe này đã bị xóa khỏi máy chủ.";
                return RedirectToAction("Index");
            }
            else
            {
                if (string.IsNullOrEmpty(linehaul.Driver))
                {
                    linehaul.Driver = Constants.Value_Staff_Default;
                }

                if (ModelState.IsValidField("VehicleNumber") && ModelState.IsValidField("Seal"))
                {
                    try
                    {
                        string sql = "UPDATE [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TableLinehaul + "] "
                                            + "SET " + Constants.DB_Linehaul_VehicleNumber + " = @Value1 "
                                            + ", " + Constants.DB_Linehaul_Seal + " = @Value2 "
                                            + "WHERE " + Constants.DB_Linehaul_ID + " = @ValueID";
                        int rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                            new SqlParameter("@Value1", linehaul.VehicleNumber),
                                            new SqlParameter("@Value2", linehaul.Seal),
                                            new SqlParameter("@ValueID", linehaul.LinehaulID));

                        return RedirectToAction("LinehaulInfo", new { id = linehaul.LinehaulID });
                    }
                    catch
                    {
                        TempData["Error"] = "Lỗi máy chủ! Chưa thể cập nhật thông tin, hãy thử lại.";
                    }
                }

                ViewBag.VehicleNumber = new SelectList(db.Vehicles, "VehicleNumber", "Type", linehaul.VehicleNumber);
                return View(linehaul);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReopenLinehaul(string LinehaulID)
        {
            Linehaul check = db.Linehauls.Find(LinehaulID);
            if (check == null)
            {
                TempData["Error"] = "Lỗi! Chuyến xe này đã bị xóa khỏi máy chủ.";
                return RedirectToAction("Index");
            }
            else
            {
                try
                {
                    string sql = "UPDATE [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TableLinehaul + "] "
                                        + "SET " + Constants.DB_Linehaul_Seal + " = @Value1 "
                                        + "WHERE " + Constants.DB_Linehaul_ID + " = @ValueID";
                    int rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                        new SqlParameter("@Value1", " "),
                                        new SqlParameter("@ValueID", LinehaulID));
                }
                catch
                {
                    TempData["Error"] = "Lỗi máy chủ! Chưa thể cập nhật thông tin, hãy thử lại.";
                }

                return RedirectToAction("LinehaulInfo", new { id = LinehaulID });
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using DeliveryManagement.Helper;
using DeliveryManagement.Models;

namespace DeliveryManagement.Areas.StationStaffs.Controllers
{
    public class LinehaulItemsController : Controller
    {
        private DeliveryDatabaseEntities db = new DeliveryDatabaseEntities();

        // GET: StationStaffs/LinehaulItems
        public ActionResult Index()
        {
            if (Session["StaffID"] == null || !(bool)Session["IsStation"])
            {
                TempData["Error"] = "Đăng nhập không hợp lệ, hãy đăng nhập lại.";
                return RedirectToAction("Login", "Home", new { area = "" });
            }

            return View();
        }

        public PartialViewResult _ListOrdersInLinehaul(string linehaulId, bool isDetails)
        {
            var listOrders = db.LINEHAUL_ORDER.Include(o => o.Order).Include(o => o.Order.Station2);
            listOrders = listOrders.Where(o => o.LinehaulID.Contains(linehaulId)).OrderBy(o => o.AddTime);

            ViewBag.IsDetails = isDetails;
            return PartialView(listOrders);
        }

        public PartialViewResult _ListPackagesInLinehaul(string linehaulId, bool isDetails)
        {
            var listPackages = db.Linehaul_Package.Include(p => p.Package).Include(p => p.Package.Station);
            listPackages = listPackages.Where(p => p.LinehaulID.Contains(linehaulId)).OrderBy(p => p.AddTime);

            ViewBag.IsDetails = isDetails;
            return PartialView(listPackages);
        }

        public ActionResult DeleteItem(string linehaulId, string itemId)
        {
            if(!string.IsNullOrEmpty(linehaulId) && !string.IsNullOrEmpty(itemId))
            {
                Linehaul linehaul = db.Linehauls.Find(linehaulId);
                if(linehaul == null)
                {
                    TempData["Error"] = "Lỗi! Chuyến xe này đã bị xóa khỏi máy chủ.";
                    return RedirectToAction("Index");
                }
                else
                {
                    if (itemId.StartsWith("P"))
                    {
                        //Delete package
                        Linehaul_Package l_package = db.Linehaul_Package.FirstOrDefault(p => p.LinehaulID.Contains(linehaulId) && p.PackageID.Contains(itemId));
                        if (l_package != null)
                        {
                            try
                            {
                                //update number of package
                                int newNum = linehaul.NumberOfPackage <= 1 ? 0 : linehaul.NumberOfPackage - 1;
                                string sql = "UPDATE [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TableLinehaul + "] "
                                + "SET " + Constants.DB_Linehaul_NumberOfPackage + " = @Value1 "
                                + "WHERE " + Constants.DB_Linehaul_ID + " = @ValueID";
                                int rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                                    new SqlParameter("@Value1", newNum),
                                                    new SqlParameter("@ValueID", linehaulId));

                                //remove package
                                //db.Linehaul_Package.Remove(l_package);
                                //db.SaveChanges();
                                sql = "DELETE FROM [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TableLinehaulPackage + "] "
                                    + "WHERE " + Constants.DB_Linehaul_ID + " = @ValueID1 "
                                    + "AND " + Constants.DB_Package_ID + " = @ValueID2";
                                rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                                    new SqlParameter("@ValueID1", linehaulId),
                                                    new SqlParameter("@ValueID2", itemId));
                            }
                            catch
                            {
                                TempData["Error"] = "Lỗi! Kết nối máy chủ lỗi, không thể cập nhật dữ liệu vừa rồi.";
                            }
                        }
                    }
                    else
                    {
                        //Delete order
                        LINEHAUL_ORDER l_order = db.LINEHAUL_ORDER.FirstOrDefault(o => o.LinehaulID.Contains(linehaulId) && o.OrderID.Contains(itemId));
                        if(l_order != null)
                        {
                            try
                            {
                                //update number of order
                                int newNum = linehaul.NumberOfOrder <= 1 ? 0 : linehaul.NumberOfOrder - 1;
                                string sql = "UPDATE [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TableLinehaul + "] "
                                + "SET " + Constants.DB_Linehaul_NumberOfOrder + " = @Value1 "
                                + "WHERE " + Constants.DB_Linehaul_ID + " = @ValueID";
                                int rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                                    new SqlParameter("@Value1", newNum),
                                                    new SqlParameter("@ValueID", linehaulId));

                                //remove package
                                //db.LINEHAUL_ORDER.Remove(l_order);
                                //db.SaveChanges();
                                sql = "DELETE FROM [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TableLinehaulOrder + "] "
                                    + "WHERE " + Constants.DB_Linehaul_ID + " = @ValueID1 "
                                    + "AND " + Constants.DB_Order_ID + " = @ValueID2";
                                rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                                    new SqlParameter("@ValueID1", linehaulId),
                                                    new SqlParameter("@ValueID2", itemId));
                            }
                            catch
                            {
                                TempData["Error"] = "Lỗi! Kết nối máy chủ lỗi, không thể cập nhật dữ liệu vừa rồi.";
                            }
                        }
                    }
                }
            }

            return RedirectToAction("LinehaulInfo", "Linehauls", new { id = linehaulId });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using DeliveryManagement.Helper;
using DeliveryManagement.Models;

namespace DeliveryManagement.Areas.StationStaffs.Controllers
{
    public class PackageItemsController : Controller
    {
        private DeliveryDatabaseEntities db = new DeliveryDatabaseEntities();

        // GET: StationStaffs/PackageItems
        public ActionResult Index()
        {
            if (Session["StaffID"] == null || !(bool)Session["IsStation"])
            {
                TempData["Error"] = "Đăng nhập không hợp lệ, hãy đăng nhập lại.";
                return RedirectToAction("Login", "Home", new { area = "" });
            }

            return View();
        }

        public PartialViewResult _ListOrderInPackage(string packageId, bool isDetails)
        {
            var listOrders = db.Package_Order.Include(p => p.Order);
            listOrders = listOrders.Where(p => p.PackageID.Contains(packageId)).OrderBy(p => p.AddTime);

            ViewBag.IsDetails = isDetails;
            return PartialView(listOrders);
        }

        public ActionResult DeleteItem(string packageId, string orderId)
        {
            if (Session["StaffID"] == null || !(bool)Session["IsStation"])
            {
                TempData["Error"] = "Đăng nhập không hợp lệ, hãy đăng nhập lại.";
                return RedirectToAction("Login", "Home", new { area = "" });
            }

            if (!string.IsNullOrEmpty(packageId) && !string.IsNullOrEmpty(orderId))
            {
                Package_Order package_order = db.Package_Order.FirstOrDefault(p => p.PackageID.Contains(packageId) && p.OrderID.Contains(orderId));
                if (package_order != null)
                {
                    try
                    {
                        // update new NumberOfOrder & TotalWeight of this package
                        int newNum = package_order.Package.NumberOfOrder - 1;
                        if (newNum < 0)
                        {
                            newNum = 0;
                        }

                        double newWeight = package_order.Package.TotalWeight - package_order.Order.TotalWeight;
                        if (newWeight < 0)
                        {
                            newWeight = 0.0;
                        }

                        string sql = "UPDATE [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TablePackage + "] "
                            + "SET " + Constants.DB_Package_NumOrder + " = @Value1 "
                            + ", " + Constants.DB_Package_Weight + " = @Value2 "
                            + "WHERE " + Constants.DB_Package_ID + " = @ValueID";
                        int rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                            new SqlParameter("@Value1", newNum),
                                            new SqlParameter("@Value2", newWeight),
                                            new SqlParameter("@ValueID", packageId));

                        // remove the order from package
                        db.Package_Order.Remove(package_order);
                        db.SaveChanges();

                        // check if after remove, there is not order in package, reset the ROUTE
                        var check = db.Package_Order.Where(p => p.PackageID.Contains(packageId));
                        if (!check.Any())
                        {
                            sql = "UPDATE [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TablePackage + "] "
                                + "SET " + Constants.DB_Package_Send + " = @Value1 "
                                + ", " + Constants.DB_Package_Receive + " = @Value2 "
                                + "WHERE " + Constants.DB_Package_ID + " = @ValueID";
                            rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                                new SqlParameter("@Value1", Constants.Value_Station_Default),
                                                new SqlParameter("@Value2", Constants.Value_Station_Default),
                                                new SqlParameter("@ValueID", packageId));
                        }
                    }
                    catch
                    {
                        //Cannot remove
                    }
                }
            }

            return RedirectToAction("Packing", "Packages", new { id = packageId });
        }
    }
}
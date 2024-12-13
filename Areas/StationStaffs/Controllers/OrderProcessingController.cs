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
    public class OrderProcessingController : Controller
    {
        private DeliveryDatabaseEntities db = new DeliveryDatabaseEntities();

        public ActionResult ReceivingOrder()
        {
            string currentStation = Session["StationID"].ToString();

            var orders = db.Orders.Include(o => o.Staff).Include(o => o.Station).Include(o => o.Station1).Include(o => o.Station2).Include(o => o.Station3);
            orders = orders.Where(o => o.CurrentStationID.Contains(currentStation));
            //orders = orders.Where(o => o.CurrentStationID.Contains("DVHG"));
            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReceivingOrder([Bind(Include = "OrderID")] Order order)
        {
            string id = order.OrderID;
            string currentStation = Session["StationID"].ToString();

            if (!string.IsNullOrWhiteSpace(id))
            {
                order = db.Orders.Find(id);
                if (order != null)
                {
                    if (!order.CurrentStationID.Contains(currentStation))
                    {
                        // if this order not in Session["StationID"]
                        string sql = "UPDATE [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TableOrder + "] "
                        + "SET " + Constants.DB_Order_CurrentStation + " = @Value1 "
                        + "WHERE " + Constants.DB_Order_ID + " = @ValueID";
                        int rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                                        new SqlParameter("@Value1", currentStation),
                                                        new SqlParameter("@ValueID", id));

                        TempData["Success"] = "Thành công! đơn " + id + " đã được nhận";
                    }
                    else
                    {
                        TempData["Success"] = "Đơn hàng đã ở trạm này rồi";
                    }
                }
                else
                {
                    TempData["Error"] = "Lỗi! Mã đơn hàng [" + id + "] không đúng, hãy kiểm tra lại.";
                    TempData["ErrValue"] = id;
                }
            }

            var orders = db.Orders.Include(o => o.Staff).Include(o => o.Station).Include(o => o.Station1).Include(o => o.Station2).Include(o => o.Station3);
            orders = orders.Where(o => o.CurrentStationID.Contains(currentStation));
            return View(orders.ToList());
        }

        // GET: StationStaffs/OrderProcessing
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Staff).Include(o => o.Station).Include(o => o.Station1).Include(o => o.Station2).Include(o => o.Station3);
            return View(orders.ToList());
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

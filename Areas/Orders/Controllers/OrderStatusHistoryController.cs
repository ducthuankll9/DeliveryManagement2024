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
    public class OrderStatusHistoryController : Controller
    {
        private DeliveryDatabaseEntities db = new DeliveryDatabaseEntities();

        public PartialViewResult _ListStatusOfAnOrder(string orderId)
        {
            var listStt = db.Order_Status.Where(s => s.OrderID.Contains(orderId)).OrderBy(s => s.Time);
            return PartialView(listStt);
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

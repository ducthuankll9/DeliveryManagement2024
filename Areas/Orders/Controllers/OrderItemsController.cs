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
    public class OrderItemsController : Controller
    {
        private DeliveryDatabaseEntities db = new DeliveryDatabaseEntities();

        public PartialViewResult _ListOrderItem(string orderId, bool isDetails)
        {
            var listItems = db.OrderItems.Where(o => o.OrderID.Contains(orderId)).OrderBy(o => o.ItemID);
            ViewBag.IsDetails = isDetails;
            return PartialView(listItems);
        }

        //public PartialViewResult _CreateItem(string orderId)
        //{
        //    OrderItem item = db.OrderItems.Where(i => i.OrderID.Contains(orderId)).OrderByDescending(i => i.ItemID).FirstOrDefault();
        //    if(item == null)
        //    {
        //        item = new OrderItem(orderId, 1);
        //    }
        //    else
        //    {
        //        item = new OrderItem(orderId, item.ItemID + 1);
        //    }

        //    return PartialView(item);
        //}

        ////POST
        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public PartialViewResult _CreateItem([Bind(Include = "OrderID,ItemID,ItemName,Quantity,Weight")] OrderItem orderItem)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            db.OrderItems.Add(orderItem);
        //            db.SaveChanges();
        //        }

        //        ViewBag.Success = "Thêm thành công";
        //        OrderItem newItem = new OrderItem(orderItem.OrderID, orderItem.ItemID + 1);
        //        return null;// PartialView(newItem);
        //    }
        //    catch
        //    {
        //        ViewBag.Error = "Lỗi kết nối! Thêm thông tin hàng hóa không thành công.";
        //        return PartialView(orderItem);
        //    }

            
        //}

        // GET: Orders/OrderItems
        
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

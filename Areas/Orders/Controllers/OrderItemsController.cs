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
        
        public ActionResult Index()
        {
            var orderItems = db.OrderItems.Include(o => o.Order);
            return View(orderItems.ToList());
        }

        // GET: Orders/OrderItems/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderItem orderItem = db.OrderItems.Find(id);
            if (orderItem == null)
            {
                return HttpNotFound();
            }
            return View(orderItem);
        }

        // GET: Orders/OrderItems/Create
        public ActionResult Create()
        {
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "SenderName");
            return View();
        }

        // POST: Orders/OrderItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,ItemID,ItemName,Quantity,Weight")] OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                db.OrderItems.Add(orderItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "SenderName", orderItem.OrderID);
            return View(orderItem);
        }

        // GET: Orders/OrderItems/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderItem orderItem = db.OrderItems.Find(id);
            if (orderItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "SenderName", orderItem.OrderID);
            return View(orderItem);
        }

        // POST: Orders/OrderItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,ItemID,ItemName,Quantity,Weight")] OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "SenderName", orderItem.OrderID);
            return View(orderItem);
        }

        // GET: Orders/OrderItems/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderItem orderItem = db.OrderItems.Find(id);
            if (orderItem == null)
            {
                return HttpNotFound();
            }
            return View(orderItem);
        }

        // POST: Orders/OrderItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            OrderItem orderItem = db.OrderItems.Find(id);
            db.OrderItems.Remove(orderItem);
            db.SaveChanges();
            return RedirectToAction("Index");
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

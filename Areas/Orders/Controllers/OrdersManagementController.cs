using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DeliveryManagement.Helper;
using DeliveryManagement.Models;
using Microsoft.Ajax.Utilities;

namespace DeliveryManagement.Areas.Orders.Controllers
{
    public class OrdersManagementController : Controller
    {
        private DeliveryDatabaseEntities db = new DeliveryDatabaseEntities();

        // GET: Orders/OrdersManagement
        public ActionResult Index()
        {
            Session["StationName"] = "TODO: do this";
            Session["StationID"] = "DVHG";
            Session["StaffName"] = "TODO: do this";
            Session["StaffID"] = "NV20230001";

            var orders = db.Orders.Include(o => o.Staff).Include(o => o.Station).Include(o => o.Station1).Include(o => o.Station2).Include(o => o.Station3);
            return View(orders.ToList());
        }

        // GET: Orders/OrdersManagement/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        public ActionResult Create()
        {
            //CREATE new order
            Random random = new Random();
            bool completeAutoGen = false;

            //generate new ID
            string strY = DateTime.Now.Year.ToString();
            string strM = DateTime.Now.Month.ToString("00");
            string strD = DateTime.Now.Day.ToString("00");
            string strH = DateTime.Now.Hour.ToString("00");
            string strMM = DateTime.Now.Minute.ToString("00");
            string frontID = "" + strY + strM + strD + strH + strMM;
            string newID;
            do
            {
                newID = frontID + random.Next(1000).ToString("D3");    //add a random number from 000 to 999
                if (db.Orders.Find(newID) == null)
                {
                    try
                    {
                        //if newID not exist, add to DB then go to view
                        //TODO: fix sau khi làm LOGIN, nạp stationID và UserID vào
                        Order addingOrder = new Order(newID, Session["StationID"] + "", Session["StaffID"] + "");
                        db.Orders.Add(addingOrder);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "Đã sảy ra lỗi khi truy cập máy chủ! Hãy tải lại trang!\n " + newID + " \n" + ex.ToString();
                    }

                    completeAutoGen = true;
                }
            } while (!completeAutoGen);

            return RedirectToAction("OrderRegistration", new { id = newID });
        }

        // GET: Orders/OrdersManagement/OrderRegistration/[OrderID]
        public ActionResult OrderRegistration(string id)
        {
            //remove ViewBag first
            ViewBag.Error = null;
            Order order = db.Orders.Find(id);
            if (order != null)
            {
                if (order.SenderName == Constants.defaultString)
                {
                    //make default value to blank before send to view
                    order = new Order(id, Session["StationID"] + "", Session["StaffID"] + "", order.OnDelivering);
                }
            }
            else
            {
                ViewBag.Error = "Đã sảy ra lỗi khi khởi tạo đơn hàng, hãy nhấn lại nút <<Tạo đơn>>!";
                ViewBag.BadError = "BADERROR";
                order = new Order();
            }

            ViewBag.FirstStation = new SelectList(db.Stations.Where(s => s.IsStation), "StationID", "StationName");
            ViewBag.LastStation = new SelectList(db.Stations.Where(s => s.IsStation), "StationID", "StationName");
            ViewBag.Transit = new SelectList(db.Stations.Where(s => s.IsStation), "StationID", "StationName");
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult OrderRegistration([Bind(Include = "OrderID,SenderName,SenderAddress,SenderPhone,SenderEmail,ReceiverName,ReceiverAddress,ReceiverPhone,FirstStation,Transit,LastStation,CurrentStationID,Creator,OnDelivering")] Order order)
        public ActionResult OrderRegistration([Bind(Include = "OrderID,SenderName,SenderAddress,SenderPhone,SenderEmail,ReceiverName,ReceiverAddress,ReceiverPhone,FirstStation,Transit,LastStation,Fee,Paid,TotalWeight,CurrentStationID,Creator,OrderPrice,OnDelivering")] Order order)
        {
            //Not binding: Fee,Paid,TotalWeight,OrderPrice,
            //need to generate as default or read from DB
            //Order record = db.Orders.FirstOrDefault(o => o.OrderID.Contains(order.OrderID));
            //if(record == null)
            //{
            //    ViewBag.Error = "Sảy ra lỗi kết nối máy chủ! Chưa thể lưu đơn hàng lên máy chủ, hãy nhấn lại nút <<Tạo đơn>>";
            //    ViewBag.BadError = "BADERROR";
            //}
            //else
            //{
            //    //order.Fee = record.Fee;
            //    //order.Paid = record.Paid;
            //    //order.TotalWeight = record.TotalWeight;
            //    //order.OrderPrice = record.OrderPrice;
            //}

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Items", new { id = order.OrderID.Trim() });
                }
                catch(Exception ex)
                {
                    ViewBag.Error = "Sảy ra lỗi kết nối máy chủ! Chưa thể lưu đơn hàng lên máy chủ, hãy nhấn lại nút <<Tạo đơn>>" + ex.ToString(); 
                    ViewBag.BadError = "BADERROR";
                }
            }
            else
            {
                ViewBag.Error = "Dữ liệu nhập vào lỗi, hãy kiểm tra lại";
            }

            ViewBag.FirstStation = new SelectList(db.Stations.Where(s => s.IsStation), "StationID", "StationName", order.FirstStation);
            ViewBag.LastStation = new SelectList(db.Stations.Where(s => s.IsStation), "StationID", "StationName", order.LastStation);
            ViewBag.Transit = new SelectList(db.Stations.Where(s => s.IsStation), "StationID", "StationName", order.Transit);
            return View(order);
        }

        public ActionResult CancelRegister(string id)
        {
            try
            {
                Order order = db.Orders.Find(id);
                db.Orders.Remove(order);
                db.SaveChanges();
            }
            catch
            {
                TempData["Error"] = "Sảy ra lỗi kết nối máy chủ! Đơn hàng " + id + " chưa thể xóa khỏi máy chủ!";
            }
            return RedirectToAction("Index");
        }

        // GET
        public ActionResult Items(string id)
        {
            OrderItem item = db.OrderItems.Where(i => i.OrderID.Contains(id)).OrderByDescending(i => i.ItemID).FirstOrDefault();
            if (item == null)
            {
                item = new OrderItem(id, 1, 1);
            }
            else
            {
                item = new OrderItem(id, item.ItemID + 1, 1);
            }

            item.Order = db.Orders.Find(id);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Items([Bind(Include = "OrderID,ItemID,ItemName,Quantity,Weight")] OrderItem orderItem)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.OrderItems.Add(orderItem);
                    db.SaveChanges();

                    // update TotalWeight
                    Order order = db.Orders.Find(orderItem.OrderID);
                    double newWeight = (orderItem.ItemID == 1) ? orderItem.Weight : (order.TotalWeight + orderItem.Weight);
                    string sql = "UPDATE [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TableOrder + "] "
                        + "SET " + Constants.DB_Order_Weight + " = @Value1 "
                        + "WHERE " + Constants.DB_Order_ID + " = @ValueID";
                    int rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                        new SqlParameter("@Value1", newWeight),
                                        new SqlParameter("@ValueID", orderItem.OrderID));

                    TempData["Success"] = "Thêm thành công";
                    return RedirectToAction("Items", new { id = orderItem.OrderID });
                }
            }
            catch
            {
                ViewBag.Error = "Lỗi kết nối! Thêm thông tin hàng hóa không thành công.";
            }

            orderItem.Order = db.Orders.Find(orderItem.OrderID);
            return View(orderItem);
        }

        public ActionResult DeleteItems(string orderId, int itemId)
        {
            if(!string.IsNullOrEmpty(orderId) && itemId>0)
            {
                OrderItem item = db.OrderItems.FirstOrDefault(i => i.OrderID.Contains(orderId) && i.ItemID == itemId);
                if(item != null)
                {
                    try
                    {
                        // update TotalWeight
                        double newWeight = item.Order.TotalWeight - item.Weight;
                        if (newWeight < 0.1)
                        {
                            newWeight = 0.1;
                        }
                        string sql = "UPDATE [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TableOrder + "] "
                        + "SET " + Constants.DB_Order_Weight + " = @Value1 "
                        + "WHERE " + Constants.DB_Order_ID + " = @ValueID";
                        int rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                            new SqlParameter("@Value1", newWeight),
                                            new SqlParameter("@ValueID", item.OrderID));

                        db.OrderItems.Remove(item);
                        db.SaveChanges();
                    }
                    catch
                    {
                        //Cannot remove
                    }
                }
            }
            return RedirectToAction("Items", new { id = orderId });
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CaculateCost([Bind(Include = "OrderID,OrderPrice")] Order order)
        {
            // validate OrderPrice
            if (order.OrderPrice == null)
            {
                TempData["Error"] = "Hãy nhập giá trị đơn hàng!";
                //ModelState.AddModelError("OrderPrice", "Đã xảy ra lỗi!");
                return RedirectToAction("Items", new { id = order.OrderID });
            }

            if(order.OrderPrice < 0)
            {
                TempData["Error"] = "Giá trị đơn hàng phải lớn hơn 0đ";
                return RedirectToAction("Items", new { id = order.OrderID });
            }

            // begin update-start
            try
            {
                string sql = "UPDATE [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TableOrder + "] "
                        + "SET " + Constants.DB_Order_Price + " = @Value1 "
                        + "WHERE " + Constants.DB_Order_ID + " = @ValueID";
                int rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                                new SqlParameter("@Value1", order.OrderPrice),
                                                new SqlParameter("@ValueID", order.OrderID));
            }
            catch
            {
                TempData["Error"] = "Lỗi kết nối! Chưa thể gửi thông tin giá trị đơn hàng đến máy chủ.";
                return RedirectToAction("Items", new { id = order.OrderID });
            }
            
            // begin update-end

            // validate Order's Items
            var items = db.OrderItems.Where(i => i.OrderID.Contains(order.OrderID));
            if (!items.Any())
            {
                TempData["Error"] = "Hãy nhập thông tin sản phẩm của đơn hàng! Phải có ít nhất 1 sản phẩm.";
                return RedirectToAction("Items", new { id = order.OrderID });
            }

            return RedirectToAction("ConfirmPaymentStatus", new { id = order.OrderID });
        }

        public double Caculate(string orderId)
        {
            // Read ini file
            InitFileReader reader = new InitFileReader(Server.MapPath("~/App_Data/SET.ini"));

            float iniMinHighValueOD = 0;
            float iniMinStrongOD = 0;
            iniMinHighValueOD = reader.GetFloatValue("PRICE", "HighValueOrder");
            iniMinStrongOD = reader.GetFloatValue("PRICE", "StrongOrder");

            Order order = db.Orders.Find(orderId);
            if (order == null)
            {
                return -1;
            }
            else
            {
                double fee = 0;
                double weight = order.TotalWeight;
                double price = (double)order.OrderPrice;

                ShippingRate rate1, rate2;
                if (string.IsNullOrEmpty(order.Transit.Trim()))
                {
                    rate1 = db.ShippingRates.FirstOrDefault(r => r.SendingStation.Contains(order.FirstStation) && r.ReceivingStation.Contains(order.LastStation));
                    if(rate1 == null)
                    {
                        return -1;
                    }

                    if (weight > iniMinStrongOD || price > iniMinHighValueOD)
                    {
                        fee = weight * (double)rate1.PricePerKgForHVO;
                        if (fee < (double)rate1.MinPriceForHVO)
                        {
                            fee = (double)rate1.MinPriceForHVO;
                        }
                    }
                    else
                    {
                        fee = weight * rate1.PricePerKg;
                        if (fee < rate1.MinPrice)
                        {
                            fee = rate1.MinPrice;
                        }
                    }
                }
                else
                {
                    rate1 = db.ShippingRates.FirstOrDefault(r => r.SendingStation.Contains(order.FirstStation) && r.ReceivingStation.Contains(order.Transit));
                    rate2 = db.ShippingRates.FirstOrDefault(r => r.SendingStation.Contains(order.Transit) && r.ReceivingStation.Contains(order.LastStation));
                    if(rate1==null || rate2==null)
                    {
                        return -1;
                    }

                    double fee1, fee2;
                    if (weight > iniMinStrongOD || price > iniMinHighValueOD)
                    {
                        fee1 = weight * (double)rate1.PricePerKgForHVO;
                        if (fee1 < (double)rate1.MinPriceForHVO)
                        {
                            fee1 = (double)rate1.MinPriceForHVO;
                        }
                        fee2 = weight * (double)rate2.PricePerKgForHVO;
                        if (fee2 < (double)rate2.MinPriceForHVO)
                        {
                            fee2 = (double)rate2.MinPriceForHVO;
                        }
                    }
                    else
                    {
                        fee1 = weight * rate1.PricePerKg;
                        if (fee1 < rate1.MinPrice)
                        {
                            fee1 = rate1.MinPrice;
                        }
                        fee2 = weight * rate2.PricePerKg;
                        if (fee2 < rate2.MinPrice)
                        {
                            fee2 = rate2.MinPrice;
                        }
                    }
                    fee = fee1 + fee2;
                }

                return fee;
            }
        }

        // GET
        public ActionResult ConfirmPaymentStatus(string id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                ViewBag.Error = "Đã sảy ra lỗi khi khởi tạo đơn hàng, hãy nhấn lại nút <<Tạo đơn>>!";
                ViewBag.BadError = "BADERROR";
                order = new Order();
            }
            // begin caculate-start
            if (order.Fee <= 0)
            {
                order.Fee = Caculate(id);
            }
            
            if(order.Fee < 0)
            {
                ViewBag.Error = "Đã sảy ra lỗi! Có thể tuyến đường chưa được thiết lập giá cước vận chuyển, hãy nhập giá thủ công";
                order.Fee = 0;
            }
            // begin caculate-end

            return View(order);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmPaymentStatus([Bind(Include = "OrderID,Fee,Paid")] Order order)
        {
            //SenderName,SenderAddress,SenderPhone,SenderEmail,ReceiverName,ReceiverAddress,ReceiverPhone,FirstStation,Transit,LastStation,
            //CurrentStationID,Creator,
            //,OnDelivering
            if (ModelState.IsValidField("Fee"))
            {
                try
                {
                    string sql = "UPDATE [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TableOrder + "] "
                        + "SET " + Constants.DB_Order_Fee + " = @Value1 "
                        + " , " + Constants.DB_Order_Paid + " = @Value2 "
                        + " , " + Constants.DB_Order_OnDelivering + " = @Value3 "
                        + "WHERE " + Constants.DB_Order_ID + " = @ValueID";
                    int rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                                    new SqlParameter("@Value1", order.Fee),
                                                    new SqlParameter("@Value2", order.Paid),
                                                    new SqlParameter("@Value3", true),
                                                    new SqlParameter("@ValueID", order.OrderID));

                    //db.Entry(order).State = EntityState.Modified;
                    //db.SaveChanges();

                    Order_Status checkStt = db.Order_Status.FirstOrDefault(s => s.OrderID.Contains(order.OrderID) && s.StatusID.Contains(Constants.Value_Status_Created));
                    if (checkStt == null)
                    {
                        // TODO: fix login
                        string currentStationId = Session["StationID"] + "";

                        Order_Status status = new Order_Status(order.OrderID, Constants.Value_Status_Created, currentStationId, System.DateTime.Now);
                        try
                        {
                            db.Order_Status.Add(status);
                            db.SaveChanges();
                        }
                        catch 
                        {
                            TempData["Error"] = "Chưa thể cập nhật lịch sử trạng thái \"Khởi tạo\" của đơn hàng";
                        }
                    }

                    TempData["Success"] = "Thành công";
                    return RedirectToAction("Index");
                }
                catch
                {
                    ViewBag.Error = "Sảy ra lỗi kết nối máy chủ! Chưa thể lưu đơn hàng lên máy chủ, hãy nhấn lại nút <<Tạo đơn>>";
                    ViewBag.BadError = "BADERROR";
                }
            }
            else
            {
                ViewBag.Error = "Dữ liệu nhập vào lỗi, hãy kiểm tra lại";
            }

            Order returnOrder = new Order();
            returnOrder.OrderID = order.OrderID;
            returnOrder.Fee = order.Fee;
            returnOrder.Paid = order.Paid;
            returnOrder.TotalWeight = order.TotalWeight;
            returnOrder.OrderPrice = order.OrderPrice;

            return View(returnOrder);
        }

        
        // GET: Orders/OrdersManagement/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            ViewBag.FirstStation = new SelectList(db.Stations.Where(s => s.IsStation), "StationID", "StationName", order.FirstStation);
            ViewBag.LastStation = new SelectList(db.Stations.Where(s => s.IsStation), "StationID", "StationName", order.LastStation);
            ViewBag.Transit = new SelectList(db.Stations.Where(s => s.IsStation), "StationID", "StationName", order.Transit);
            return RedirectToAction("OrderRegistration", new { id = id });
        }

        // GET: Orders/OrdersManagement/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/OrdersManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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

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
    public class PackagesController : Controller
    {
        private DeliveryDatabaseEntities db = new DeliveryDatabaseEntities();

        // GET: StationStaffs/Packages
        public ActionResult Index()
        {
            var packages = db.Packages.Include(p => p.Status);
            return View(packages.ToList());
        }

        // GET: StationStaffs/Packages/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = db.Packages.Find(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        // GET: StationStaffs/Packages/Create
        public ActionResult Create()
        {
            string userID = Session["StaffID"].ToString();
            // Package package = new Package(DateTime.Now, Constants.Value_Status_Created, 0.1, userID, Constants.Value_Station_Default, Constants.Value_Station_Default);

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
                string frontID = "P" + strY + strM + strD + strH + strMM;

                newID = frontID + random.Next(100).ToString("00");    //add a random number from 00 to 99
                if(db.Packages.Find(newID) == null)
                {
                    try
                    {
                        //package.PackageID = newID;
                        //db.Packages.Add(package);
                        //db.SaveChanges();

                        int defInt = 0;
                        double defDbl = 0.0d;
                        string sql = "INSERT INTO [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TablePackage + "] "
                            + "([PackageID] ,[CreateTime] ,[CompleteTime] ,[NumberOfOrder] ,[StatusID] ,[TotalWeight] ,[Packer] ,[SendingStation] ,[ReceivingStation]) "
                            + "VALUES ( @ValueID, @Value1, NULL, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7 ) ";
                        int rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                            new SqlParameter("@ValueID", newID),            // PackageID
                                            new SqlParameter("@Value1", DateTime.Now),      // CreateTime
                                            new SqlParameter("@Value2", defInt),            // NumberOfOrder
                                            new SqlParameter("@Value3", Constants.Value_Status_Created),    // StatusID
                                            new SqlParameter("@Value4", defDbl),            // TotalWeight
                                            new SqlParameter("@Value5", userID),            // Packer
                                            new SqlParameter("@Value6", Constants.Value_Station_Default),   // SendingStation
                                            new SqlParameter("@Value7", Constants.Value_Station_Default)    // ReceivingStation
                                            );
                    } 
                    catch 
                    {
                        TempData["Error"] = "Đã sảy ra lỗi khi truy cập máy chủ! Hãy tải lại trang!";
                    }
                    completeAutoGen = true;
                }

            } while (!completeAutoGen);

            return RedirectToAction("Packing", new { id = newID });
        }

        public ActionResult Packing(string id)
        {
            Package_Order package_Order;
            Package package = db.Packages.Find(id);
            if (!string.IsNullOrEmpty(package.StatusID))
            {
                if (package.StatusID.Trim() == Constants.Value_Status_Packed || package.StatusID.Trim() == Constants.Value_Status_Completed)
                {
                    ViewBag.IsPacked = true;
                }
                else
                {
                    ViewBag.IsPacked = false;
                }
                package_Order = new Package_Order(id);
            }
            else
            {
                package_Order = new Package_Order();
            }
            
            
            // TODO: Kiểm tra package status đã Packed hoặc Completed chưa,
            //              TRUE: thì view khóa không cho thêm order,
            //                  có nút REOPEN: chuyển lại package sang status "Packing" và load lại Action này (Action sẽ có đoạn kiểm tra và load lại theo case false)
            //              FALSE: trực tiếp mở view với 1 form gồm thẻ PackageID hidden và input OrderID 
            //                  ấn submit >> kiểm tra package đã tạo hành trình chưa,
            //                      nếu chưa sẽ tạo hành trình dựa trên FirstS>Transit (hoặc FirstS>LastS) và cập nhật status "Packing" cho package
            //                          kiểm tra order tồn tại chưa, chưa thì thêm value vào table Package_Order và cập nhật status packing cho order
            //                      còn nếu rồi thì báo order đã tồn tại trong package (Error)
            //      Form có 1 thẻ submit khác dẫn đến action KẾT THÚC (CompletePackage): chuyển package sang status "Packed"

            return View(package_Order);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Packing([Bind(Include = "PackageID,OrderID,AddTime")] Package_Order package_order)
        {
            if (ModelState.IsValidField("OrderID"))
            {
                package_order.AddTime = DateTime.Now;
                try
                {
                    // check the ROUTE
                    Order order = db.Orders.Find(package_order.OrderID);
                    string thisStation = "", nextStation = "";

                    if (order.CurrentStationID == order.FirstStation)
                    {
                        //if order now in the first station
                        thisStation = order.FirstStation;
                        if (string.IsNullOrWhiteSpace(order.Transit.Trim()))
                        {
                            nextStation = order.LastStation;
                        }
                        else
                        {
                            nextStation = order.Transit;
                        }
                    }
                    else if (order.CurrentStationID == order.Transit)
                    {
                        //if order now in the transit station
                        thisStation = order.Transit;
                        nextStation = order.LastStation;
                    }
                    else if (order.CurrentStationID == order.LastStation)
                    {
                        //if order has come to the last station
                        TempData["Error"] = "Không thể thêm đơn hàng này! Hành trình của đơn hàng không phù hợp, vui lòng kiểm tra lại hoặc thêm vào gói hàng khác";
                    }
                    else
                    {
                        //if order now in the wrong station
                        TempData["Warning"] = "Đơn hàng này chưa đúng lộ trình, hãy cập nhật lại hành trình mới nếu cần thiết";
                        thisStation = order.CurrentStationID;
                        if (string.IsNullOrWhiteSpace(order.Transit.Trim()))
                        {
                            nextStation = order.LastStation;
                        }
                        else
                        {
                            nextStation = order.Transit;
                        }
                    }

                    Package package = db.Packages.Find(package_order.PackageID);
                    if(package.SendingStation == Constants.Value_Station_Default || package.ReceivingStation == Constants.Value_Station_Default)
                    {
                        string sql = "UPDATE [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TablePackage + "] "
                                + "SET " + Constants.DB_Package_Send + " = @Value1 "
                                + ", " + Constants.DB_Package_Receive + " = @Value2 "
                                + "WHERE " + Constants.DB_Package_ID + " = @ValueID";
                        int rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                            new SqlParameter("@Value1", thisStation),
                                            new SqlParameter("@Value2", nextStation),
                                            new SqlParameter("@ValueID", package_order.PackageID));

                        package.SendingStation = thisStation;
                        package.ReceivingStation = nextStation;
                    }

                    if(thisStation == package.SendingStation && nextStation == package.ReceivingStation)
                    {
                        // the order is correct with package route
                        db.Package_Order.Add(package_order);
                        db.SaveChanges();

                        // update weight and num order
                        string sql = "UPDATE [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TablePackage + "] "
                                + "SET " + Constants.DB_Package_NumOrder + " = @Value1 "
                                + ", " + Constants.DB_Package_Weight + " = @Value2 "
                                + "WHERE " + Constants.DB_Package_ID + " = @ValueID";
                        int rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                            new SqlParameter("@Value1", package.NumberOfOrder + 1),
                                            new SqlParameter("@Value2", package.TotalWeight + order.TotalWeight),
                                            new SqlParameter("@ValueID", package_order.PackageID));

                        TempData["Success"] = "Thành công!";
                        return RedirectToAction("Packing", new { id = package_order.PackageID });
                    }
                    else
                    {
                        TempData["Error"] = "Hành trình của đơn hàng không phù hợp với gói hàng, hãy đóng vào gói khác";
                    }
                }
                catch
                {
                    TempData["Error"] = "Lỗi! Mã đơn hàng [" + package_order.OrderID + "] không tồn tại, hãy kiểm tra lại.";
                }
            }
            else
            {
                TempData["Error"] = "Lỗi! Mã đơn hàng [" + package_order.OrderID + "] không đúng, hãy kiểm tra lại.";
            }

            return View(package_order);
        }

        public PartialViewResult _ShortPackageDetails(string packageId)
        {
            Package package = db.Packages.Find(packageId);
            if (package != null)
            {
                return PartialView(package);
            }
            else
            {
                return PartialView(new Package());
            }
        }

        // GET: StationStaffs/Packages/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = db.Packages.Find(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            ViewBag.Packer = new SelectList(db.Staffs, "StaffID", "Password", package.Packer);
            ViewBag.ReceivingStation = new SelectList(db.Stations, "StationID", "StationName", package.ReceivingStation);
            ViewBag.SendingStation = new SelectList(db.Stations, "StationID", "StationName", package.SendingStation);
            ViewBag.StatusID = new SelectList(db.Status, "StatusID", "StatusName", package.StatusID);
            return View(package);
        }

        // POST: StationStaffs/Packages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PackageID,CreateTime,CompleteTime,NumberOfOrder,StatusID,TotalWeight,Packer,SendingStation,ReceivingStation")] Package package)
        {
            if (ModelState.IsValid)
            {
                db.Entry(package).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Packer = new SelectList(db.Staffs, "StaffID", "Password", package.Packer);
            ViewBag.ReceivingStation = new SelectList(db.Stations, "StationID", "StationName", package.ReceivingStation);
            ViewBag.SendingStation = new SelectList(db.Stations, "StationID", "StationName", package.SendingStation);
            ViewBag.StatusID = new SelectList(db.Status, "StatusID", "StatusName", package.StatusID);
            return View(package);
        }

        // GET: StationStaffs/Packages/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = db.Packages.Find(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        // POST: StationStaffs/Packages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Package package = db.Packages.Find(id);
            db.Packages.Remove(package);
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

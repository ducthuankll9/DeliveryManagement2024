using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
            //ViewBag.Packer = new SelectList(db.Staffs, "StaffID", "Password");
            //ViewBag.ReceivingStation = new SelectList(db.Stations, "StationID", "StationName");
            //ViewBag.SendingStation = new SelectList(db.Stations, "StationID", "StationName");
            //ViewBag.StatusID = new SelectList(db.Status, "StatusID", "StatusName");
            string userID = Session["StaffID"].ToString();
            Package package = new Package(DateTime.Now, Constants.Value_Status_Created, 0.1, userID, Constants.Value_Station_Default, Constants.Value_Station_Default);

            // generate new ID
            string strY = DateTime.Now.Year.ToString();
            string strM = DateTime.Now.Month.ToString("00");
            string strD = DateTime.Now.Day.ToString("00");
            string strH = DateTime.Now.Hour.ToString("00");
            string strMM = DateTime.Now.Minute.ToString("00");
            string frontID = "P" + strY + strM + strD + strH + strMM;
            string newID;

            Random random = new Random();
            bool completeAutoGen = false;
            do
            {
                newID = frontID + random.Next(100).ToString("00");    //add a random number from 00 to 99
                if(db.Packages.Find(newID) == null)
                {
                    try
                    {
                        package.PackageID = newID;
                        db.Packages.Add(package);
                        db.SaveChanges();
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
            ViewBag.PackageID = id;

            string checkStt = db.Packages.Find(id).StatusID.Trim();
            if (checkStt == Constants.Value_Status_Packed || checkStt == Constants.Value_Status_Completed)
            {
                ViewBag.IsPacked = true;
            }
            else
            {
                ViewBag.IsPacked = false;
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

            return View();
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

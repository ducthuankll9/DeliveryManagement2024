using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows;
using System.Windows.Media;
using DeliveryManagement.Helper;
using DeliveryManagement.Models;
using GemBox.Pdf;

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

        // GET
        public ActionResult Packing(string id)
        {
            Package_Order package_Order;
            Package package = db.Packages.Find(id);
            if (package != null)
            {
                if (package.StatusID.Trim() == Constants.Value_Status_Packed || package.StatusID.Trim() == Constants.Value_Status_Completed)
                {
                    ViewBag.IsPacked = true;
                }
                else
                {
                    ViewBag.IsPacked = false;
                }
                ViewBag.NumberOfOrder = package.NumberOfOrder;

                package_Order = new Package_Order(id);
            }
            else
            {
                ViewBag.NumberOfOrder = 0;
                package_Order = new Package_Order();
            }

            return View(package_Order);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Packing([Bind(Include = "PackageID,OrderID,AddTime")] Package_Order package_order)
        {
            ViewBag.IsPacked = false;

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
                    ViewBag.NumberOfOrder = package.NumberOfOrder;

                    if (package.SendingStation == Constants.Value_Station_Default || package.ReceivingStation == Constants.Value_Station_Default)
                    {
                        string sql = "UPDATE [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TablePackage + "] "
                                + "SET " + Constants.DB_Package_Send + " = @Value1 "
                                + ", " + Constants.DB_Package_Receive + " = @Value2 "
                                + ", " + Constants.DB_Package_StatusID + " = @Value3 "
                                + "WHERE " + Constants.DB_Package_ID + " = @ValueID";
                        int rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                            new SqlParameter("@Value1", thisStation),
                                            new SqlParameter("@Value2", nextStation),
                                            new SqlParameter("@Value3", Constants.Value_Status_Packing),
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompletePackage([Bind(Include = "PackageID")] Package package)
        {
            Package check = db.Packages.Find(package.PackageID);
            if(check == null)
            {
                TempData["Error"] = "Lỗi khi truy cập máy chủ! Không thể tìm thấy gói hàng của bạn, hãy thử lại sau.";
                return RedirectToAction("Index");
            }

            try
            {
                string sql = "UPDATE [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TablePackage + "] "
                                + "SET " + Constants.DB_Package_StatusID + " = @Value1 "
                                + " , " + Constants.DB_Package_CompleteTime + " = @Value2 "
                                + "WHERE " + Constants.DB_Package_ID + " = @ValueID";
                int rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                    new SqlParameter("@Value1", Constants.Value_Status_Packed),
                                    new SqlParameter("@Value2", DateTime.Now),
                                    new SqlParameter("@ValueID", package.PackageID));
            }
            catch
            {
                TempData["Error"] = "Lỗi khi truy cập máy chủ! Chưa thể hoàn thành gói hàng này.";
            }

            return RedirectToAction("Packing", new { id = package.PackageID.Trim() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReopenPackage([Bind(Include = "PackageID")] Package package)
        {
            Package check = db.Packages.Find(package.PackageID);
            if (check == null)
            {
                TempData["Error"] = "Lỗi khi truy cập máy chủ! Không thể tìm thấy gói hàng của bạn, hãy thử lại sau.";
                return RedirectToAction("Index");
            }

            try
            {
                string sql = "UPDATE [" + Constants.DB_DBNAME + "].[dbo].[" + Constants.DB_TablePackage + "] "
                                + "SET " + Constants.DB_Package_StatusID + " = @Value1 "
                                + " , " + Constants.DB_Package_CompleteTime + " = NULL "
                                + "WHERE " + Constants.DB_Package_ID + " = @ValueID";
                int rowsAffected = db.Database.ExecuteSqlCommand(sql,
                                    new SqlParameter("@Value1", Constants.Value_Status_Packing),
                                    new SqlParameter("@ValueID", package.PackageID));
            }
            catch
            {
                TempData["Error"] = "Lỗi khi truy cập máy chủ! Chưa thể hoàn thành gói hàng này.";
            }

            return RedirectToAction("Packing", new { id = package.PackageID.Trim() });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrintPackageBill(string PackageID)
        {
            if (!string.IsNullOrEmpty(PackageID))
            {
                Package check = db.Packages.Find(PackageID);
                if(check == null)
                {
                    TempData["Error"] = "Lỗi khi truy cập máy chủ! Không thể tìm thấy gói hàng của bạn, hãy thử lại sau.";
                    return RedirectToAction("Index");
                }
                else
                {
                    bool bolGenPdf = false;
                    string QRpath = MyQRCodeHelper.GenerateAndSaveQRCode(PackageID);

                    string templatePath = Path.Combine(Server.MapPath("~/Setup/"), Constants.Template_Package);
                    string outputExcelPath = Constants.Path_Excel + PackageID + ".xlsx";
                    string pdfPath = Server.MapPath(Constants.ServerPath_PDF) + PackageID + ".pdf";

                    if (ExcelHelper.CopyExcelFile(templatePath, outputExcelPath))
                    {    
                        bolGenPdf = ExcelHelper.CreatePackageBill(outputExcelPath, check, QRpath, pdfPath);
                    }

                    if (bolGenPdf)
                    {
                        //TODO: print bill, thành công thì RedirectToAction Index

                        //using(PdfDocument pdfDocument = PdfDocument.Load(pdfPath))
                        //{
                        //    using(PrintDocument printDocument = new PrintDocument())
                        //    {
                        //        printDocument.PrinterSettings.PrintFileName = pdfPath;
                        //        printDocument.DocumentName = PackageID;
                        //        printDocument.PrintPage += (sender, e) =>
                        //        {
                        //            pdfDocument.Render(pdfDocument.PageCount - 1, e.PageBounds.X, e.PageBounds.Y, true);
                        //        };

                        //        printDocument.Print();
                        //    }
                        //}


                        //PdfDocument document = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Modify);
                        //using (var printDocument = new System.Drawing.Printing.PrintDocument())
                        //{
                        //    //printDocument.PrinterSettings = printerSetting;
                        //    printDocument.PrinterSettings.PrintFileName = pdfPath;
                        //    PdfPage page = document.Pages[0];
                        //    PdfPage pageNew = new PdfPage();
                        //    printDocument.PrintPage += (sender, e) =>
                        //    {
                        //        //XGraphics graphics = XGraphics.FromPdfPage(document.Pages[0]);
                        //        XSize size = new XSize(page.Width, page.Height);
                        //        XGraphics graphics = XGraphics.FromGraphics(e.Graphics, size);
                        //        XRect rect = new XRect(e.MarginBounds.Left, e.MarginBounds.Top, e.MarginBounds.Width, e.MarginBounds.Height);
                        //        graphics.DrawImage(XImage.FromFile(pdfPath), rect);
                        //    };
                        //    printDocument.Print();
                        //}


                        ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                        using(var document = PdfDocument.Load(pdfPath))
                        {
                            document.Print();
                        }
                    }
                    else
                    {
                        //TODO: có lỗi, báo lỗi lên màn hình Packing
                    }
                }
            }

            // else
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

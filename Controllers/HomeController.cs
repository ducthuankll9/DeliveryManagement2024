using DeliveryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeliveryManagement.Controllers
{
    public class HomeController : Controller
    {
        //  DESKTOP-7TQTNK0\MAYAO
        private DeliveryDatabaseEntities db = new DeliveryDatabaseEntities();

        [HttpGet]
        public ActionResult Login()
        {
            Session.Clear();

            Session["DARKLAYOUT"] = "NO";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string StaffID, string Password)
        {
            if(!string.IsNullOrEmpty(StaffID) && !string.IsNullOrEmpty(Password))
            {
                Staff staff = db.Staffs.Find(StaffID);
                if(staff == null)
                {
                    TempData["Error"] = "Đăng nhập không thành công! Tài khoản " + StaffID + " không tồn tại trong hệ thống.";
                }
                else if(Password != staff.Password)
                {
                    TempData["Error"] = "Đăng nhập không thành công! Mật khẩu bạn nhập không đúng.";
                }
                else
                {
                    Session["StaffName"] = staff.Fullname;
                    Session["StaffID"] = staff.StaffID;
                    Station station = db.Stations.Find(staff.StationID);
                    if(station == null)
                    {
                        TempData["Error"] = "Lỗi máy chủ! Hãy liên hệ đến quản trị viên";
                    }
                    else
                    {
                        Session["StationName"] = station.StationName;
                        Session["StationID"] = station.StationID;
                        Session["IsStation"] = station.IsStation;
                        Session["IsAdmin"] = station.IsAdmin;
                        Session["IsDriver"] = station.IsDriver;

                        if(station.IsAdmin)
                        {
                            return RedirectToAction("Index", "Staffs", new { area = "Admin" });
                        }
                        else if(station.IsStation)
                        {
                            return RedirectToAction("Index", "OrdersManagement", new { area = "Orders" });
                        }
                        else
                        {
                            return RedirectToAction("Index", "Linehauls", new { area = "Drivers" });
                        }
                        
                    }
                }
            }

            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
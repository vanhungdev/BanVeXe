using WebsiteBanVeXe.Common;
using WebsiteBanVeXe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace WebsiteBanVeXe.Controllers
{
    public class CustomerController : Controller
    {
        private BanVeXeDbContext db = new BanVeXeDbContext();
        // GET: Customer
        public ActionResult Login()
        {
            return View("Login");
        }
        [HttpPost]
        public ActionResult login(FormCollection fc)
        {
            String Username = fc["username"];
            string Pass = Mystring.ToMD5(fc["password"]);
            var user_account = db.Users.Where(m => m.access == 1 && m.status == 1 && (m.username == Username));
           
                if (user_account.Count() == 0)
                {
                    ViewBag.error = "Tên Đăng Nhập Không Đúng";
                }
                else
                {
                    var pass_account = db.Users.Where(m => m.access != 1 && m.status == 1 && m.password == Pass);
                    if (pass_account.Count() == 0)
                    {
                        ViewBag.error = "Mật Khẩu Không Đúng";
                    }
                    else
                    {
                    var user = user_account.First();
                    Session.Add(CommonConstants.CUSTOMER_SESSION, user);
                    Session["userName11"] = user.fullname;
                    Session["id"] = user.ID;
                    if (!Response.IsRequestBeingRedirected)
                        Message.set_flash("Đăng nhập thành công ", "success");
                    return Redirect("~/tai-khoan");
                }
                }
            
            ViewBag.sess = Session["Admin_id"];
            return View("Login");

        }
        public void logout()
        {
            Session["userName11"] = "";
            Session[Common.CommonConstants.CUSTOMER_SESSION]="";
            Response.Redirect("~/dang-nhap");
            Message.set_flash("Đăng xuất thành công", "success");
        }
        public ActionResult register()
        {
            return View("register");
        }
        [HttpPost]
        public ActionResult register(user muser, FormCollection fc)
        {
            string uname = fc["uname"];
            string fname = fc["fname"];
            string Pass = Mystring.ToMD5(fc["psw"]);
            string Pass2 = Mystring.ToMD5(fc["repsw"]);
            if (Pass2 != Pass)
            {
                ViewBag.error = "Mật khẩu không khớp";
                return View("loginEndRegister");
            }
            string email = fc["email"];
            string address = fc["address"];
            string phone = fc["phone"];
            if (ModelState.IsValid)
            {
                var Luser = db.Users.Where(m => m.status == 1 && m.username == uname && m.access == 1);
                if (Luser.Count() > 0)
                {
                    ViewBag.error = "Tên Đăng Nhập đã tồn tại";
                    return View("loginEndRegister");
                }
                else
                {
                    muser.img = "defalt.png";
                    muser.password = Pass;
                    muser.username = uname;
                    muser.fullname = fname;
                    muser.email = email;
                    muser.address = address;
                    muser.phone = phone;
                    muser.gender = "nam";
                    muser.access = 1;
                    muser.status = 1;
                    db.Users.Add(muser);
                    db.SaveChanges();
                    Message.set_flash("Đăng ký tài khoản thành công, Đăng nhập ở đây ", "success");
                    return Redirect("~/dang-nhap");
                }

            }
            Message.set_flash("Đăng ký tài khoản thất bai", "danger");
            return View("register");
        }

        public  ActionResult Myaccount()
        {
            user sessionUser = (user)Session[Common.CommonConstants.CUSTOMER_SESSION];
            return View("Myaccount", sessionUser);
        }
        [HttpPost]
        public ActionResult Myaccount(user user)
        {

            Session[Common.CommonConstants.CUSTOMER_SESSION] = "";
            Session.Add(CommonConstants.CUSTOMER_SESSION, user);
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.success = "Cập nhật thành công.";
            return View("Myaccount", user);
        }
        public ActionResult ListOderCus()
        {
            user sessionUser = (user)Session[Common.CommonConstants.CUSTOMER_SESSION];
            var listOrder = db.orders.Where(m => m.email == sessionUser.email || m.phone == sessionUser.phone).OrderByDescending(m=>m.ID).ToList();
            return View("ListOderCus", listOrder);
        }
        public ActionResult orderDetailCus(int id)
        {
            var sigleOrder = db.orders.Find(id);
            return View("orderDetailCus", sigleOrder);
        }
        
    }
}
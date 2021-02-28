using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteBanVeXe.Common;
using WebsiteBanVeXe.Models;

namespace WebsiteBanVeXe.Areas.Admin.Controllers
{


    public class UserController : BaseController
    {
        private BanVeXeDbContext db = new BanVeXeDbContext();

        // GET: Admin/User
        public ActionResult Index()
        {
            var list = db.Users.Where(m => m.status != 0).OrderByDescending(m => m.ID).ToList();
            return View(list);
        }

        // GET: Admin/User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user muser = db.Users.Find(id);
            if (muser == null)
            {
                return HttpNotFound();
            }
            return View(muser);
        }

        // GET: Admin/User/Create
        public ActionResult Create()
        {
            ViewBag.role = db.roles.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( user muser, FormCollection data)
        {
            if (ModelState.IsValid)
            {
                string password1 = data["password1"];
                string password2 = data["password2"];
                string username = muser.username;
                var Luser = db.Users.Where(m => m.status == 1 && m.username == username);
                if (password1!=password2) {ViewBag.error = "PassWord không khớp";}
                if (Luser.Count()>0) { ViewBag.error1 = "Tên Đăng nhâp đã tồn tại";}
                else
                {
                    string pass = Mystring.ToMD5(password1);
                    muser.img = "ádasd";
                    muser.password = pass;
                    muser.address = "";
                    db.Users.Add(muser);
                    db.SaveChanges();
                    Message.set_flash("Tạo user  thành công", "success");
                    return RedirectToAction("Index");
                }
            }
            return View(muser);
        }

        // GET: Admin/User/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user muser = db.Users.Find(id);
            if (muser == null)
            {
                return HttpNotFound();
            }
            ViewBag.role = db.roles.ToList();
            return View(muser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( user muser)
        {
            if (ModelState.IsValid)
            {
                    muser.img = "ádasd";               
                    db.Entry(muser).State = EntityState.Modified;
                    db.SaveChanges();
                Message.set_flash("Cập nhật thành công", "success");
                return RedirectToAction("Index");
            }
            return View(muser);
        }

        //status
        public ActionResult Status(int id)
        {
            user muser = db.Users.Find(id);
            muser.status = (muser.status == 1) ? 2 : 1;
            db.Entry(muser).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Thay đổi trang thái thành công", "success");
            return RedirectToAction("Index");
        }
        //trash
        public ActionResult trash()
        {
            var list = db.Users.Where(m => m.status == 0).ToList();
            return View("Trash", list);
        }
        public ActionResult Deltrash(int id)
        {
            user muser = db.Users.Find(id);
            muser.status = 0;
            db.Entry(muser).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Xóa thành công", "success");
            return RedirectToAction("Index");
        }

        public ActionResult Retrash(int id)
        {
            user muser = db.Users.Find(id);
            muser.status = 2;
            db.Entry(muser).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("khôi phục thành công", "success");
            return RedirectToAction("trash");
        }
        public ActionResult deleteTrash(int id)
        {
            user muser = db.Users.Find(id);
            db.Users.Remove(muser);
            db.SaveChanges();
            Message.set_flash("Đã xóa vĩnh viễn 1 User", "success");
            return RedirectToAction("trash");
        }

    }
}

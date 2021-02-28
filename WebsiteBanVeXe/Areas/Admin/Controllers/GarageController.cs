using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteBanVeXe.Models;

namespace WebsiteBanVeXe.Areas.Admin.Controllers
{
    public class GarageController : Controller
    {
        private BanVeXeDbContext db = new BanVeXeDbContext();
        // GET: Admin/Garage
        public ActionResult Index()
        {
            var list_garage = db.coach.Where(m => m.status != 0).ToList();
            return View(list_garage);
        }

        public ActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Coach coach)
        {
            var list = db.coach.Where(m => m.status == 1).ToList();
            foreach (var item in list)
            {
                if(item.name.ToSlug() == coach.name.ToSlug())
                {
                    Message.set_flash("Tên nhà xe đã tồn tại", "danger");
                    return View("Create");
                }
                else if(item.licensePlate.ToSlug() == coach.licensePlate.ToSlug())
                {
                    Message.set_flash("Biển số xe đã tồn tại", "danger");
                    return View("Create");
                }
            }

            if (ModelState.IsValid)
            {
                HttpPostedFileBase file;
                file = Request.Files["img"];
                string filename = file.FileName.ToString();
                string ExtensionFile = Mystring.GetFileExtension(filename);
                string namefilenew = Mystring.ToSlug(coach.name.ToSlug()) + "." + ExtensionFile;
                var path = Path.Combine(Server.MapPath("~/Public/images/flight"), namefilenew);
                file.SaveAs(path);
                coach.img = namefilenew;
                db.coach.Add(coach);
                Message.set_flash("Thêm nhà xe thành công", "success");
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            Message.set_flash("Thêm vé thất bại", "danger");
            return View("Create");
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coach coach = db.coach.Find(id);
            if (coach == null)
            {
                return HttpNotFound();
            }
            return View(coach);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Coach coach)
        {
            var list = db.coach.Where(m => m.status == 1 && m.id != coach.id).ToList();
            foreach (var item in list)
            {
                if ( item.name.ToSlug() == coach.name.ToSlug())
                {
                    Message.set_flash("Tên nhà xe đã tồn tại", "danger");
                    return View("Create");
                }
                else if (item.licensePlate.ToSlug() == coach.licensePlate.ToSlug())
                {
                    Message.set_flash("Biển số xe đã tồn tại", "danger");
                    return View("Create");
                }
            }
            if (ModelState.IsValid)
            {
                HttpPostedFileBase file;
                string slug = Mystring.ToSlug(coach.name.ToString());
                file = Request.Files["img"];
                string filename = file.FileName.ToString();
                if (filename.Equals("") == false)
                {
                    file = Request.Files["img"];
                    string ExtensionFile = Mystring.GetFileExtension(filename);
                    string namefilenew = Mystring.ToSlug(coach.name.ToSlug()+ "." + ExtensionFile);
                    var path = Path.Combine(Server.MapPath("~/Public/images/flight"), namefilenew);
                    file.SaveAs(path);
                    coach.img = namefilenew;
                }
                db.Entry(coach).State = EntityState.Modified;
                db.SaveChanges();
                Message.set_flash("Sửa thành công", "success");
                return RedirectToAction("Index");
            }
            Message.set_flash("Sửa thất bại", "danger");
            return View("Edit");
        }
        public ActionResult Status(int id)
        {
            Coach coach = db.coach.Find(id);
            coach.status = (coach.status == 1) ? 2 : 1;     
            db.Entry(coach).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Thay đổi trang thái thành công", "success");
            return RedirectToAction("Index");
        }
        //trash
        public ActionResult trash()
        {
            var list = db.coach.Where(m => m.status == 0).ToList();
            return View("Trash", list);
        }
        public ActionResult Deltrash(int id)
        {
            Coach coach = db.coach.Find(id);
            coach.status = 0;
            db.Entry(coach).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Xóa thành công", "success");
            return RedirectToAction("Index");
        }

        public ActionResult Retrash(int id)
        {
            Coach coach = db.coach.Find(id);
            coach.status = 2;        
            db.Entry(coach).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Khôi phục thành Công", "success");
            return RedirectToAction("trash");
        }
    
        public ActionResult deleteTrash(int id)
        {
            Coach coach = db.coach.Find(id);
            db.coach.Remove(coach);
            db.SaveChanges();
            Message.set_flash("Đã xóa vĩnh viễn 1 Nhà xe", "success");
            return RedirectToAction("trash");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteBanVeXe.Common;
using WebsiteBanVeXe.Models;

namespace WebsiteBanVeXe.Areas.Admin.Controllers
{
    [CustomAuthorizeAttribute(RoleID = "SALESMAN")]
    public class TicketsController : BaseController
    {
       
        private BanVeXeDbContext db = new BanVeXeDbContext();

        // GET: Admin/Tickets
        public ActionResult Index()
        {
            return View(db.tickets.Where(m=>m.status != 0).OrderByDescending(m=>m.id).ToList());
        }

        // GET: Admin/Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ticket ticket = db.tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Admin/Tickets/Create
        public ActionResult Create()
        {
            ViewBag.listCoach = db.coach.Where(m => m.status == 1).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ticket ticket)
        {
            Coach coach = db.coach.Find(ticket.garageId);
            if (ModelState.IsValid)
            {
                ticket.code  = "MaVe" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                ticket.licensePlates = coach.licensePlate;
                ticket.CoachName = coach.name;
                ticket.img = coach.img;
                ticket.guestTotal = coach.seatTotal;
                ticket.created_at = DateTime.Now;
                ticket.updated_at = DateTime.Now;
                ticket.sold = 0;
                ticket.day = DateTime.Parse(ticket.departure_date.ToString("MM/dd/yyyy"));
                ticket.created_by = int.Parse(Session["Admin_id"].ToString());
                ticket.updated_by = int.Parse(Session["Admin_id"].ToString());
                db.tickets.Add(ticket);
                Message.set_flash("Thêm vé thành công", "success");
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.listCoach = db.coach.Where(m => m.status == 1).ToList();
            Message.set_flash("Thêm vé thất bại", "danger");
            return View("Create");
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ticket ticket = db.tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.listCoach = db.coach.Where(m => m.status == 1).ToList();
            return View(ticket);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(ticket ticket)   
        {
            Coach coach = db.coach.Find(ticket.garageId);
            if (ModelState.IsValid)
            {
                string slug = Mystring.ToSlug(ticket.name.ToString());
                ticket.licensePlates = coach.licensePlate;
                ticket.CoachName = coach.name;
                ticket.img = coach.img;
                ticket.guestTotal = coach.seatTotal;
                ticket.updated_at = DateTime.Now;
                ticket.day = DateTime.Parse(ticket.departure_date.ToString("MM/dd/yyyy"));
                ticket.updated_by = int.Parse(Session["Admin_id"].ToString());
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                Message.set_flash("Sửa thành công", "success");
                return RedirectToAction("Index");
            }
            ViewBag.listCoach = db.coach.Where(m => m.status == 1).ToList();
            Message.set_flash("Sửa thất bại", "danger");
            return View("Edit");
        }

        public ActionResult Status(int id)
        {
            ticket tickets = db.tickets.Find(id);
            tickets.status = (tickets.status == 1) ? 2 : 1;
            db.Entry(tickets).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Thay đổi trang thái thành công", "success");
            return RedirectToAction("Index");
        }
        //trash
        public ActionResult trash()
        {
            var list = db.tickets.Where(m => m.status == 0).ToList();
            return View("Trash", list);
        }
        public ActionResult Deltrash(int id)
        {
            ticket morder = db.tickets.Find(id);
            morder.status = 0;
            db.Entry(morder).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Xóa thành công", "success");
            return RedirectToAction("Index");
        }

        public ActionResult Retrash(int id)
        {
            ticket morder = db.tickets.Find(id);
            morder.status = 2;
            db.Entry(morder).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Khôi phục thành công", "success");
            return RedirectToAction("trash");
        }
        public ActionResult deleteTrash(int id)
        {
            ticket morder = db.tickets.Find(id);
            db.tickets.Remove(morder);
            db.SaveChanges();
            Message.set_flash("Đã xóa vĩnh viễn 1 Đơn hàng", "success");
            return RedirectToAction("trash");
        }


    }
}

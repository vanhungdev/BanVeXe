using WebsiteBanVeXe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebsiteBanVeXe.Controllers
{
    public class HomeController : Controller
    {
        BanVeXeDbContext db = new BanVeXeDbContext();
        // GET: Home
        public ActionResult Index()
        {
            DateTime date_now = DateTime.Now;
            string date_now1 = date_now.ToString("MM-dd-yyyy");
            DateTime date_now2 = DateTime.Parse(date_now1);
            ViewBag.dateNow = date_now;
            // lay cac chuyen xe trong ngay
            var list = db.tickets.Where(m => m.status == 1 && m.day == date_now2).Take(20).ToList();
            return View(list);
        }
        public ActionResult CoachOfday()
        {
            DateTime date_now = DateTime.Now;
            string date_now1 = date_now.ToString("MM-dd-yyyy");
            DateTime date_now2 = DateTime.Parse(date_now1);
            ViewBag.dateNow = date_now;
            // lay cac chuyen xe trong ngay
            var list = db.tickets.Where(m => m.status == 1 && m.day == date_now2).Take(20).ToList();
            return View("CoachOfDay", list);
        }
        
    }
}
using WebsiteBanVeXe.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebsiteBanVeXe.Controllers
{
    public class SiteController : Controller
    {
        BanVeXeDbContext db = new BanVeXeDbContext();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CoachSearch(FormCollection fc, int? page)
        {
       
            if (page == null) page = 1;
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            string typeTicket = fc["typeticket"];
            int songuoi = int.Parse(fc["songuoi"]);
            ViewBag.songuoi = songuoi;
            //noi khoi hanh
            string departure_address = fc["departure_address"];
            //noi den
            string arrival_address = fc["arrival_address"];
            //ngay khoi hanh
            string departure_date = fc["departure_date"];

            ViewBag.url = "chuyen-Xe";
            //convert sang mm/dd/yy cho may hieu 
            DateTime ngaychay1 = DateTime.ParseExact(departure_date, "d/M/yyyy", CultureInfo.InvariantCulture);
                //định dạng sang mm/dd/yy
            string ngaychay2 = ngaychay1.ToString("MM-dd-yyyy");
            // chuyển sang định dạng date để so sánh
            DateTime ngaychay3 = DateTime.Parse(ngaychay2);

            // viewbag này để hiển thị ra view
            ViewBag.noiXuatPhat = departure_address;
            ViewBag.noiVe = arrival_address;
            ViewBag.ngayXuatPhat = departure_date;
            // neu la ve 2 chieu
            if (typeTicket.Equals("enable"))
            {
                string ngayve = fc["arrival_date"];
                DateTime ngayden1 = DateTime.ParseExact(ngayve, "d/M/yyyy", CultureInfo.InvariantCulture);
                string ngayden2 = ngayden1.ToString("MM-dd-yyyy");
                DateTime ngayden3 = DateTime.Parse(ngayden2);
                //In ngày ra view
                ViewBag.ngayden = ngayve;
                ViewBag.date = ngayden3.ToString("MM-dd-yyyy");

                var list = db.tickets.Where(m => m.departure_address.Contains(departure_address) && m.arrival_address.Contains(arrival_address)).
             Where(m => m.day == ngaychay3).ToList();
                return View("CoachSearchReturn", list.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                //ve 1 chieu
                var list = db.tickets.Where(m => m.departure_address.Contains(departure_address) && m.arrival_address.Contains(arrival_address)).
             Where(m => m.day == ngaychay3).ToList();
                return View("CoachSearchOnway", list.ToPagedList(pageNumber, pageSize));
            }         
        }

        public ActionResult return_ticket(string date,string noiXuatPhat, string noiden)
        {
            DateTime ngayXuatPhat3 = DateTime.Parse(date);
            var list = db.tickets.Where(m => m.departure_address.Contains(noiden) && m.arrival_address.Contains(noiXuatPhat)).
               Where(m => m.day == ngayXuatPhat3).ToList();
            return View("_returnTicket", list);
        }
        public ActionResult AllChuyenXe(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 8;
            ViewBag.url = "chuyen-xe";
            int pageNumber = (page ?? 1);
            ViewBag.breadcrumb = "Tất cả chuyến XuatPhat";
            var list_Coach = db.tickets.Where(m => m.status == 1).ToList();
            return View("allCoach", list_Coach.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult postOftoPic(int? page, string slug)
        {
            if (page == null) page = 1;
            int pageSize = 8;
            var singleC = db.Topics.Where(m => m.status == 1 && m.slug == slug).First();
            ViewBag.nameTopic = slug;
            ViewBag.url = "tin-tuc/" + slug + "";
            int pageNumber = (page ?? 1);
            var listPost = db.Posts.Where(m => m.status == 1 && m.topid == singleC.ID).ToList();
            return View("postOftoPic", listPost.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult AllPost(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 8;

            ViewBag.url = "tin-tuc";
            int pageNumber = (page ?? 1);
            var listPost = db.Posts.Where(m => m.status == 1 ).ToList();
            return View("postOftoPic", listPost.ToPagedList(pageNumber, pageSize));
        }
        
        public ActionResult topic()
        {

            var list = db.Topics.Where(m => m.status == 1).ToList();
            return View("_topic", list);
        }

        public ActionResult postSearch(string keyw, int? page)
        {
            if (page == null) page = 1;
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            ViewBag.url = "tim-kiem-bai-viet?keyw=" + keyw + "";
            @ViewBag.nameTopic = "Tim kiếm từ khóa: " + keyw;
            var list = db.Posts.Where(m => m.title.Contains(keyw) || m.detail.Contains(keyw)).OrderBy(m => m.ID);
            return View("postOftoPic", list.ToList().ToPagedList(pageNumber, pageSize));
        }
        public ActionResult PostDetal(string slug)
        {

            var single = db.Posts.Where(m => m.status == 1 && m.slug == slug).First();
            ViewBag.Bra = single.title;
            return View("PostDetal", single);
        }
        
            public ActionResult CoachDetail(int id)
        {
            var single = db.tickets.Where(m => m.status == 1 && m.id == id).First();
            return View("CoachDetail", single);
        }
        public ActionResult lienHe()
        {
            var single = db.Posts.Where(m => m.status == 1 && m.slug == "lien-he").First();
            return View("PostDetal", single);
        }

        public ActionResult OrderSearch()
        {
       
            return View("OrderSearch");
        }
        [HttpPost]
        public ActionResult OrderSearch(FormCollection fc)
        {
            string keyw = fc["keyw"];
            ViewBag.keyW = keyw;
            var listOrder = db.orders.Where(m => m.email == keyw || m.phone == keyw).OrderByDescending(m=>m.ID).ToList();
            return View("ListOrderSearch", listOrder);
        }

    }
}
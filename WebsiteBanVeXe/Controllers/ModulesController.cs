﻿using WebsiteBanVeXe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebsiteBanVeXe.Controllers
{
    public class ModulesController : Controller
    {
        // GET: Modules
        BanVeXeDbContext db = new BanVeXeDbContext();
        public ActionResult _Header()
        {
            if ( (string)Session["userName11"] !="")
            {
                ViewBag.sessionFullname = Session["userName11"];
            }
            else
            {
               
            }
            return View("_Header");
        }
        public ActionResult _Mainmenu()
        {

            var list = db.menus.Where(m => m.status == 1 && m.parentid == 0).ToList();
            return View("_Mainmenu", list);
        }
        public ActionResult _Footer()
        {
            return View("_Footer");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HMP_ZD.Models;
using HMP_ZD.Utils;

namespace HMP_ZD.Controllers.Admin
{
    public class AdminController : Controller
    {

        /// <summary>
        /// êVãKçÏê¨
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View("~/Views/Admin/Index.cshtml");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HMP_ZD.Models;
using HMP_ZD.Utils;

namespace HMP_ZD.Controllers.Ledger
{
    public class HomeController : Controller
    {
        // GET: Top
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var res = new EmptyResult();
            // 空を戻す
            //return res;
            //return View();

            return RedirectToAction("Index", "Top");
           //return new RedirectResult("~/index.html");
        }
    }
}
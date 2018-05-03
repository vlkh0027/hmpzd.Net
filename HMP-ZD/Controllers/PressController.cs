using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HMP_ZD.Models;
using HMP_ZD.Utils;

namespace HMP_ZD.Controllers
{
    /// <summary>
    /// 掲載誌申込締切
    /// </summary>
    public class PressController : Controller
    {
        /// <summary>
        /// 一覧
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}
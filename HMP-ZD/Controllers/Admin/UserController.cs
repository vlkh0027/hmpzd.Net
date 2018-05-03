using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HMP_ZD.Models;
using HMP_ZD.Utils;
using System.Dynamic;
using EO = System.Dynamic.ExpandoObject;

namespace HMP_ZD.Controllers.Admin
{
    public class UserController : Controller
    {

        /// <summary>
        /// ˆê——‰æ–Ê
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View("~/Views/Admin/User/Index.cshtml");
        }


    }
}
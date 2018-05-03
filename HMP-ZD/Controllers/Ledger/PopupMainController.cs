using HMP_ZD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HMP_ZD.Controllers.Admin.Ledger
{
    public class PopupMainController : Controller
    {
        // GET: PopupMain
        public ActionResult Index()
        {
            var db = new HMPZDDB();

            // データ一式。JOINさせる
            
            return PartialView("~/Views/Shared/Ledger/Info_popup_Main.cshtml");
        }
    }
}
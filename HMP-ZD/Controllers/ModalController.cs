using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HMP_ZD.Models;
using HMP_ZD.Utils;

namespace HMP_ZD.Controllers
{
    public class ModalController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return new EmptyResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteHistory()
        {
            return View();            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Notify()
        {
            return View();            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult UserSelect()
        {
            using (HMPZDDB db = new HMPZDDB())
            {
                //  ユーザ選択の会社DropDown
                ViewBag.CompanyList = db.Organizations.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                               d.Hierarchy == 1)
                                                   .ToList()
                                                   .OrderBy(d => int.Parse(d.ID))
                                                   .Select(d => new SelectListItem()
                                                   {
                                                       Value = d.ID,
                                                       Text = d.CompanyName,
                                                   })
                                                   .ToList();
            }

            return View();            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult VehicleSelect()
        {
            return View();            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerSelect()
        {
            return View();            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult TeamSelect()
        {
            return View();            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CooperativeCompanySelect()
        {
            return View();            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult UrlShare()
        {
            return View();            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerDetail()
        {
            return View();            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult HeaderSelect()
        {
            return View();
        }

    }
}
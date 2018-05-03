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
    /// お知らせ表示ポップアップの部分ビュー
    /// </summary>
    public class InfoPopupController : Controller
    {
        [ChildActionOnly]
        public ActionResult Popup()
        {
            /*
             *  暫定 
             */
            ViewBag.ShowInformation = false;

            //  2週間後の23時59分59秒
            DateTime twoWeeks = DateTime.Now.AddDays(14).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            using(HMPZDDB db = new HMPZDDB())
            {
                //  メンテナンス情報
                var mt = db.Shutdown.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                d.ShutdownStart <= twoWeeks)
                                    .OrderBy(d => d.ShutdownStart)
                                    .Select(d => d.ShutdownReason)
                                    .ToList();

                ViewBag.ShowMaintenance = mt.Any();
                ViewBag.Maintenance = mt.Aggregate("システムメンテナンスの為、下記の日程は電子台帳システムをご利用いただくことができません。<br />",
                                                   (m, n) => m + "<br />" + n);

                //  お知らせ
                var info = db.Information.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                     d.TopFlg == 1);

                foreach(var item in info)
                {
                    //  cookieですでに表示されているIDは表示しない

                    ViewBag.Title = item.Title;
                    ViewBag.InfoBody = item.Contents;
                }
                                                
            }

            return PartialView("/Views/Shared/info_popup.cshtml");
        }
    }
}
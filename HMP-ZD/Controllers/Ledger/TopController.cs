using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HMP_ZD.Models;
using HMP_ZD.Utils;
namespace HMP_ZD.Controllers.Ledger
{
    public class TopController : Controller
    {
        // GET: Top
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {            
            return View("~/Views/Ledger/Top.cshtml");
        }

        /// <summary>
        /// データロード
        /// </summary>
        /// <returns></returns>
        

        [HttpPost]
        public ActionResult New(string param)
        {
            return null;
        }

        /// <summary>
        /// データ入力
        /// </summary>
        private void SetData()
        {
            using(HMPZDDB db = new HMPZDDB())
            {
                #region DropDownList items
                //  確度
                ViewBag.StopItems = db.Accuracy.Where(d => d.DeleteFlg != DBUtil.FLAG_YES)
                                               .OrderBy(d => d.Order)
                                               .Select(d => new SelectListItem()
                                               {
                                                   Value = d.ID.ToString(),
                                                   Text = d.AccuracyName,
                                               })
                                               .ToList();

                //  広告種類
                ViewBag.AdvertisementTypes = db.AdvertisementTypes.Where(d => d.DeleteFlg != DBUtil.FLAG_YES)
                                                                  .OrderBy(d => d.Order)
                                                                  .Select(d => new SelectListItem()
                                                                  {
                                                                      Value = d.ID.ToString(),
                                                                      Text = d.AdvertisementType,
                                                                  })
                                                                  .ToList();
                //  画
                ViewBag.PictureItems = db.Pictures.Where(d => d.DeleteFlg != DBUtil.FLAG_YES)
                                                  .OrderBy(d => d.Order)
                                                  .Select(d => new SelectListItem()
                                                  {
                                                      Value = d.ID.ToString(),
                                                      Text = d.Picture,
                                                  })
                                                  .ToList();
                //  色
                ViewBag.ColorItems = db.Colors.Where(d => d.DeleteFlg != DBUtil.FLAG_YES)
                                              .OrderBy(d => d.Order)
                                              .Select(d => new SelectListItem()
                                              {
                                                  Value = d.ID.ToString(),
                                                  Text = d.Color,
                                              })
                                              .ToList();
                //  ページ
                ViewBag.PageItems = db.Pages.Where(d => d.DeleteFlg != DBUtil.FLAG_YES)
                                            .OrderBy(d => d.Order)
                                            .Select(d => new SelectListItem()
                                            {
                                                Value = d.ID.ToString(),
                                                Text = d.Page,
                                            })
                                            .ToList();

                //  送稿形態
                ViewBag.TransmissionFormItems = db.TransmissionForms.Where(d => d.DeleteFlg != DBUtil.FLAG_YES)
                                                                    .OrderBy(d => d.Order)
                                                                    .Select(d => new SelectListItem()
                                                                    {
                                                                        Value = d.ID.ToString(),
                                                                        Text = d.TransmissionForm,
                                                                    })
                                                                    .ToList();

                ViewBag.DepartmentItems = new List<SelectListItem>();
                ViewBag.ChargeTeamItems = new List<SelectListItem>();
                ViewBag.CustomerIDItems = new List<SelectListItem>();
                ViewBag.VehicleIDItems = new List<SelectListItem>();
                #endregion

                #region Popup data
                //  ユーザ選択の会社DropDown
                ViewBag.CompanyItems = db.Organizations.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                   d.Hierarchy == 1)
                                                       .ToList()
                                                       .OrderBy(d => int.Parse(d.ID))
                                                       .Select(d => new SelectListItem()
                                                       {
                                                           Value = d.ComapnyCode,
                                                           Text = d.CompanyName,
                                                       })
                                                       .ToList();
                #endregion
            }
        }

        /// <summary>
        /// チーム別表示対応
        /// </summary>
        /// <param name="header"></param>
        /// <param name="data"></param>
        private void GridDataForTeam(out object[] header, out object[] data)
        {
            header =
                new[] {
new { name = "Fld1", title = "HEN受付"},
new { name = "Fld2", title = "媒体社申込"},
new { name = "Fld3", title = "担当T"},
new { name = "Fld4", title = "担当部"},
new { name = "Fld5", title = "発売月"},
new { name = "Fld6", title = "発売日"},
new { name = "Fld7", title = "掲載開始日"},
new { name = "Fld8", title = "掲載終了日"},
new { name = "Fld9", title = "媒体社"},
new { name = "Fld10", title = "媒体誌"},
new { name = "Fld11", title = "iメディア媒体社"},
new { name = "Fld12", title = "広告主"},
new { name = "Fld13", title = "件名"},
new { name = "Fld14", title = "純広告：面"},
new { name = "Fld15", title = "純広告：色"},
new { name = "Fld16", title = "純広告：P"},
new { name = "Fld17", title = "TU：面"},
new { name = "Fld18", title = "TU：色"},
new { name = "Fld19", title = "TU：P"},
new { name = "Fld20", title = "仕様"},
new { name = "Fld21", title = "媒体社"},
new { name = "Fld22", title = "納品日"},
new { name = "Fld23", title = "掲載料(G)"},
new { name = "Fld24", title = "掲載料(料率)"},
new { name = "Fld25", title = "掲載料(N)"},
new { name = "Fld26", title = "制作費(G)"},
new { name = "Fld27", title = "制作費(料率)"},
new { name = "Fld28", title = "制作費(N)"},
new { name = "Fld29", title = "媒体収益（N)"},
new { name = "Fld30", title = "引渡建値(G)"},
new { name = "Fld31", title = "戦略原価B(N)"},
new { name = "Fld32", title = "媒体支払額(N)"},
new { name = "Fld33", title = "原価項目"},
new { name = "Fld34", title = "売上金額"},
new { name = "Fld35", title = "支払原価計"},
new { name = "Fld36", title = "営収額"},
new { name = "Fld37", title = "営収率"},
new { name = "Fld38", title = "作成者"},
new { name = "Fld39", title = "作成日"},
new { name = "Fld40", title = "更新日"},

                };

            var list = new List<object>();

            // ダミーデータ            
            for (int i = 1; i <= 1007; i++)
            {
                list.Add(new
                {
                    id = i,
                    Fld1 = "ABCD1" + i,
                    Fld2 = "ABCD2" + i,
                    Fld3 = "ABCD3" + i,
                    Fld4 = "ABCD4" + i,
                    Fld5 = "ABCD5" + i,
                    Fld6 = "ABCD6" + i,
                    Fld7 = "ABCD7" + i,
                    Fld8 = "ABCD8" + i,
                    Fld9 = "ABCD9" + i,
                    Fld10 = "ABCD10" + i,
                    Fld11 = "ABCD11" + i,
                    Fld12 = "ABCD12" + i,
                    Fld13 = "ABCD13" + i,
                    Fld14 = "ABCD14" + i,
                    Fld15 = "ABCD15" + i,
                    Fld16 = "ABCD16" + i,
                    Fld17 = "ABCD17" + i,
                    Fld18 = "ABCD18" + i,
                    Fld19 = "ABCD19" + i,
                    Fld20 = "ABCD20" + i,
                    Fld21 = "ABCD21" + i,
                    Fld22 = "ABCD22" + i,
                    Fld23 = "ABCD23" + i,
                    Fld24 = "ABCD24" + i,
                    Fld25 = "ABCD25" + i,
                    Fld26 = "ABCD26" + i,
                    Fld27 = "ABCD27" + i,
                    Fld28 = "ABCD28" + i,
                    Fld29 = "ABCD29" + i,
                    Fld30 = "ABCD30" + i,
                    Fld31 = "ABCD31" + i,
                    Fld32 = "ABCD32" + i,
                    Fld33 = "ABCD33" + i,
                    Fld34 = "ABCD34" + i,
                    Fld35 = "ABCD35" + i,
                    Fld36 = "ABCD36" + i,
                    Fld37 = "ABCD37" + i,
                    Fld38 = "ABCD38" + i,
                    Fld39 = "ABCD39" + i,
                    Fld40 = "ABCD40" + i,
                    Fld41 = "ABCD41" + i,
                    Fld42 = "ABCD42" + i,
                    Fld43 = "ABCD43" + i,
                    Fld44 = "ABCD44" + i,
                    Fld45 = "ABCD45" + i,
                    Fld46 = "ABCD46" + i,
                    Fld47 = "ABCD47" + i,
                    Fld48 = "ABCD48" + i,
                    Fld49 = "ABCD49" + i,
                    Fld50 = "ABCD50" + i,
                    Fld51 = "ABCD51" + i,
                    Fld52 = "ABCD52" + i,

                });
            }

            data = list.ToArray();

        }

    }
}
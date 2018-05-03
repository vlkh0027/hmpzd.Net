using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HMP_ZD.Models;
using HMP_ZD.Utils;
using System.Dynamic;
using V = HMP_ZD.Utils.ValueUtil;

namespace HMP_ZD.Controllers.Admin
{
    public class AccuracyController : Controller
    {
        #region "一覧"
        /// <summary>
        /// 一覧画面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View("~/Views/Admin/Accuracy/Index.cshtml");
        }

        /// <summary>
        /// GridData
        /// </summary>
        /// <returns></returns>
        public ActionResult GridData()
        {
            if (!Request.IsAjaxRequest())
            {
                return new EmptyResult();
            }

            // 総件数
            var total = 0;

            // 1ページの表示件数。どちらで持つべきか？ひとまずサーバで定義
            var limit = 20;

            // 現状のページ
            var page = 1;
            if (!string.IsNullOrWhiteSpace(Request.QueryString["page"]))
            {
                int.TryParse(Request.QueryString["page"], out page);
            }

            // 検索文字列
            var words = Request.QueryString["S_WORDS"] ?? "";

            // 表示ヘッダー
            var header = new[] {
                new { name = "Order", title = "表示順" },
                new { name = "AccracyName", title = "ステータス" },
                new { name = "Del_", title = "" }
                };

            //var list = Enumerable.Range(0, 0).ToList();

            var db = new HMPZDDB();

            var list = db.Accuracy.Where(d => d.DeleteFlg != 1).ToList();

            // 検索不要
            //if (!string.IsNullOrWhiteSpace(Request.QueryString["S_WORDS"]))
            //{
            //    var wordList = C.toSearch( Request.QueryString["S_WORDS"] ?? "").Replace("　", " ").Split(' ');

            //    list = list.Where(d => {
            //        foreach (var word in wordList)
            //        {
            //            var name = C.toSearch(d.DepartmentName + " " + d.TeamName + " " + d.FirstName + " " + d.LastName);
            //            if (!(name.IndexOf(word) >= 0))
            //            {
            //                return false;
            //            }
            //        }
            //        return true;
            //    }).ToList();
            //}

            // 総件数
            total = list.Count;

            // ページング+データ更新
            // slicgrid は null を戻してはいけない
            var rows = list.Skip((page - 1) * limit).Take(limit)
                .Select(d => new
                {
                    ID = d.ID ,
                    Order = d.Order ?? 0,
//  2017/10/25 - Y.Yamamoto modified.
//  for HMP_ZD-153
//                    AccracyName = d.Status ?? 0
                    AccuracyName = d.AccuracyName,
                })
                .ToList();

            // 総ページ数
            var totalPage = 0;
            if (total > 0)
            {
                var n = total / limit;
                totalPage = (int)Math.Ceiling((double)total / limit);
            }


            return Json(new { header = header, rows = rows, total = total, page = page, totalPage = totalPage }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region"削除"
        /// <summary>
        /// 削除
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Del(int ID = 0)
        {
            if (Request.IsAjaxRequest())
            {
                if (ID > 0)
                {
                    using (HMPZDDB db = new HMPZDDB())
                    {

                        var res = db.Accuracy.Where(d => d.DeleteFlg != 1 && d.ID == ID).FirstOrDefault();
                        if (res != null)
                        {
                            db.Accuracy.Remove(res);
                        }

                        db.SaveChanges();
                    }

                }

                return Json(new { res = "ok" });

            }

            return new EmptyResult();
        }
        #endregion

        #region"個票"

        /// <summary>
        /// 個票画面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(int ID=0)
        {

            Accuracy res = null;

            if (ID>0)
            {
                using (HMPZDDB db = new HMPZDDB())
                {
                     res = db.Accuracy.Where(d => d.DeleteFlg != 1 && d.ID == ID).FirstOrDefault();
                }

            }


            if (res!=null)
            {
                ViewBag.ID = res.ID;
                ViewBag.ORDER = res.Order;
                ViewBag.AccuracyName = "";
            }
            
            return View("~/Views/Admin/Accuracy/Edit.cshtml");
        }
        
        
        /// <summary>
        /// 個票の編集
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Update()
        {
            if (!Request.IsAjaxRequest())
            {
                return new EmptyResult();
            }

            // 新規の場合、0
            var id = V.ParseInt(Request.Form["ID"]) ?? 0;


            var err = Validate();
            if (err.Any())
            {
                return Json(new { err = err });
            }
            

            using (HMPZDDB db = new HMPZDDB())
            {

                Accuracy data = null;
                if (id == 0)
                {
                    data = new Accuracy();
                    // TODO これはバグってるな。。。
                    data.Created = data.Updated = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    // TODO ログインユーザ
                    data.CreatedBy = data.UpdatedBy = DateTime.Now;

                }
                else
                {
                    data = db.Accuracy.Where(d => d.DeleteFlg != 1 && d.ID == id).FirstOrDefault();

                    // TODO これはバグってるな。。。
                    data.Updated = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    // TODO ログインユーザ
                    data.UpdatedBy = DateTime.Now;
                }


                data.Order = V.ParseInt(Request.Form["Order"]) ?? 0;
                // data.Status

                if (id==0)
                {
                    db.Accuracy.Add(data);
                    id = data.ID;
                }

                db.SaveChanges();

            }

            return Json(new { res = "ok",id=id });
        }
        
        /// <summary>
        /// 値のチェック
        /// </summary>
        /// <returns></returns>
        private String[] Validate()
        {
            var err=new List<String>();

            var id = V.ParseInt(Request.Form["ID"]) ?? 0;


            if ((V.ParseInt(Request.Form["Order"]) ?? 0)==0)
            {
                err.Add("表示順を入力してください");
            }

            if (string.IsNullOrWhiteSpace(Request.Form["AccuracyName"]))
            {
                err.Add("ステータスを入力してください");
            }

            return err.ToArray();
        }

        #endregion
    }
}
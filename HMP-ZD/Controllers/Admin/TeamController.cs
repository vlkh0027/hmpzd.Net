using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HMP_ZD.Models;
using HMP_ZD.Utils;
using System.Dynamic;
using V = HMP_ZD.Utils.ValueUtil;
using C = HMP_ZD.Utils.CommonUtil;

namespace HMP_ZD.Controllers.Admin
{
    public class TeamController : Controller
    {
        #region "一覧"
        /// <summary>
        /// 一覧画面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View("~/Views/Admin/Team/Index.cshtml");
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
                new { name = "DepartmentName", title = "部署" },
                new { name = "TeamName", title = "チーム" },
                new { name = "UserName", title = "担当" },
                new { name = "Del_", title = "" }
                };

            //var list = Enumerable.Range(0, 0).ToList();

            var db = new HMPZDDB();

            // データ一式。JOINさせる
            var list = db.TeamRepresentatives.Where(d => d.DeleteFlg != 1)
                .Join(db.Teams.Where(d => d.DeleteFlg != 1), d1 => d1.TeamID, d2 => d2.ID, (d1, d2) =>
                new {
                    ID = d1.ID,
                    DepartmentName = d1.DepartmentName,
                    FirstName = d1.FirstName,
                    LastName = d1.LastName,
                    TeamName = d2.TeamName
                })
                .ToList();


            if (!string.IsNullOrWhiteSpace(Request.QueryString["S_WORDS"]))
            {
                var wordList = V.ToSearch( Request.QueryString["S_WORDS"] ?? "").Replace("　", " ").Split(' ')  ;

                list = list.Where(d => {
                    foreach (var word in wordList)
                    {
                        var name = V.ToSearch(d.DepartmentName + " " + d.TeamName + " " + d.FirstName + " " + d.LastName);
                        if (!(name.IndexOf(word) >= 0))
                        {
                            return false;
                        }
                    }
                    return true;
                }).ToList();
            }

            // 総件数
            total = list.Count;

            // ページング+データ更新
            // slicgrid は null を戻してはいけない
            var rows = list.Skip((page - 1) * limit).Take(limit)
                .Select(d => new
                {
                    ID = d.ID ,
                    DepartmentName = d.DepartmentName ?? "",
                    TeamName = d.TeamName ?? "",
                    UserName = d.LastName + " " + d.FirstName
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

                        var res = db.TeamRepresentatives.Where(d => d.DeleteFlg != 1 && d.ID == ID).FirstOrDefault();
                        if (res != null)
                        {
                            db.TeamRepresentatives.Remove(res);
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

            TeamRepresentatives res = null;

            if (ID>0)
            {
                using (HMPZDDB db = new HMPZDDB())
                {
                     res = db.TeamRepresentatives.Where(d => d.DeleteFlg != 1 && d.ID == ID).FirstOrDefault();
                }

            }

            var organizationId = "";
            var teamId = 0;

            if (res!=null)
            {
                using (HMPZDDB db = new HMPZDDB())
                {
                    var team = db.Teams.Where(d => d.DeleteFlg !=1 && d.ID == res.TeamID).FirstOrDefault();
                    if (team != null)
                    {
                        organizationId = team.OrganizationID;
                        teamId = team.ID;

                    }
                }

                ViewBag.ID = res.ID;
                //ViewBag.DepartmentName = res.DepartmentName;
                ViewBag.UserID = res.UserID ?? "";
                ViewBag.FirstName = res.FirstName ?? "";
                ViewBag.LastName = res.LastName ?? "";
            }

            // 部署リスト
            using (HMPZDDB db = new HMPZDDB())
            {
                var departmentList = GetMagazineDeparmentList();

                ViewBag.DepartmentList = departmentList
                .Select(
                    d => new SelectListItem
                    {
                        Value = d.ID.ToString(),
                        Text = d.DepartmentAbbrName,
                        Selected = (organizationId == d.ID.ToString())
                    }).ToList();

                // DEBUG
                //foreach (var d in departmentList)
                //{
                //    for (var i=1;i<=10;i++)
                //    {
                //        db.Teams.Add(
                //            new Teams()
                //            {
                //                OrganizationID = d.ID,
                //                TeamName = d.DepartmentCommonName +"チーム" + i,
                //                TeamCode = d.ID+i.ToString("000000")
                //            });
                //    }
                //    db.SaveChanges();
                //}
            }

            // チームリスト
            // JSのほうでやってもよい気がする
            using (HMPZDDB db = new HMPZDDB())
            {
                ViewBag.TeamList = 
                    db.Teams.Where(d => d.DeleteFlg != 1)
                    .Select(
                        d =>  new SelectListItem {
                            
                            Value =d.ID.ToString(),
                            Text =d.TeamName,
                            Selected =(d.ID==teamId)
                        }).ToList();

            }

            return View("~/Views/Admin/Team/Edit.cshtml");
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

                // チーム情報
                var teamId =V.ParseInt(Request.Form["TeamID"]) ?? 0;
                
                // 組織情報
                Teams team = null;
                Organizations organization = null;
                if (teamId > 0)
                {
                    team = db.Teams.Where(d => d.DeleteFlg != 1 && d.ID == teamId).FirstOrDefault();
                }
                if (team != null && !string.IsNullOrWhiteSpace(team.OrganizationID))
                {
                    organization = db.Organizations.Where(d => d.DeleteFlg != 1 && d.ID == team.OrganizationID).FirstOrDefault();
                }

                // ユーザ情報
                var userIdList = (Request.Form["UserIDList"] ?? "").Split(',');
                foreach (var row in userIdList)
                {
                    // 既存のユーザID x TeamID 情報は一度消す
                    var teamRepresentative = db.TeamRepresentatives.Where(d => d.DeleteFlg != 1 && d.TeamID == teamId && d.UserID == row).FirstOrDefault();
                    if (teamRepresentative != null)
                    {
                        db.TeamRepresentatives.Remove(teamRepresentative);
                    }

                    // 新規として追加
                    TeamRepresentatives data = new TeamRepresentatives();

                    data.Created = data.Updated = DateTime.Now;
                    
                    // TODO ログインユーザ
                    data.CreatedBy = data.UpdatedBy = "";

                    // チーム情報
                    data.TeamID = teamId;

                    // 組織情報
                    if (organization != null)
                    {
                        data.CompanyName = organization.CompanyName ?? "";
                        data.DivisionGroupName = organization.DivisionGroupCommonName;
                        data.DivisionName = organization.DivisionAbbrName;
                        data.DepartmentName = organization.DepartmentAbbrName;
                    }

                    Users user = db.Users.Where(d => d.DeleteFlg != 1 && d.ID == row).FirstOrDefault(); 
                    if (user == null)
                    {
                        continue;
                    }

                    data.UserID = user.ID;
                    data.FirstName = user.FirstName;
                    data.LastName = user.LastName;

                    db.TeamRepresentatives.Add(data);



                }
                

                db.SaveChanges();
               

            }

            return Json(new { res = "ok" });
        }
        
        /// <summary>
        /// 値のチェック
        /// </summary>
        /// <returns></returns>
        private String[] Validate()
        {
            var err=new List<String>();

            var id = V.ParseInt(Request.Form["ID"]) ?? 0;

            if ((V.ParseInt(Request.Form["TeamID"]) ?? 0)==0)
            {
                err.Add("チームを選択してください");
            }

            // 空
            if (!(Request.Form["UserID"] ?? "").Split(',').Any())
            {
                err.Add("ユーザを選択してください");
            }

            return err.ToArray();
        }
        
        /// <summary>
        /// 雑誌局リスト
        /// </summary>
        /// <returns></returns>
        private List<Organizations> GetMagazineDeparmentList()
        {
            var list = new List<Organizations>();

            // 部署リスト
            using (HMPZDDB db = new HMPZDDB())
            {
                list = db.Organizations
                    .Where(d => d.DeleteFlg != 1 &&
                    (d.DepartmentAbbrName.Contains("雑誌一部")
                || d.DepartmentAbbrName.Contains("雑誌二部")
                || d.DepartmentAbbrName.Contains("雑誌三部")
                || d.DepartmentAbbrName.Contains("業推一部")
                || d.DepartmentAbbrName.Contains("業推二部"))).ToList();
            }

            return list;
        }

        #endregion
    }
}
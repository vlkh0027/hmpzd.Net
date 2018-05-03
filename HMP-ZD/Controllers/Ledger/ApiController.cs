using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HMP_ZD.Models;
using HMP_ZD.Utils;
using System.Globalization;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using Codeplex.Data;
using System.Reflection;

namespace HMP_ZD.Controllers.Ledger
{
    public class ApiController : Controller
    {
        /// <summary>
        /// 部署から担当チームを取得
        /// </summary>
        /// <param name="id">部署コード</param>
        /// <returns></returns>
        public ActionResult GetTeams(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            using(HMPZDDB db = new HMPZDDB())
            {
                var ret = db.Teams.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                              d.OrganizationID == id)
                                  .Select(d => new SelectListItem()
                                  {
                                      Value = d.ID.ToString(),
                                      Text = d.TeamName,
                                  })
                                  .ToList();

                return Json(ret, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 雑誌局チームに紐付く媒体社リストの取得
        /// </summary>
        /// <param name="id">チームID</param>
        /// <returns></returns>
        public ActionResult GetMagazineCompanies(int? id)
        {
            if(!id.HasValue)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            using(HMPZDDB db = new HMPZDDB())
            {
                List<SelectListItem> ret = new List<SelectListItem>();

                //  担当者グループリストを取得
                var pubResGrp = db.PublisherResponsibleGroups.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                         d.MagazineGroupID == id.Value);

                //  媒体社リストを取得
                foreach(var item in pubResGrp)
                {
                    var cust = db.Customers.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                d.ID == item.CustomerID);

                    if(cust == null)
                    {
                        continue;
                    }


                    ret.Add(new SelectListItem()
                    {
                        Value = cust.ID,
                        Text = cust.HENCustomerAbbrName,
                    });
                }

                return Json(ret, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 媒体社に紐付く媒体誌リストの取得
        /// </summary>
        /// <param name="id">媒体社ID</param>
        /// <returns></returns>
        public ActionResult GetVehiclesByID(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            using(HMPZDDB db = new HMPZDDB())
            {
                var ret = db.Vehicles.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                 d.CustomerID == id)
                                     .OrderBy(d => d.HENVehicleName)
                                     .Select(d => new SelectListItem()
                                     {
                                         Value = d.ID.ToString(),
                                         Text = d.HENVehicleName,
                                     })
                                     .ToList();

                return Json(ret, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 媒体社名から紐付く媒体誌リストを取得
        /// </summary>
        /// <param name="name">媒体社名</param>
        /// <returns></returns>
        public ActionResult GetVehicelesByName(string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            using(HMPZDDB db = new HMPZDDB())
            {
                var custID = db.Customers.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                              (d.HENNormalizationKana == name ||
                                                              d.HENNormalizationEN == name ||
                                                              d.HENCustomerName == name ||
                                                              d.HENCustomerAbbrName == name ||
                                                              d.HENCustomerNameKana == name ||
                                                              d.HENCustomerAbbrNameKana == name ||
                                                              d.HENCustomerNameEN == name ||
                                                              d.HENCustomerAbbrNameEN == name ||
                                                              d.HENCustomerNameCommon == name));

                if(custID == null)
                {
                    //  得意先検索しても出てこなかった
                    //  - nullを返す
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                //  IDを使用した媒体誌検索へ
                return GetVehiclesByID(custID.ID);
            }
        }

        public ActionResult GetMyDepartmentTeamInfo(string id)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            using(HMPZDDB db = new HMPZDDB())
            {
                //  ユーザ情報
                var user = db.Users.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                        d.ID == id);
                //  所属チーム
                var team = db.TeamRepresentatives.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                      d.UserID == id);

                if(user == null || team == null)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                var userInfo = new
                {
                    userID= id,
                    companyCode = user.CompanyCode,
                    companyName = team.CompanyName,
                    groupCode = user.DivisionGroupCode,
                    groupName = team.DivisionGroupName,
                    divisionCode = user.DivisionCode,
                    divisionName = team.DivisionName,
                    departmentCode = user.DepartmentCode,
                    departmentName = team.DepartmentName,
                    name = team.LastName + " " + team.FirstName,
                    teamID = team.TeamID,
                };

                return Json(userInfo, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 広告社検索
        /// </summary>
        /// <param name="q">検索語（半角空白区切りで複数語句入力可）</param>
        /// <returns>ヒットした広告社をJSONのリスト形式で渡す
        /// ID: CustomerID
        /// Name: 略称
        /// </returns>
        public ActionResult SearchCompany(string q)
        {
            if(string.IsNullOrEmpty(q))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            string[] queries = q.Split(' ');

            using(HMPZDDB db = new HMPZDDB())
            {
                var ret = db.Customers.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                  d.HENDeleteFlg != DBUtil.FLAG_YES &&
                                                  queries.Any(e => d.HENCustomerAbbrName.Contains(e)))
                                      .OrderBy(d => d.HENCustomerAbbrName)
                                      .Select(d => new
                                      {
                                          ID = d.ID,
                                          Name = d.HENCustomerAbbrName,
                                      })
                                      .ToList();

                return Json(ret, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 会社情報取得
        /// </summary>
        /// <param name="id">customer ID</param>
        /// <returns></returns>
        public ActionResult GetCompany(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            using(HMPZDDB db = new HMPZDDB())
            {
                var cust = db.Customers.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                            d.HENDeleteFlg != DBUtil.FLAG_YES &&
                                                            d.ID == id);

                if(cust == null)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                //  業種分類（大）データを取得
                var industryL = db.IndustryLargeClassifications.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                                    d.HENLargeClassIndustryTypeCode == cust.HENCustomerIndustryCode);

                var ret = new
                {
                    ID = cust.ID,
                    Name = cust.HENCustomerAbbrName,
                    IndustryID = (industryL == null) ? 0 : industryL.ID,
                    IndustryName = (industryL == null) ? "(undef)": "(no name)",
                };

                return Json(ret, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetVehicleSchedule(int? id)
        {
            if(!id.HasValue)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            int ID = id.Value;

            using(HMPZDDB db = new HMPZDDB())
            {
                DateTime? ret = null;

                //  進行スケジュールから媒体誌の進行スケジュールデータを取得
                var sch = db.ProgressSchedules.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                   d.VehicleID == ID);

                if(sch != null)
                {
                    //  進行スケジュールIDからデータ取得
                    var timeTable = db.ProgressTimetable.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                    d.ProgressScheduleID == sch.ID)
                                                        .OrderByDescending(d => d.ReleaseDate)
                                                        .FirstOrDefault();

                    if(timeTable != null)
                    {
                        ret = timeTable.ReleaseDate;
                    }
                }

                //  進行スケジュールデータから取得できたか?
                if(!ret.HasValue)
                {
                    //  HEN媒体誌スケジュールからデータを取得
                    var hen = db.VehicleSchedules.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                      d.VehicleID == id);

                    if(hen != null)
                    {
                        ret = hen.HENReleaseDate;
                    }
/*                    else
                    {
                        return Json(null, JsonRequestBehavior.AllowGet);
                    }
*/                }

                /*
                                var jsRet = new
                                {
                                    ReleaseDate = ret.Value.ToString("yyyy/MM/dd"),
                                };
                */
                var jsRet = new
                {
                    ReleaseDate = DateTime.Now.ToString("yyyy/MM/dd"),
                };

                return Json(jsRet, JsonRequestBehavior.AllowGet);
            }
        }

        #region ユーザ検索POPUP
        /// <summary>
        /// 会社情報検索
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetOrganizations(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            using(HMPZDDB db = new HMPZDDB())
            {
                var items = db.Organizations.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                        d.SuperOrganizationCode == id)
                                            .ToList()
                                            .OrderBy(d => int.Parse(d.ID))
                                            .Select(d => new
                                            {
                                                id = d.ID,
                                                companyName = d.CompanyName,
                                                divisionGroupName = d.DivisionGroupCommonName,
                                                divisionName = d.DivisionAbbrName,
                                                departmentName = d.DepartmentAbbrName,
                                            })
                                            .ToList();

                return Json(items, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// ユーザ検索
        /// </summary>
        /// <param name="q">検索語句（フリーワード）</param>
        /// <param name="company">会社ID</param>
        /// <param name="group">部門グループID</param>
        /// <param name="division">部門ID</param>
        /// <param name="department">部署ID</param>
        /// <returns>検索結果</returns>
        /// <remarks>各IDはOrganizationsテーブルのIDフィールドの値</remarks>
        public ActionResult SearchUser(string q, string company, string group, string division, string department)
        {
            string[] queries = q.Split(' ');

            //=====================================================
            //  LINQ to Entityの性質上、非常に長い条件文になっているので
            //  要注意
            //=====================================================
            using(HMPZDDB db = new HMPZDDB())
            {
                Users[] usrs = null;

                if(!string.IsNullOrEmpty(department))
                {
                    //  部署コード検索
                    usrs = db.Users.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                            (d.DepartmentCode == department ||
                                            d.Add1DepartmentCode == department ||
                                            d.Add2DepartmentCode == department ||
                                            d.Add3DepartmentCode == department ||
                                            d.Add4DepartmentCode == department ||
                                            d.Add5DepartmentCode == department ||
                                            d.Add6DepartmentCode == department ||
                                            d.Add7DepartmentCode == department ||
                                            d.Add8DepartmentCode == department ||
                                            d.Add9DepartmentCode == department ||
                                            d.Add10DepartmentCode == department ||
                                            d.Add11DepartmentCode == department ||
                                            d.Add12DepartmentCode == department ||
                                            d.Add13DepartmentCode == department ||
                                            d.Add14DepartmentCode == department ||
                                            d.Add15DepartmentCode == department))
                                   .ToArray();
                }
                else
                {
                    if(!string.IsNullOrEmpty(division))
                    {
                        //  部門コード検索
                        usrs = db.Users.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                (d.DivisionCode == division ||
                                                d.Add1DivisionCode == division ||
                                                d.Add2DivisionCode == division ||
                                                d.Add3DivisionCode == division ||
                                                d.Add4DivisionCode == division ||
                                                d.Add5DivisionCode == division ||
                                                d.Add6DivisionCode == division ||
                                                d.Add7DivisionCode == division ||
                                                d.Add8DivisionCode == division ||
                                                d.Add9DivisionCode == division ||
                                                d.Add10DivisionCode == division ||
                                                d.Add11DivisionCode == division ||
                                                d.Add12DivisionCode == division ||
                                                d.Add13DivisionCode == division ||
                                                d.Add14DivisionCode == division ||
                                                d.Add15DivisionCode == division))
                                        .ToArray();
                    }
                    else
                    {
                        if(!string.IsNullOrEmpty(group))
                        {
                            //  部門グループコード検索
                            usrs = db.Users.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                    (d.DivisionGroupCode == group ||
                                                    d.Add1DivisionGroupCode == group ||
                                                    d.Add2DivisionGroupCode == group ||
                                                    d.Add3DivisionGroupCode == group ||
                                                    d.Add4DivisionGroupCode == group ||
                                                    d.Add5DivisionGroupCode == group ||
                                                    d.Add6DivisionGroupCode == group ||
                                                    d.Add7DivisionGroupCode == group ||
                                                    d.Add8DivisionGroupCode == group ||
                                                    d.Add9DivisionGroupCode == group ||
                                                    d.Add10DivisionGroupCode == group ||
                                                    d.Add11DivisionGroupCode == group ||
                                                    d.Add12DivisionGroupCode == group ||
                                                    d.Add13DivisionGroupCode == group ||
                                                    d.Add14DivisionGroupCode == group ||
                                                    d.Add15DivisionGroupCode == group))
                                           .ToArray();
                        }
                        else
                        {
                            if(!string.IsNullOrEmpty(company))
                            {
                                //  IDから会社コードを取得
                                var comID = db.Organizations.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                                 d.ID == company)
                                                            .ComapnyCode;

                                //  会社コード検索
                                usrs = db.Users.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                           d.CompanyCode == comID)
                                               .ToArray();
                            }
                        }
                    }
                }

                if(usrs == null)
                {
                    if(!string.IsNullOrEmpty(q))
                    {
                        usrs = db.Users.Where(d => queries.Any(e => d.LastName.Contains(e) ||
                                                                    d.FirstName.Contains(e) ||
                                                                    d.LastNameKANA.Contains(e) ||
                                                                    d.FirstNameKANA.Contains(e) ||
                                                                    d.LastNameEN.Contains(e) ||
                                                                    d.FirstNameEN.Contains(e) ||
                                                                    d.CompanyName.Contains(e) ||
                                                                    d.DivisionGroupName.Contains(e) ||
                                                                    d.DivisionName.Contains(e) ||
                                                                    d.DepartmentName.Contains(e) ||
                                                                    d.JobTitle.Contains(e) ||
                                                                    d.Add1CompanyName.Contains(e) ||
                                                                    d.Add1DivisionGroupName.Contains(e) ||
                                                                    d.Add1DivisionName.Contains(e) ||
                                                                    d.Add1DepartmentName.Contains(e) ||
                                                                    d.Add2CompanyName.Contains(e) ||
                                                                    d.Add2DivisionName.Contains(e) ||
                                                                    d.Add2DepartmentName.Contains(e) ||
                                                                    d.Add3CompanyName.Contains(e) ||
                                                                    d.Add3DivisionGroupName.Contains(e) ||
                                                                    d.Add3DivisionName.Contains(e) ||
                                                                    d.Add3DepartmentName.Contains(e) ||
                                                                    d.Add4CompanyName.Contains(e) ||
                                                                    d.Add4DivisionGroupName.Contains(e) ||
                                                                    d.Add4DivisionName.Contains(e) ||
                                                                    d.Add4DepartmentName.Contains(e) ||
                                                                    d.Add5CompanyName.Contains(e) ||
                                                                    d.Add5DivisionGroupName.Contains(e) ||
                                                                    d.Add5DivisionName.Contains(e) ||
                                                                    d.Add5DepartmentName.Contains(e) ||
                                                                    d.Add6CompanyName.Contains(e) ||
                                                                    d.Add6DivisionGroupName.Contains(e) ||
                                                                    d.Add6DivisionName.Contains(e) ||
                                                                    d.Add6DepartmentName.Contains(e) ||
                                                                    d.Add7CompanyName.Contains(e) ||
                                                                    d.Add7DivisionGroupName.Contains(e) ||
                                                                    d.Add7DivisionName.Contains(e) ||
                                                                    d.Add7DepartmentName.Contains(e) ||
                                                                    d.Add8CompanyName.Contains(e) ||
                                                                    d.Add8DivisionGroupName.Contains(e) ||
                                                                    d.Add8DivisionName.Contains(e) ||
                                                                    d.Add8DepartmentName.Contains(e) ||
                                                                    d.Add9CompanyName.Contains(e) ||
                                                                    d.Add9DivisionGroupName.Contains(e) ||
                                                                    d.Add9DivisionName.Contains(e) ||
                                                                    d.Add9DepartmentName.Contains(e) ||
                                                                    d.Add10CompanyName.Contains(e) ||
                                                                    d.Add10DivisionGroupName.Contains(e) ||
                                                                    d.Add10DivisionName.Contains(e) ||
                                                                    d.Add10DepartmentName.Contains(e) ||
                                                                    d.Add11CompanyName.Contains(e) ||
                                                                    d.Add11DivisionGroupName.Contains(e) ||
                                                                    d.Add11DivisionName.Contains(e) ||
                                                                    d.Add11DepartmentName.Contains(e) ||
                                                                    d.Add12CompanyName.Contains(e) ||
                                                                    d.Add12DivisionGroupName.Contains(e) ||
                                                                    d.Add12DivisionName.Contains(e) ||
                                                                    d.Add12DepartmentName.Contains(e) ||
                                                                    d.Add13CompanyName.Contains(e) ||
                                                                    d.Add13DivisionGroupName.Contains(e) ||
                                                                    d.Add13DivisionName.Contains(e) ||
                                                                    d.Add13DepartmentName.Contains(e) ||
                                                                    d.Add14CompanyName.Contains(e) ||
                                                                    d.Add14DivisionGroupName.Contains(e) ||
                                                                    d.Add14DivisionName.Contains(e) ||
                                                                    d.Add14DepartmentName.Contains(e) ||
                                                                    d.Add15CompanyName.Contains(e) ||
                                                                    d.Add15DivisionGroupName.Contains(e) ||
                                                                    d.Add15DivisionName.Contains(e) ||
                                                                    d.Add15DepartmentName.Contains(e)))
                                       .ToArray();
                    }
                }
                else
                {
                    if(!string.IsNullOrEmpty(q))
                    {
                        Func<string, bool> IsContains = (s) =>
                        {
                            if(string.IsNullOrEmpty(s))
                            {
                                return false;
                            }

                            return queries.Any(e => s.Contains(e));
                        };

                        usrs = usrs.Where(d => IsContains(d.LastName) ||
                                               IsContains(d.FirstName) ||
                                               IsContains(d.LastNameKANA) ||
                                               IsContains(d.FirstNameKANA) ||
                                               IsContains(d.LastNameEN) ||
                                               IsContains(d.FirstNameEN) ||
                                               IsContains(d.CompanyName) ||
                                               IsContains(d.DivisionGroupName) ||
                                               IsContains(d.DivisionName) ||
                                               IsContains(d.DepartmentName) ||
                                               IsContains(d.JobTitle) ||
                                               IsContains(d.Add1CompanyName) ||
                                               IsContains(d.Add1DivisionGroupName) ||
                                               IsContains(d.Add1DivisionName) ||
                                               IsContains(d.Add1DepartmentName) ||
                                               IsContains(d.Add2CompanyName) ||
                                               IsContains(d.Add2DivisionName) ||
                                               IsContains(d.Add2DepartmentName) ||
                                               IsContains(d.Add3CompanyName) ||
                                               IsContains(d.Add3DivisionGroupName) ||
                                               IsContains(d.Add3DivisionName) ||
                                               IsContains(d.Add3DepartmentName) ||
                                               IsContains(d.Add4CompanyName) ||
                                               IsContains(d.Add4DivisionGroupName) ||
                                               IsContains(d.Add4DivisionName) ||
                                               IsContains(d.Add4DepartmentName) ||
                                               IsContains(d.Add5CompanyName) ||
                                               IsContains(d.Add5DivisionGroupName) ||
                                               IsContains(d.Add5DivisionName) ||
                                               IsContains(d.Add5DepartmentName) ||
                                               IsContains(d.Add6CompanyName) ||
                                               IsContains(d.Add6DivisionGroupName) ||
                                               IsContains(d.Add6DivisionName) ||
                                               IsContains(d.Add6DepartmentName) ||
                                               IsContains(d.Add7CompanyName) ||
                                               IsContains(d.Add7DivisionGroupName) ||
                                               IsContains(d.Add7DivisionName) ||
                                               IsContains(d.Add7DepartmentName) ||
                                               IsContains(d.Add8CompanyName) ||
                                               IsContains(d.Add8DivisionGroupName) ||
                                               IsContains(d.Add8DivisionName) ||
                                               IsContains(d.Add8DepartmentName) ||
                                               IsContains(d.Add9CompanyName) ||
                                               IsContains(d.Add9DivisionGroupName) ||
                                               IsContains(d.Add9DivisionName) ||
                                               IsContains(d.Add9DepartmentName) ||
                                               IsContains(d.Add10CompanyName) ||
                                               IsContains(d.Add10DivisionGroupName) ||
                                               IsContains(d.Add10DivisionName) ||
                                               IsContains(d.Add10DepartmentName) ||
                                               IsContains(d.Add11CompanyName) ||
                                               IsContains(d.Add11DivisionGroupName) ||
                                               IsContains(d.Add11DivisionName) ||
                                               IsContains(d.Add11DepartmentName) ||
                                               IsContains(d.Add12CompanyName) ||
                                               IsContains(d.Add12DivisionGroupName) ||
                                               IsContains(d.Add12DivisionName) ||
                                               IsContains(d.Add12DepartmentName) ||
                                               IsContains(d.Add13CompanyName) ||
                                               IsContains(d.Add13DivisionGroupName) ||
                                               IsContains(d.Add13DivisionName) ||
                                               IsContains(d.Add13DepartmentName) ||
                                               IsContains(d.Add14CompanyName) ||
                                               IsContains(d.Add14DivisionGroupName) ||
                                               IsContains(d.Add14DivisionName) ||
                                               IsContains(d.Add14DepartmentName) ||
                                               IsContains(d.Add15CompanyName) ||
                                               IsContains(d.Add15DivisionGroupName) ||
                                               IsContains(d.Add15DivisionName) ||
                                               IsContains(d.Add15DepartmentName))
                                   .ToArray();

                                                                

                    }
                }

                //  検索結果を整形
                var items = usrs.OrderBy(d => d.LastName)
                               .ThenBy(d => d.FirstName)
                               .Select(d => new
                {
                    id = d.ID,
                    company = d.CompanyName,
                    divisiongroup = d.DivisionGroupName,
                    division = d.DivisionName,
                    department = d.DepartmentName,
                    name = d.LastName + " " + d.FirstName,
                    mail = d.Email,
                })
                .ToList();

                return Json(items, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}

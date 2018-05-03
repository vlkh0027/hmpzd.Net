using HMP_ZD.Models;
using HMP_ZD.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HMP_ZD.Controllers.Ledger
{
    public class WorkController : Controller
    {
        // GET: Work
        public ActionResult New()
        {
            SetBaseData();
            return View("~/Views/Ledger/Work/New.cshtml");
        }


        /// <summary>
        /// 基本データの設定
        /// </summary>
        private void SetBaseData()
        {
            using (HMPZDDB db = new HMPZDDB())
            {
                #region DropDownList items

                ViewBag.TeamItems = new List<SelectListItem>();
                ViewBag.CustomerIDItems = new List<SelectListItem>();
                ViewBag.VehicleIDItems = new List<SelectListItem>();

                if (ViewBag.Department != null)
                {
                    //  担当チーム
                    string departmenet = (string)ViewBag.Department;

                    ViewBag.TeamItems = db.Teams.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                            d.OrganizationID == departmenet)
                                                .OrderBy(d => d.ID)
                                                .Select(d => new SelectListItem()
                                                {
                                                    Value = d.ID.ToString(),
                                                    Text = d.TeamName,
                                                })
                                                .ToList();
                }
                else
                {
                    ViewBag.TeamItems = new List<SelectListItem>();
                }

                if (ViewBag.TeamID != null)
                {
                    //  媒体社リスト
                    var team = (int)ViewBag.TeamID;

                    //  担当者グループリストを取得
                    ViewBag.CustomerIDItems = (from d1 in db.PublisherResponsibleGroups
                                               join d2 in db.Customers on d1.CustomerID equals d2.ID
                                               where d1.MagazineGroupID == team
                                               orderby d2.HENCustomerAbbrName
                                               select new SelectListItem()
                                               {
                                                   Value = d2.ID,
                                                   Text = d2.HENCustomerAbbrName,
                                               })
                                              .ToList();
                }
                else
                {
                    ViewBag.CustomerIDItems = new List<SelectListItem>();
                }

                if (ViewBag.CustomerID != null)
                {
                    //  媒体誌リスト
                    var custID = (string)ViewBag.CustomerID;

                    ViewBag.VehicleIDItems = db.Vehicles.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                    d.CustomerID == custID)
                                                        .OrderBy(d => d.HENVehicleName)
                                                        .Select(d => new SelectListItem()
                                                        {
                                                            Value = d.ID.ToString(),
                                                            Text = d.HENVehicleName,
                                                        })
                                                        .ToList();
                }
                else
                {
                    ViewBag.VehicleIDItems = new List<SelectListItem>();
                }

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

                //  部署
                //  ログインユーザによって候補が増減する
                var tmp = new List<SelectListItem>();

                string userID = (string)ViewBag.UserID;
                var perm = db.UserPermissions.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                  d.UserID == userID);

                if (perm == null)
                {
                    perm = new UserPermissions();
                    perm.OperableDepartment = "1,2,3";
                }

                //  権限を取得
                var permissions = perm.OperableDepartment.Split(',')
                                                         .Select(d => int.Parse(d.Trim()));

                if (permissions.Any(d => d == (int)DepartmentType.Magazine1 || d > (int)DepartmentType.Magazine3))
                {
                    //  雑誌1部
                    tmp.Add(GetDepartmentData("雑誌一部"));
                }

                if (permissions.Any(d => d == (int)DepartmentType.Magazine2 || d > (int)DepartmentType.Magazine3))
                {
                    //  雑誌2部
                    tmp.Add(GetDepartmentData("雑誌二部"));
                }

                if (permissions.Any(d => d == (int)DepartmentType.Magazine3 || d > (int)DepartmentType.Magazine3))
                {
                    //  雑誌3部
                    tmp.Add(GetDepartmentData("雑誌三部"));
                }
                /*
                                //  業務推進部 / 雑誌進行部 / 局長席 / 営業局は全候補を表示
                                if(permissions.Any(d => d > (int)DepartmentType.Magazine3))
                                {
                                    tmp.Add(GetDepartmentData("雑業推一"));
                                    tmp.Add(GetDepartmentData("雑業推二"));
                                    tmp.Add(GetDepartmentData("新雑進行"));


                                    tmp.Add(new SelectListItem() { Value = ((int)DepartmentType.BizPromo).ToString(), Text = "業務推進部" });
                                    tmp.Add(new SelectListItem() { Value = ((int)DepartmentType.MagProgress).ToString(), Text = "雑誌進行部" });
                                    tmp.Add(new SelectListItem() { Value = ((int)DepartmentType.Director).ToString(), Text = "局長席" });
                                    tmp.Add(new SelectListItem() { Value = ((int)DepartmentType.Operating).ToString(), Text = "営業局" });
                                }
                */
                ViewBag.DepartmentItems = tmp;

                #endregion

                #region Popup data
                //  ユーザ選択の会社DropDown
                ViewBag.CompanyItems = db.Organizations.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                   d.Hierarchy == 1)
                                                       .ToList()
                                                       .OrderBy(d => int.Parse(d.ID))
                                                       .Select(d => new SelectListItem()
                                                       {
                                                           Value = d.ID,
                                                           Text = d.CompanyName,
                                                       })
                                                       .ToList();
                #endregion
            }
        }

        /// <summary>
        /// 部署データを取得
        /// </summary>
        /// <param name="departmentName"></param>
        /// <returns></returns>
        private SelectListItem GetDepartmentData(string departmentName)
        {
            using (HMPZDDB db = new HMPZDDB())
            {
                var org = db.Organizations.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                               d.OrganizationCommonName == departmentName);

                if (org == null)
                {
                    return null;
                }

                return new SelectListItem()
                {
                    Value = org.ID,
                    Text = org.OrganizationAbbrName,
                };
            }
        }
    }
}
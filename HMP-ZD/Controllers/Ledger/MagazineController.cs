using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HMP_ZD.Models;
using HMP_ZD.Utils;
using Codeplex.Data;
using System.Text;
using System.IO;

namespace HMP_ZD.Controllers.Ledger
{
    public class MagazineController : Controller
    {
        // GET: Magazine
        /// <summary>
        /// 新規作成
        /// </summary>
        /// <returns></returns>
        public ActionResult New()
        {
            //  ユーザ情報取得
            UserInfo user = new UserInfo();

            ViewBag.UserName = user.UserName;
            ViewBag.UserID = user.UserID;
            ViewBag.ActionType = ActionType.New;

            SetBaseData();

            return View("~/Views/Ledger/Magazine/New.cshtml");
        }

        /// <summary>
        /// 編集画面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if(!id.HasValue)
            {
                //  EditはIDが必須
                return View();
            }

            ViewBag.ActionType = ActionType.Edit;

            SetData(id.Value);
            SetBaseData();

            return View("/Views/Ledger/Magazine/Edit.cshtml");
        }

        /// <summary>
        /// 詳細画面
        /// </summary>
        /// <param name="id">台帳ID</param>
        /// <returns></returns>
        public ActionResult Detail(int? id)
        {
            if(!id.HasValue)
            {
                //  DetailはIDが必須
                return View();
            }

            ViewBag.ActionType = ActionType.Detail;

            SetData(id.Value);

            return View("/Views/Ledger/Magazine/Detail.cshtml");
        }

        /// <summary>
        /// 台帳削除
        /// </summary>
        /// <param name="id">台帳ID</param>
        /// <returns></returns>
        #region Delete
        public ActionResult DeleteLedger(int? id)
        {
            int result;

            if(!id.HasValue)
            {
                var ret = new
                {
                    Date = DateTime.Now.ToString("s"),
                    Result = false,
                    Reason = "電子台帳IDが指定されていません",
                };

                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            using(HMPZDDB db = new HMPZDDB())
            {
                //  台帳データ取得
                var ledger = db.ElectronLedger.FirstOrDefault(d => d.ID == id.Value);

                if(ledger == null)
                {
                    var ret = new
                    {
                        Date = DateTime.Now.ToString("s"),
                        Result = false,
                        Reason = "指定されたIDは存在しませんでした",
                    };

                    return Json(ret, JsonRequestBehavior.AllowGet);
                }

                DBUtil.DeleteData(ref ledger);

                //  その他データ
                var sale = db.SaleRepresentatives.Where(d => d.ElectronLedgerID == id.Value).ToArray();

                for(int i = 0; i < sale.Count(); i++)
                {
                    DBUtil.DeleteData(ref sale[i]);
                }

                var mag = db.MagazineRepresentatives.Where(d => d.ElectronLedgerID == id.Value).ToArray();

                for(int i = 0; i < mag.Count(); i++)
                {
                    DBUtil.DeleteData(ref mag[i]);
                }

                var prod = db.ProductionRepresentatives.Where(d => d.ElectronLedgerID == id.Value).ToArray();

                for(int i = 0; i < prod.Count(); i++)
                {
                    DBUtil.DeleteData(ref prod[i]);
                }

                var prog = db.ProgressRepresentatives.Where(d => d.ElectronLedgerID == id.Value).ToArray();

                for(int i = 0; i < prod.Count(); i++)
                {
                    DBUtil.DeleteData(ref prog[i]);
                }

                var ba = db.BARepresentatives.Where(d => d.ElectronLedgerID == id.Value).ToArray();

                for(int i = 0; i < ba.Count(); i++)
                {
                    DBUtil.DeleteData(ref ba[i]);
                }

                var adv = db.AdvertisementValues.Where(d => d.ElectronLedgerID == id.Value).ToArray();

                for(int i = 0; i < adv.Count(); i++)
                {
                    DBUtil.DeleteData(ref adv[i]);
                }

                result = db.SaveChanges();
                if(result > 0)
                {
                    var ret = new
                    {
                        Date = DateTime.Now.ToString("s"),
                        Result = true,
                        Reason = "削除しました",
                    };

                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var ret = new
                    {
                        Date = DateTime.Now.ToString("s"),
                        Result = false,
                        Reason = "削除に失敗しました",
                    };

                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion

        /// <summary>
        /// 雑誌台帳保存
        /// </summary>
        /// <returns></returns>
        /// 

        #region Đăng Ký
        public ActionResult RegistMagazineLedger()
        {
            bool ret;
            List<string> reason;

            //  JSONデータ読み込み
            Request.InputStream.Position = 0;

            var data = DynamicJson.Parse(Request.InputStream, Encoding.UTF8);

            //  正当性チェック
            if(!CheckValid(data, out reason))
             {
                var failed = new
                {
                    date = DateTime.Now.ToLongTimeString(),
                    result = false,
                    reason = reason,
                };

                return Json(failed, JsonRequestBehavior.AllowGet);
            }

            using(HMPZDDB db = new HMPZDDB())
            {
                try
                {
                    #region 台帳データ保存 //Lưu
                    ElectronLedger ledger;// = new ElectronLedger();

                    //  新規登録か編集かの違いはIDの有無で判別する
                    if(data.ID != null)
                    {
                        //  編集
                        int id = (int)data.ID;
                        ledger = db.ElectronLedger.FirstOrDefault(d => d.ID == id);
                    }
                    else
                    {
                        //  新規
                        ledger = new ElectronLedger();
                    }

                    ledger.Status = ValueUtil.ParseInt(data.Status);
                    ledger.Type = ValueUtil.ParseInt(data.Type);
                    ledger.IssuanceStatus = ValueUtil.ParseInt(data.IssuanceStatus);
                    ledger.IssuanceDate = ValueUtil.ToDateTime(data.IssuanceDate);
                    ledger.IssuanceUserName = ValueUtil.ToString(data.IssuanceUserName);
                    ledger.ReceptStatus = ValueUtil.ParseInt(data.ReceptStatus);
                    ledger.ReceptDate = ValueUtil.ToDateTime(data.ReceptDate);
                    ledger.ReceptUserName = ValueUtil.ToString(data.ReceptUserName);
                    ledger.BAOrderStatus = ValueUtil.ParseInt(data.BAOrderStatus);
                    ledger.BAOrderDate = ValueUtil.ToDateTime(data.BAOrderDate);
                    ledger.BAOrderUserName = ValueUtil.ToString(data.BAOrderUserName);
                    ledger.CustomerApplicationStatus = ValueUtil.ParseInt(data.CustomerApplicationStatus);
                    ledger.CustomerApplicationDate = ValueUtil.ToDateTime(data.CustomerApplicationDate);
                    ledger.CustomerApplicationUser = ValueUtil.ToString(data.CustomerApplicationUser);
                    ledger.SubmitReceptStatus = ValueUtil.ParseInt(data.SubmitReceptStatus);
                    ledger.SubmitReceptDate = ValueUtil.ToDateTime(data.SubmitReceptDate);
                    ledger.SubmitReceptUser = ValueUtil.ToString(data.SubmitReceptUser);

                    //  フリー入力が存在していたらフリーを優先する
                    if(string.IsNullOrEmpty(ValueUtil.ToString(data.CustomerFree)))
                    {
                        ledger.CustomerID = ValueUtil.ToString(data.CustomerID);
                    }
                    else
                    {
                        ledger.CustomerFree = ValueUtil.ToString(data.CustomerFree);
                    }

                    if(string.IsNullOrEmpty(ValueUtil.ToString(data.VehicleFree)))
                    {
                        ledger.VehicleID = ValueUtil.ParseInt(data.VehicleID);
                    }
                    else
                    {
                        ledger.VehicleFree = ValueUtil.ToString(data.VehicleFree);
                    }

                    if(string.IsNullOrEmpty(ValueUtil.ToString(data.OrderDestinationFree)))
                    {
                        ledger.OrderDestinationID = ValueUtil.ParseInt(data.OrderDestinationID);
                    }
                    else
                    {
                        ledger.OrderDestinationFree = ValueUtil.ToString(data.OrderDestinationFree);
                    }

                    ledger.ReleaseDate = ValueUtil.ToString(data.ReleaseDate);

                    //  発売日フラグ
                    #region ReleaseDateFlg
                    //  初期値として0を入れておく
                    ledger.ReleaseDateFlg = 0;

                    if(ledger.VehicleID != null)
                    {
                        //  進行スケジュールから媒体誌のスケジュールデータを取得
                        var sch = db.ProgressSchedules.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                           d.VehicleID == ledger.VehicleID);

                        if(sch != null)
                        {
                            var timeTbl = db.ProgressTimetable.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                          d.ProgressScheduleID == sch.ID)
                                                              .OrderByDescending(d => d.ReleaseDate)
                                                              .FirstOrDefault();

                            if(timeTbl != null && timeTbl.ReleaseDate == ValueUtil.ToDateTime(data.ReleaseDate))
                            {
                                //  進行予定表マスタから取得できた
                                ledger.ReleaseDateFlg = 1;
                            }
                            else
                            {
                                //  HEN媒体誌スケジュールからデータを取得
                                var hen = db.VehicleSchedules.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                                  d.VehicleID == ledger.VehicleID);

                                if(hen != null && hen.HENReleaseDate == ValueUtil.ToDateTime(data.ReleaseDate))
                                {
                                    //  HEN媒体誌スケジュールからのデータ
                                    ledger.ReleaseDateFlg = 2;
                                }
                            }
                        }
                    }
                    #endregion

                    //  広告主
                    if(!string.IsNullOrEmpty(ValueUtil.ToString(data.Company)))
                    {
                        //  Companyデータ取得
                        string name = ValueUtil.ToString(data.Company);
                        var com = db.Customers.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                   d.HENDeleteFlg != DBUtil.FLAG_YES &&
                                                                   (d.HENCustomerName == name ||
                                                                   d.HENCustomerAbbrName == name ||
                                                                   d.HENCustomerNameCommon == name));

                        if(com == null)
                        {
                            //  フリーとして登録
                            ledger.AdvertiserFree = name;
                        }
                        else
                        {
                            //  IDを登録
                            ledger.AdvertiserID = com.ID;
                        }
                    }

                    //                ledger.IndustryClassificationID = 0;
                    ledger.Subject = ValueUtil.ToString(data.Subject);
                    ledger.ElectronicMagazine = ((bool)data.ElectronicMagazine) ? 1 : 0;

                    //  ネット金額計算
                    #region prices
                    ledger.RecordingDate = ValueUtil.ToDateTime(data.RecordingDate);
                    ledger.CommissionRate = ValueUtil.ParseDecimal(data.CommissionRate);
                    ledger.PublicationFeeGross = ValueUtil.ParseDecimal(data.PublicationFeeGross);
                    ledger.PublicationFeeRate = ValueUtil.ParseDecimal(data.PublicationFeeRate);
                    ledger.PublicationFeeNet = ValueUtil.GetNetPrice(ledger.PublicationFeeGross, ledger.PublicationFeeRate);
                    ledger.PublicationFeeComment = ValueUtil.ToString(data.PublicationFeeComment);
                    ledger.ProductionFeeGross = ValueUtil.ParseDecimal(data.ProductionFeeGross);
                    ledger.ProductionFeeRate = ValueUtil.ParseDecimal(data.ProductionFeeRate);
                    ledger.ProductionFeeNet = ValueUtil.GetNetPrice(ledger.ProductionFeeGross, ledger.ProductionFeeNet);
                    ledger.ProductionFeeComment = ValueUtil.ToString(data.ProductionFeeComment);
                    ledger.Other1Gross = ValueUtil.ParseDecimal(data.Other1Gross);
                    ledger.Other1Rate = ValueUtil.ParseDecimal(data.Other1Rate);
                    ledger.Other1Net = ValueUtil.GetNetPrice(ledger.Other1Gross, ledger.Other1Rate);
                    ledger.Other1Comment = ValueUtil.ToString(data.Other1Comment);
                    ledger.Other2Gross = ValueUtil.ParseDecimal(data.Other2Gross);
                    ledger.Other2Rate = ValueUtil.ParseDecimal(data.Other2Rate);
                    ledger.Other2Net = ValueUtil.GetNetPrice(ledger.Other2Gross, ledger.Other2Rate);
                    ledger.Other2Comment = ValueUtil.ToString(data.Other2Comment);
                    ledger.Other3Gross = ValueUtil.ParseDecimal(data.Other3Gross);
                    ledger.Other3Rate = ValueUtil.ParseDecimal(data.Other3Rate);
                    ledger.Other3Net = ValueUtil.GetNetPrice(ledger.Other3Gross, ledger.Other3Rate);
                    ledger.Other3Comment = ValueUtil.ToString(data.Other3Comment);
                    ledger.OtherComment = ValueUtil.ToString(data.OtherComment);
                    ledger.MediaRevenueGross = ValueUtil.ParseDecimal(data.MediaRevenueGross);
                    ledger.MediaRevenueRate = ValueUtil.ParseDecimal(data.MediaRevenueRate);
                    ledger.MediaRevenueNet = ValueUtil.GetNetPrice(ledger.MediaRevenueGross, ledger.MediaRevenueRate);
                    ledger.MediaRevenueComment = ValueUtil.ToString(data.MediaRevenueComment);
                    ledger.StrategicCostBGross = ValueUtil.ParseDecimal(data.StrategicCostBGross);
                    ledger.StrategicCostBRate = ValueUtil.ParseDecimal(data.StrategicCostBRate);
                    ledger.StrategicCostBNet = ValueUtil.GetNetPrice(ledger.StrategicCostBGross, ledger.StrategicCostBRate);
                    ledger.StrategicCostBComment = ValueUtil.ToString(data.StrategicCostBComment);
                    ledger.StrategicCostAGross = ValueUtil.ParseDecimal(data.StrategicCostAGross);
                    ledger.StrategicCostARate = ValueUtil.ParseDecimal(data.StrategicCostARate);
                    ledger.StrategicCostANet = ValueUtil.GetNetPrice(ledger.StrategicCostAGross, ledger.StrategicCostARate);
                    ledger.StrategicCostAComment = ValueUtil.ToString(data.StrategicCostAComment);
                    ledger.DeliveryQuotationPriceComment = ValueUtil.ToString(data.DeliveryQuotationPriceComment);
                    ledger.Deadline = ValueUtil.ToDateTime(data.Deadline);
                    ledger.ProgressMemo = ViewBag.ProgressMemo;
                    ledger.PaymentSumComment = ViewBag.PaymentSumComment;

                    ledger.PM = (int)CalcPM(ledger);
                    ledger.BusinessIncome = CalcIncome(ledger).ToString();
                    ledger.BusinessIncomeRare = CalcIncomeRate(ledger).ToString();
                    #endregion

                    ledger.CalibrationDate = ValueUtil.ToDateTime(data.CalibrationDate);
                    ledger.TransmissionDate = ValueUtil.ToDateTime(data.TransmissionDate);
                    ledger.TransmissionMethod = ValueUtil.ParseInt(data.TransmissionMethod);
                    ledger.TransmissionForm = ValueUtil.ParseInt(data.TransmissionForm);
                    ledger.TransmissionFormFree = ValueUtil.ToString(data.TransmissionFormFree);
                    ledger.DocumentType = ValueUtil.ParseInt(data.DocumentType);
                    ledger.PublisherRepresentative = ValueUtil.ToString(data.PublisherRepresentative);
                    ledger.ProgressMemo = ValueUtil.ToString(data.ProgressMemo);
                    ledger.Memo = ValueUtil.ToString(data.Memo);

                    //  DB保存
                    db.ElectronLedger.Add(ledger);
                    DBUtil.InsertData(ref ledger);

                    //  台帳IDを確定させるために
                    //  台帳データは先に登録する
                    db.SaveChanges();
                    #endregion

                    #region 広告値
                    foreach(var item in data.AdvertisementTypes)
                    {
                        if((bool)item.Checked == false)
                        {
                            continue;
                        }

                        AdvertisementValues picture = new AdvertisementValues();
                        AdvertisementValues color = new AdvertisementValues();
                        AdvertisementValues page = new AdvertisementValues();

                        picture.ElectronLedgerID = ledger.ID;
                        color.ElectronLedgerID = ledger.ID;
                        page.ElectronLedgerID = ledger.ID;

                        picture.ItemID = 1;
                        color.ItemID = 2;
                        page.ItemID = 3;

                        //  TypeID
                        string buf = ValueUtil.ToString(item.Advertisement);
                        var typ = db.AdvertisementTypes.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                            d.AdvertisementType == buf);

                        if(typ == null)
                        {
                            continue;
                        }

                        picture.TypeID = typ.ID;
                        color.TypeID = typ.ID;
                        page.TypeID = typ.ID;

                        //  画
                        buf = ValueUtil.ToString(item.Picture);
                        var pic = db.Pictures.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                  d.Picture == buf);

                        if(pic == null)
                        {
                            //  存在しない値はFreeに保存
                            picture.Free = buf;
                        }
                        else
                        {
                            //  IDを保存
                            picture.Value = pic.ID;
                        }

                        //  色
                        buf = ValueUtil.ToString(item.Color);
                        var col = db.Colors.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                d.Color == buf);

                        if(col == null)
                        {
                            color.Free = buf;
                        }
                        else
                        {
                            color.Value = col.ID;
                        }

                        //  ページ
                        buf = ValueUtil.ToString(item.Page);
                        var pag = db.Pages.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                               d.Page == buf);

                        if(pag == null)
                        {
                            page.Free = buf;
                        }
                        else
                        {
                            page.Value = pag.ID;
                        }

                        db.AdvertisementValues.Add(picture);
                        db.AdvertisementValues.Add(color);
                        db.AdvertisementValues.Add(page);

                        DBUtil.InsertData(ref picture);
                        DBUtil.InsertData(ref color);
                        DBUtil.InsertData(ref page);
                    }
                    #endregion

                    #region 台帳データに紐付くデータの保存
                    //  営業担当者
                    foreach(var item in data.SaleRepresentatives)
                    {
                        SaleRepresentatives sale = new SaleRepresentatives();

                        sale.ElectronLedgerID = ledger.ID;
                        sale.UserID = ValueUtil.ToString(item);

                        ret = SetUserData(db, sale);
                        if(!ret)
                        {
                            continue;
                        }

                        //  DB保存
                        db.SaleRepresentatives.Add(sale);
                        DBUtil.InsertData(ref sale);
                    }

                    //  雑誌部担当者
                    foreach(var item in data.MagazineRepresentatives)
                    {
                        MagazineRepresentatives mag = new MagazineRepresentatives();

                        mag.ElectronLedgerID = ledger.ID;
                        mag.UserID = ValueUtil.ToString(item);

                        ret = SetUserData(db, mag);
                        if(!ret)
                        {
                            continue;
                        }

                        //  DB保存
                        db.MagazineRepresentatives.Add(mag);
                        DBUtil.InsertData(ref mag);
                    }

                    //  制作担当者
                    foreach(var item in data.ProductionRepresentatives)
                    {
                        ProductionRepresentatives prod = new ProductionRepresentatives();

                        prod.ElectronLedgerID = ledger.ID;
                        prod.UserID = ValueUtil.ToString(item);

                        ret = SetUserData(db, prod);
                        if(!ret)
                        {
                            continue;
                        }

                        //  DB保存
                        db.ProductionRepresentatives.Add(prod);
                        DBUtil.InsertData(ref prod);
                    }

                    //  進行部担当者
                    foreach(var item in data.ProgressRepresentatives)
                    {
                        ProgressRepresentatives prog = new ProgressRepresentatives();

                        prog.ElectronLedgerID = ledger.ID;
                        prog.UserID = ValueUtil.ToString(item);

                        ret = SetUserData(db, prog);
                        if(!ret)
                        {
                            continue;
                        }

                        //  DB保存
                        db.ProgressRepresentatives.Add(prog);
                        DBUtil.InsertData(ref prog);
                    }

                    //  BA担当者
                    foreach(var item in data.BARepresentatives)
                    {
                        BARepresentatives ba = new BARepresentatives();

                        ba.ElectronLedgerID = ledger.ID;
                        ba.UserID = ValueUtil.ToString(item);

                        ret = SetUserData(db, ba);
                        if(!ret)
                        {
                            continue;
                        }

                        //  DB保存
                        db.BARepresentatives.Add(ba);
                        DBUtil.InsertData(ref ba);
                    }

                    //  メール送信
                    foreach(var item in data.MailDestinations)
                    {
                        MailDestinations mail = new MailDestinations();

                        mail.ElectronLedgerID = ledger.ID;
                        mail.UserID = ValueUtil.ToString(item);

                        ret = SetUserData(db, mail);
                        if(!ret)
                        {
                            continue;
                        }

                        //  DB保存
                        db.MailDestinations.Add(mail);
                        DBUtil.InsertData(ref mail);
                    }

                    //  更新情報メール
                    MailContents mcont = new MailContents();

                    mcont.ElectronLedgerID = ledger.ID;
                    mcont.Subject = ValueUtil.ToString(data.MailSubject);
                    mcont.Text = ValueUtil.ToString(data.MailBody);

                    List<string> addr = new List<string>();

                    //  ユーザIDからメールアドレスを取得
                    foreach(var item in data.MailContents)
                    {
                        string buf = ValueUtil.ToString(item);
                        var user = db.Users.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                d.ID == buf);

                        if(user == null || string.IsNullOrEmpty(user.Email))
                        {
                            continue;
                        }

                        addr.Add(user.Email);
                    }

                    //  カンマ区切りにして保存
                    if(addr.Any())
                    {
                        mcont.Destination = addr.Aggregate((m, n) => m + "," + n);
                    }

                    db.MailContents.Add(mcont);
                    DBUtil.InsertData(ref mcont);
                    #endregion

                    //  データベース更新
                    db.SaveChanges();
                }
                catch
                {
                }
            }

            var success = new
            {
                Date = DateTime.Now.ToLongTimeString(),
                Reesult = true,
                Reason = "成功しました。",
            };

            return Json(success, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Upload
        [HttpPost]
        public ActionResult UploadFiles()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/App_Data/Upload"), fname);
                        file.SaveAs(fname);
                    }

                    // Read excel content and return data to client


                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }
        #endregion

        /// <summary>
        /// 基本データの設定
        /// </summary>
        private void SetBaseData()
        {
            using(HMPZDDB db = new HMPZDDB())
            {
                #region DropDownList items

                ViewBag.TeamItems = new List<SelectListItem>();
                ViewBag.CustomerIDItems = new List<SelectListItem>();
                ViewBag.VehicleIDItems = new List<SelectListItem>();

                if(ViewBag.Department != null)
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

                if(ViewBag.TeamID != null)
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

                if(ViewBag.CustomerID != null)
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

                if(perm == null)
                {
                    perm = new UserPermissions();
                    perm.OperableDepartment = "1,2,3";
                }

                //  権限を取得
                var permissions = perm.OperableDepartment.Split(',')
                                                         .Select(d => int.Parse(d.Trim()));

                if(permissions.Any(d => d == (int)DepartmentType.Magazine1 || d > (int)DepartmentType.Magazine3))
                {
                    //  雑誌1部
                    tmp.Add(GetDepartmentData("雑誌一部"));
                }

                if(permissions.Any(d => d == (int)DepartmentType.Magazine2 || d > (int)DepartmentType.Magazine3))
                {
                    //  雑誌2部
                    tmp.Add(GetDepartmentData("雑誌二部"));
                }

                if(permissions.Any(d => d == (int)DepartmentType.Magazine3 || d > (int)DepartmentType.Magazine3))
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
            using(HMPZDDB db = new HMPZDDB())
            {
                var org = db.Organizations.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                               d.OrganizationCommonName == departmentName);

                if(org == null)
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

        /// <summary>
        /// 詳細・編集画面のデータ取得・設定
        /// </summary>
        /// <param name="id">台帳ID</param>
        private bool SetData(int id)
        {
            using(HMPZDDB db = new HMPZDDB())
            {
                var ledger = db.ElectronLedger.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                   d.ID == id);

                if(ledger == null)
                {
                    //  データがなかった
                    return false;
                }

                //  DateTimeを文字列に変換
                Func<DateTime?, string> FormatDateTime = (d) =>
                {
                    if(!d.HasValue)
                    {
                        return "";
                    }

                    return d.Value.ToString("yyyy/MM/dd");
                };

                ViewBag.ID = ledger.ID;
                ViewBag.ControlNumber = ledger.ControlNumber;
                ViewBag.Status = ledger.Status;
                ViewBag.Type = ledger.Type;
                ViewBag.IssuanceStatus = ledger.IssuanceStatus;
                ViewBag.IssuanceDate = FormatDateTime(ledger.IssuanceDate);
                ViewBag.IssuanceUserName = ledger.IssuanceUserName;
                ViewBag.ReceptStatus = ledger.ReceptStatus;
                ViewBag.ReceptDate = FormatDateTime(ledger.ReceptDate);
                ViewBag.ReceptUserName = ledger.ReceptUserName;
                ViewBag.BAOrderStatus = ledger.BAOrderStatus;
                ViewBag.BAOrderDate = FormatDateTime(ledger.BAOrderDate);
                ViewBag.BAOrderUserName = ledger.BAOrderUserName;
                ViewBag.CustomerApplicationStatus = ledger.CustomerApplicationStatus;
                ViewBag.CustomerApplicationDate = FormatDateTime(ledger.CustomerApplicationDate);
                ViewBag.CustomerApplicationUser = ledger.CustomerApplicationUser;
                ViewBag.SubmitReceptStatus = ledger.SubmitReceptStatus;
                ViewBag.SubmitReceptDate = FormatDateTime(ledger.SubmitReceptDate);
                ViewBag.SubmitReceptUser = ledger.SubmitReceptUser;
                ViewBag.Stop = ledger.Stop;
                ViewBag.StopDate = FormatDateTime(ledger.StopDate);
                ViewBag.StopUser = ledger.StopUser;
                ViewBag.TeamID = ledger.TeamID;
//                ViewBag.Stop = ledger.Stop;
//                ViewBag.StopDate = ledger.StopDate;
//                ViewBag.StopUser = ledger.StopUser;

                ViewBag.Department = string.Empty;

                if(ledger.TeamID.HasValue)
                {
                    //  TeamIDから部署を取得
                    var team = db.Teams.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                            d.ID == ledger.TeamID.Value);

                    if(team != null)
                    {
                        //  チーム情報から部署情報を取得
                        var org = db.Organizations.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                       d.ID == team.OrganizationID);

                        if(org != null)
                        {
                            ViewBag.Department = org.ID;
                        }
                    }
                }

                //  媒体社
                if(!string.IsNullOrEmpty(ledger.CustomerFree))
                {
                    ViewBag.CustomerFree = ledger.CustomerFree;
                }
                else
                {
                    var cust = db.Customers.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                d.HENDeleteFlg != DBUtil.FLAG_YES &&
                                                                d.ID == ledger.CustomerID);

                    if(cust != null)
                    {
                        ViewBag.CustomerID = cust.ID;
                        ViewBag.CustomerName = cust.HENCustomerAbbrName;
                    }
                }

                //  媒体誌
                if(!string.IsNullOrEmpty(ledger.VehicleFree))
                {
                    ViewBag.VehicleFree = ledger.VehicleFree;
                }
                else
                {
                    var veh = db.Vehicles.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                              d.HENDeleteFlg != DBUtil.FLAG_YES &&
                                                              d.ID == ledger.VehicleID);

                    if(veh != null)
                    {
                        ViewBag.VehicleID = veh.ID;
                        ViewBag.VehicleName = veh.HENVehicleName;
                    }
                }

                ViewBag.OrderDestinationID = ledger.OrderDestinationID;
                ViewBag.OrderDestinationFree = ledger.OrderDestinationFree;
                ViewBag.ReleaseDate = ledger.ReleaseDate;
                ViewBag.ReleaseDateFlg = ledger.ReleaseDateFlg;

                //  広告主
                if(!string.IsNullOrEmpty(ledger.AdvertiserFree))
                {
                    ViewBag.AdvertiserName = ledger.AdvertiserFree;
                }
                else
                {
                    var adv = db.Customers.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                               d.HENDeleteFlg != DBUtil.FLAG_YES &&
                                                               d.ID == ledger.AdvertiserID);

                    if(adv != null)
                    {
                        ViewBag.AdvertiserName = adv.HENCustomerAbbrName;
                    }
                }

                //  支払情報
                ViewBag.IndustryClassificationID = ledger.IndustryClassificationID;
                ViewBag.Subject = ledger.Subject;
                ViewBag.ElectronicMagazine = ledger.ElectronicMagazine;
                ViewBag.RecordingDate = FormatDateTime(ledger.RecordingDate);
                ViewBag.CommissionRate = FormatNumber(ledger.CommissionRate);
                ViewBag.PublicationFeeGross = FormatNumber(ledger.PublicationFeeGross);
                ViewBag.PublicationFeeRate = FormatNumber(ledger.PublicationFeeRate);
                ViewBag.PublicationFeeNet = FormatNumber(ledger.PublicationFeeNet, true);
                ViewBag.PublicationFeeComment = ledger.PublicationFeeComment;
                ViewBag.ProductionFeeGross = FormatNumber(ledger.ProductionFeeGross);
                ViewBag.ProductionFeeRate = FormatNumber(ledger.ProductionFeeRate);
                ViewBag.ProductionFeeNet = FormatNumber(ledger.ProductionFeeNet, true);
                ViewBag.ProductionFeeComment = ledger.ProductionFeeComment;
                ViewBag.Other1Gross = FormatNumber(ledger.Other1Gross);
                ViewBag.Other1Rate = FormatNumber(ledger.Other1Rate);
                ViewBag.Other1Net = FormatNumber(ledger.Other1Net, true);
                ViewBag.Other1Comment = ledger.Other1Comment;
                ViewBag.Other2Gross = FormatNumber(ledger.Other2Gross);
                ViewBag.Other2Rate = FormatNumber(ledger.Other2Rate);
                ViewBag.Other2Net = FormatNumber(ledger.Other2Net, true);
                ViewBag.Other2Comment = ledger.Other2Comment;
                ViewBag.Other3Gross = FormatNumber(ledger.Other3Gross);
                ViewBag.Other3Rate = FormatNumber(ledger.Other3Rate);
                ViewBag.Other3Net = FormatNumber(ledger.Other3Net, true);
                ViewBag.Other3Comment = ledger.Other3Comment;
                ViewBag.OtherComment = ledger.OtherComment;
                ViewBag.MediaRevenueGross = FormatNumber(ledger.MediaRevenueGross);
                ViewBag.MediaRevenueRate = FormatNumber(ledger.MediaRevenueRate);
                ViewBag.MediaRevenueNet = FormatNumber(ledger.MediaRevenueNet, true);
                ViewBag.MediaRevenueComment = ledger.MediaRevenueComment;
                ViewBag.StrategicCostBGross = FormatNumber(ledger.StrategicCostBGross);
                ViewBag.StrategicCostBRate = FormatNumber(ledger.StrategicCostBRate);
                ViewBag.StrategicCostBNet = FormatNumber(ledger.StrategicCostBNet, true);
                ViewBag.StrategicCostBComment = ledger.StrategicCostBComment;
                ViewBag.StrategicCostAGross = FormatNumber(ledger.StrategicCostAGross);
                ViewBag.StrategicCostARate = FormatNumber(ledger.StrategicCostARate);
                ViewBag.StrategicCostANet = FormatNumber(ledger.StrategicCostANet, true);
                ViewBag.StrategicCostAComment = ledger.StrategicCostAComment;
                ViewBag.DeliveryQuotationPriceComment = ledger.DeliveryQuotationPriceComment;
                ViewBag.BusinessIncome = ledger.BusinessIncome;
//                ViewBag.BusinessIncomeRare = ledger.BusinessIncomeRare;
                ViewBag.PM = FormatNumber(ledger.PM);
                ViewBag.Memo = ledger.Memo;
                ViewBag.CalibrationDate = FormatDateTime(ledger.CalibrationDate);
                ViewBag.TransmissionDate = FormatDateTime(ledger.TransmissionDate);
                ViewBag.TransmissionMethod = ledger.TransmissionMethod;
                ViewBag.TransmissionForm = ledger.TransmissionForm;
                ViewBag.TransmissionFormFree = ledger.TransmissionFormFree;
                ViewBag.DocumentType = ledger.DocumentType;
                ViewBag.PublisherRepresentative = ledger.PublisherRepresentative;
                ViewBag.ProgressMemo = ledger.ProgressMemo;
                ViewBag.Deadline = FormatDateTime(ledger.Deadline);
                ViewBag.PaymentSumComment = ledger.PaymentSumComment;
                ViewBag.RecordedMemo = ledger.RecordedMemo;

                //  送稿形態名称
                ViewBag.TransmissionFormName = string.Empty;

                if(ledger.TransmissionForm.HasValue)
                {
                    var tran = db.TransmissionForms.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                        d.ID == ledger.TransmissionForm.Value);

                    if(tran != null)
                    {
                        ViewBag.TransmissionFormName = tran.TransmissionForm;
                    }
                }

                //  DBには存在しないが表示させるデータの計算
                ViewBag.OtherGross = FormatNumber(CalcOtherGross(ledger), true);
                ViewBag.OtherNet = FormatNumber(CalcOtherNet(ledger), true);
                ViewBag.PaymentSumGross = FormatNumber(CalcPaymentSumGross(ledger), true);
                ViewBag.PaymentSumNet = FormatNumber(CalcPaymentSumNet(ledger), true);
                ViewBag.DeliveryGross = FormatNumber(CalcDeliveryGross(ledger), true);
                ViewBag.DeliveryNet = FormatNumber(CalcDeliveryNet(ledger), true);

                //  広告値
                var grps = db.AdvertisementValues.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                             d.ElectronLedgerID == id)
                                                 .OrderBy(d => d.ItemID)
                                                 .GroupBy(d => d.TypeID);

                //  広告値リスト作成用匿名クラス
                var advs = Enumerable.Range(0, 0).Select(d => new
                {
                    Name = string.Empty,
                    Picture = string.Empty,
                    Color = string.Empty,
                    Page = string.Empty,
                })
                .ToList();

                foreach(var grp in grps)
                {
                    string name = string.Empty;
                    string picture = string.Empty;
                    string color = string.Empty;
                    string page = string.Empty;
                    var pic = grp.FirstOrDefault(d => d.ItemID == 1);
                    var col = grp.FirstOrDefault(d => d.ItemID == 2);
                    var pag = grp.FirstOrDefault(d => d.ItemID == 3);
                    var adv = grp.First();

                    //  名前
                    var typ = db.AdvertisementTypes.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                        d.ID == adv.TypeID.Value);

                    if(typ != null)
                    {
                        name = typ.AdvertisementType;
                    }

                    //  画
                    if(pic.Value == null)
                    {
                        picture = pic.Free;
                    }
                    else
                    {
                        var mst = db.Pictures.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                  d.ID == pic.Value);

                        if(mst != null)
                        {
                            picture = mst.Picture;
                        }
                    }

                    //  色
                    if(col.Value == null)
                    {
                        color = col.Free;
                    }
                    else
                    {
                        var mst = db.Colors.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                d.ID == col.Value);

                        if(mst != null)
                        {
                            color = mst.Color;
                        }
                    }

                    //  ページ
                    if(pag.Value == null)
                    {
                        page = pag.Free;
                    }
                    else
                    {
                        var mst = db.Pages.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                               d.ID == pag.Value);

                        if(mst != null)
                        {
                            page = mst.Page;
                        }
                    }

                    advs.Add(new
                    {
                        Name = name,
                        Picture = picture,
                        Color = color,
                        Page = page,
                    });
                }

                ViewBag.AdvetisementValues = advs;

                //  営業担当者
                ViewBag.SaleRepresentatives = db.SaleRepresentatives.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                                d.ElectronLedgerID == id)
                                                                    .OrderBy(d => d.ID)
                                                                    .Select(d => new SelectListItem()
                                                                    {
                                                                        Value = d.UserID,
                                                                        Text = d.CompanyName + " " +
                                                                               d.DivisionGroupName + " " +
                                                                               d.DivisionName + " " +
                                                                               d.DepartmentName + " " +
                                                                               d.LastName + " " +
                                                                               d.FirstName,
                                                                    })
                                                                    .ToList();
                //  雑誌部担当者
                ViewBag.MagazineRepresentatives = db.MagazineRepresentatives.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                                        d.ElectronLedgerID == id)
                                                                            .OrderBy(d => d.ID)
                                                                            .Select(d =>  new SelectListItem()
                                                                            { 
                                                                                Value = d.UserID,
                                                                                Text = d.CompanyName + " " +
                                                                                       d.DivisionGroupName + " " +
                                                                                       d.DivisionName + " " +
                                                                                       d.DepartmentName + " " +
                                                                                       d.LastName + " " +
                                                                                       d.FirstName,
                                                                            })
                                                                            .ToList();
                //  制作担当者
                ViewBag.ProductionRepresentatives = db.ProductionRepresentatives.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                                            d.ElectronLedgerID == id)
                                                                                .OrderBy(d => d.ID)
                                                                                .Select(d =>  new SelectListItem()
                                                                                { 
                                                                                    Value = d.UserID,
                                                                                    Text = d.CompanyName + " " +
                                                                                           d.DivisionGroupName + " " +
                                                                                           d.DivisionName + " " +
                                                                                           d.DepartmentName + " " +
                                                                                           d.LastName + " " +
                                                                                           d.FirstName,
                                                                                })
                                                                                .ToList();
                //  進行部担当者
                ViewBag.ProgressRepresentatives = db.ProgressRepresentatives.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                                        d.ElectronLedgerID == id)
                                                                            .OrderBy(d => d.ID)
                                                                            .Select(d =>  new SelectListItem()
                                                                            { 
                                                                                Value = d.UserID,
                                                                                Text = d.CompanyName + " " +
                                                                                       d.DivisionGroupName + " " +
                                                                                       d.DivisionName + " " +
                                                                                       d.DepartmentName + " " +
                                                                                       d.LastName + " " +
                                                                                       d.FirstName,
                                                                            })
                                                                            .ToList();

                //  BA担当者
                ViewBag.BARepresentatives = db.BARepresentatives.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                            d.ElectronLedgerID == id)
                                                                .OrderBy(d => d.ID)
                                                                .Select(d =>  new SelectListItem()
                                                                { 
                                                                    Value = d.UserID,
                                                                    Text = d.CompanyName + " " +
                                                                            d.DivisionGroupName + " " +
                                                                            d.DivisionName + " " +
                                                                            d.DepartmentName + " " +
                                                                            d.LastName + " " +
                                                                            d.FirstName,
                                                                })
                                                                .ToList();

                //  メール送信
                ViewBag.MailDestinations = db.MailDestinations.Where(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                                          d.ElectronLedgerID == id)
                                                              .OrderBy(d => d.ID)
                                                              .Select(d => new
                                                              {
                                                                  Name = d.LastName + " " + d.FirstName,
                                                                  DepartmentName = d.DivisionName,
                                                                  Email = d.Email,
                                                              })
                                                              .ToList();

                //  送稿件数
                var trans = from d1 in db.TransmissionCount
                            join d2 in db.TransmissionForms on d1.TransmissionForm equals d2.ID
                            join d3 in db.Users on d1.TransmissionRepresentativeID equals d3.ID
                            where d1.ElectronLedgerID == ledger.ID
                            orderby d1.TransmissionDate
                            select new
                            {
                                Date = FormatDateTime(d1.TransmissionDate),
                                BA = string.Empty,
                                Name = string.Empty, 
                            };
            }

            return true;
        }

        /// <summary>
        /// Usersテーブルの情報を取得して入力
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// Lấy dữ liệu user
        private bool SetUserData<T>(HMPZDDB db, T data)
        {
            Type typ = data.GetType();
            string refID;

            refID = (string)typ.GetProperty("UserID").GetValue(data);

            var user = db.Users.FirstOrDefault(d => d.DeleteFlg != DBUtil.FLAG_YES &&
                                                    d.ID == refID);

            if(user == null)
            {
                return false;
            }

            typ.GetProperty("CompanyName").SetValue(data, user.CompanyName);
            typ.GetProperty("DivisionGroupName").SetValue(data, user.DivisionGroupName);
            typ.GetProperty("DivisionName").SetValue(data, user.DivisionName);
            typ.GetProperty("DepartmentName").SetValue(data, user.DepartmentName);
            typ.GetProperty("LastName").SetValue(data, user.LastName);
            typ.GetProperty("FirstName").SetValue(data, user.FirstName);

            //  Emailフィールドが存在する場合はデータを入れる
            if(typ.GetProperties().Any(d => d.Name == "Email"))
            {
                typ.GetProperty("Email").SetValue(data, user.Email);
            }

            return true;
        }

        /// <summary>
        /// 値の正当性チェック
        /// </summary>
        /// <param name="data">チェック対象のデータ</param>
        /// <param name="err">結果（エラー情報）</param>
        /// <returns>
        /// true    正しい
        /// false   正しくない（エラー）
        /// </returns>
        private bool CheckValid(dynamic data, out List<string> err)
        {
            err = new List<string>();

            bool ret = true;

            //  必須項目チェック
            if(string.IsNullOrWhiteSpace(ValueUtil.ToString(data.CustomerID)))
            {
                ret = false;
                err.Add("媒体社を選択してください");
            }

            if(string.IsNullOrWhiteSpace(ValueUtil.ToString(data.VehicleID)))
            {
                ret = false;
                err.Add("媒体誌を選択してください");
            }

            if(string.IsNullOrWhiteSpace(ValueUtil.ToString(data.ReleaseDate)))
            {
                ret = false;
                err.Add("発売日を選択してください");
            }

            if(string.IsNullOrWhiteSpace(ValueUtil.ToString(data.Company)))
            {
                ret = false;
                err.Add("広告主を選択してください");
            }

            if(string.IsNullOrWhiteSpace(ValueUtil.ToString(data.RecordingDate)))
            {
                ret = false;
                err.Add("計上年月を選択してください");
            }
            
            return ret;
        }

        #region Calcuration
        /// <summary>
        /// その他金額合計グロス計算
        /// </summary>
        /// <param name="ledger">台帳データ</param>
        /// <returns>その他金額合計グロス</returns>
        private decimal CalcOtherGross(ElectronLedger ledger)
        {
            //  その他金額合計グロス = その他1グロス + その他2グロス + その他3グロス
            return (ledger.Other1Gross ?? 0) + (ledger.Other2Gross ?? 0) + (ledger.Other3Gross ?? 0);
        }

        /// <summary>
        /// その他金額合計ネット計算
        /// </summary>
        /// <param name="ledger">台帳データ</param>
        /// <returns>その他金額合計ネット</returns>
        private decimal CalcOtherNet(ElectronLedger ledger)
        {
            //  その他金額合計ネット = その他1ネット + その他2ネット + その他3ネット
            return (ledger.Other1Net ?? 0) + (ledger.Other2Net ?? 0) + (ledger.Other3Net ?? 0);
        }

        /// <summary>
        /// 払い金額合計グロス計算
        /// </summary>
        /// <param name="ledger">台帳データ</param>
        /// <returns>払い金額合計グロス</returns>
        private decimal CalcPaymentSumGross(ElectronLedger ledger)
        {
            decimal otherGross = CalcOtherGross(ledger);

            //  払い金額合計グロス = その他金額合計グロス + 掲載料グロス + 制作料グロス   
            return otherGross + (ledger.ProductionFeeGross ?? 0) + (ledger.PublicationFeeGross ?? 0);
        }

        /// <summary>
        /// 払い金額合計ネット計算
        /// </summary>
        /// <param name="ledger">台帳データ</param>
        /// <returns>払い金額合計ネット</returns>
        private decimal CalcPaymentSumNet(ElectronLedger ledger)
        {
            decimal otherNet = CalcOtherNet(ledger);

            //  払い金額合計ネット = その他金額合計ネット + 掲載料ネット + 制作料ネット
            return otherNet + (ledger.ProductionFeeNet ?? 0) + (ledger.PublicationFeeNet ?? 0);
        }

        /// <summary>
        /// 引き渡し建値グロス計算
        /// </summary>
        /// <param name="ledger">台帳データ</param>
        /// <returns>引き渡し建値グロス</returns>
        private decimal CalcDeliveryGross(ElectronLedger ledger)
        {
            decimal paymentSumGross = CalcPaymentSumGross(ledger);
            
            //  引き渡し建値グロス = 払い金額合計グロス + (媒体収益グロス - 媒体戦略原価Bグロス - 戦略原価Aグロス)
            return paymentSumGross + ((ledger.MediaRevenueGross ?? 0) - (ledger.StrategicCostBGross ?? 0) - (ledger.StrategicCostAGross ?? 0));
        }

        /// <summary>
        /// 引き渡し建値ネット計算
        /// </summary>
        /// <param name="ledger">台帳データ</param>
        /// <returns>引き渡し建値ネット</returns>
        private decimal CalcDeliveryNet(ElectronLedger ledger)
        {
            decimal paymentSumNet = CalcPaymentSumNet(ledger);

            //  引き渡し建値ネット = 払い金額合計ネット + (媒体収益ネット - 媒体戦略原価Bネット - 戦略原価Aネット)
            return paymentSumNet + ((ledger.MediaRevenueNet ?? 0) - (ledger.StrategicCostBNet ?? 0) - (ledger.StrategicCostANet ?? 0));
        }

        /// <summary>
        /// PM計算
        /// </summary>
        /// <param name="ledger">台帳データ</param>
        /// <returns>PM</returns>
        private decimal CalcPM(ElectronLedger ledger)
        {
            decimal deliveryGross = CalcDeliveryGross(ledger);
            decimal deliveryNet = CalcDeliveryNet(ledger);

            //  PM = 引き渡し建値グロス * コミッション率 + 引き渡し建値ネット
            return deliveryGross * (ledger.CommissionRate ?? 0) + deliveryNet;
        }

        /// <summary>
        /// 営収計算
        /// </summary>
        /// <param name="ledger">台帳データ</param>
        /// <returns>営収</returns>
        private decimal CalcIncome(ElectronLedger ledger)
        {
            decimal deliveryNet = CalcDeliveryNet(ledger);
            decimal PM = CalcPM(ledger);

            //  営収 = PM - 引き渡し建値ネット
            return PM - deliveryNet;
        }

        /// <summary>
        /// 営収率計算
        /// </summary>
        /// <param name="ledger">台帳データ</param>
        /// <returns>営収率</returns>
        private decimal CalcIncomeRate(ElectronLedger ledger)
        {
            decimal ret = 0;
            decimal income = CalcIncome(ledger);
            decimal PM = CalcPM(ledger);

            //  ゼロ割り防止
            if(PM != 0)
            {
                //  営収率 = 営収 / PM
                ret = Math.Round(income / PM);
            }

            return ret;
        }
        #endregion

        /// <summary>
        /// 数値のフォーマット設定
        /// 編集の時はケタ区切りをしない
        /// 詳細の時はケタ区切りを行う
        /// </summary>
        /// <param name="d">処理対象の数値</param>
        /// <param name="force">編集の時にも強制的にケタ区切りを挿入する</param>
        /// <returns>整形後の文字列</returns>
        private string FormatNumber(decimal? d, bool force = false)
        {
            string ret = "0";

            if(!d.HasValue)
            {
                return ret;
            }

            if(ViewBag.ActionType == ActionType.Edit && force == false)
            {
                //  編集の場合はそのまま表示
                ret = d.ToString();
            }
            else if(ViewBag.ActionType == ActionType.Detail || force == true)
            {
                //  詳細の場合はケタ区切りを付けて表示
                ret = string.Format("{0:#,0}", d);
            }

            return ret;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMP_ZD
{
    //  メニュー定義
    public enum HeaderMenu
    {
        /// <summary>トップページ</summary>
        Index,
        /// <summary>集計</summary>
        Aggregate,
        /// <summary>得意先検索</summary>
        Customer,
        /// <summary>雑誌検索</summary>
        MagSearch,
        /// <summary>進行スケジュール</summary>
        Schedule,
        /// <summary>掲載誌申込締切</summary>
        Close,
        /// <summary>お知らせ</summary>
        Information,
        /// <summary>更新履歴</summary>
        History,
        /// <summary>管理者ページ</summary>
        Admin,
    };

    //  台帳種類
    public enum LedgerType
    {
        /// <summary>雑誌</summary>
        Magazine = 1,
        /// <summary>Web</summary>
        Web,
        /// <summary>制作</summary>
        Work,
    }

    /// <summary>
    /// 処理種別
    /// </summary>
    public enum ActionType
    {
        /// <summary>詳細</summary>
        Detail,
        /// <summary>新規登録</summary>
        New,
        /// <summary>編集</summary>
        Edit,
    }

    /// <summary>
    /// 部署
    /// </summary>
    public enum DepartmentType
    {
        /// <summary>雑誌1部</summary>
        Magazine1 = 1,
        /// <summary>雑誌2部</summary>
        Magazine2 = 2,
        /// <summary>雑誌3部</summary>
        Magazine3 = 3,
        /// <summary>業務推進部</summary>
        BizPromo = 4,
        /// <summary>雑誌進行部</summary>
        MagProgress = 5,
        /// <summary>局長席</summary>
        Director = 6,
        /// <summary>営業局</summary>
        Operating = 7,
    }

    public class Declares
    {
        /// <summary>
        /// お知らせのCookie
        /// </summary>
        public static readonly string COOKIE_NAME = "HMP_ZD_Information";
    }
}
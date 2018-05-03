using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using HMP_ZD.Models;

namespace HMP_ZD.Utils
{
    /// <summary>
    /// データベース操作補助クラス
    /// </summary>
    public class DBUtil
    {
        public static readonly int FLAG_YES = 1;
        public static readonly int FLAG_NO = 0;

        /// <summary>
        /// データベースに登録
        /// </summary>
        /// <param name="data">登録対象データ</param>
        /// <returns></returns>
        public static void InsertData<T>(ref T data) where T : class
        {
            Type typ = data.GetType();
            DateTime dtNow = DateTime.Now;
            string username = "USER";

            //  日付・ユーザ情報を設定
            typ.GetProperty("CreatedBy").SetValue(data, username);
            typ.GetProperty("Created").SetValue(data, dtNow);

            typ.GetProperty("UpdatedBy").SetValue(data, username);
            typ.GetProperty("Updated").SetValue(data, dtNow);
        }

        /// <summary>
        /// 更新フィールドの情報更新
        /// </summary>
        /// <param name="data">更新対象データ</param>
        public static void UpdateData<T>(ref T data) where T : class
        {
            Type typ = data.GetType();
            DateTime dtNow = DateTime.Now;
            string username = "USER";

            typ.GetProperty("UpdatedBy").SetValue(data, username);
            typ.GetProperty("Updated").SetValue(data, dtNow);
        }

        /// <summary>
        /// データ削除
        /// 実際には削除フィールドにフラグを立てる
        /// </summary>
        /// <param name="data">処理対象データ</param>
        public static void DeleteData<T>(ref T data) where T : class
        {
            data.GetType().GetProperty("DeleteFlg").SetValue(data, 1);
            UpdateData(ref data);
        }
    }
}
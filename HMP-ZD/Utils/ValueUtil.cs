using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualBasic;

namespace HMP_ZD.Utils
{
    /// <summary>
    /// 各値の変換補助
    /// </summary>
    public class ValueUtil
    {
        /// <summary>
        /// string型変換
        /// </summary>
        /// <param name="obj">変換対象</param>
        /// <returns>
        /// 成功  string型
        /// 失敗  null
        /// Empty値の場合はnullを返す
        /// </returns>
        public static string ToString(object obj)
        {
            string buf = (string)obj;

            if(string.IsNullOrEmpty(buf))
            {
                return null;
            }

            return buf;
        }

        /// <summary>
        /// DateTime型変換
        /// </summary>
        /// <param name="obj">変換対象</param>
        /// <returns>
        /// 成功  DateTime型
        /// 失敗  null
        /// </returns>
        public static DateTime? ToDateTime(object obj)
        {
            bool ret;
            DateTime dt;
            string buf = (string)obj;

            if(string.IsNullOrEmpty(buf))
            {
                return null;
            }

            ret = DateTime.TryParse(buf, out dt);
            if(!ret)
            {
                return null;
            }

            return dt;
        }

        /// <summary>
        /// int変換
        /// </summary>
        /// <param name="obj">変換対象</param>
        /// <returns>
        /// 成功  int型数値
        /// 失敗  null
        /// </returns>
        public static int? ParseInt(object obj)
        {
            bool ret;
            int integer;
            string buf = (string)obj;

            if(string.IsNullOrEmpty(buf))
            {
                return null;
            }

            ret = int.TryParse(buf, out integer);
            if(!ret)
            {
                return null;
            }

            return integer;
        }

        /// <summary>
        /// decimal型のパース
        /// </summary>
        /// <param name="obj">変換対象のオブジェクト（stringを想定）</param>
        /// <returns>
        /// 成功  decimal型の値
        /// 失敗  0
        /// </returns>
        public static decimal? ParseDecimal(object obj)
        {
            bool ret;
            decimal dec;
            string buf = (string)obj;

            if(string.IsNullOrEmpty(buf))
            {
                return null;
            }

            ret = decimal.TryParse((string)obj, out dec);
            if(!ret)
            {
                return null;
            }

            return dec;
        }

        /// <summary>
        /// 台帳の支払情報の「ネット」金額の計算
        /// </summary>
        /// <param name="gross">グロス金額</param>
        /// <param name="rate">料率</param>
        /// <returns>ネット金額</returns>
        public static decimal GetNetPrice(decimal? gross, decimal? rate)
        {
            decimal g = gross ?? 0;
            decimal r = rate ?? 0;

            return Math.Round(g * ((100 - r) / 100));
        }

        /// <summary>
        /// 検索用文字列への変換
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToSearch(string str)
        {
            str = (str ?? "").Trim();

            if (string.IsNullOrWhiteSpace(str))
            {
                return "";
            }

            //    ハイフン統一
            str = str.Replace("‐", "－");
            str = str.Replace("－", "－");
            str = str.Replace("―", "－");
            str = str.Replace("ー", "－");

            str = Strings.StrConv(str, VbStrConv.Uppercase | VbStrConv.Hiragana | VbStrConv.Wide,1041).Replace("　", " ");

            return str;

        }

    }
}
using System;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMP_ZD.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class CommonUtil
    {
        /// <summary>
        /// intへの変換
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        //public static int toInt(object o)
        //{
        //    int res = 0;
        //    if (o == null)
        //    {
        //        return res;
        //    }

        //    if (o is string)
        //    {
        //        int.TryParse((string) o, out res);
        //        return res;
        //    }

        //    return (int) o;
        //}

        /// <summary>
        /// 検索用文字列への変換
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        //public static string toSearch(string str)
        //{
        //    str = (str ?? "").Trim();

        //    if (string.IsNullOrWhiteSpace(str))
        //    {
        //        return "";
        //    }

        //    //    ハイフン統一
        //    str = str.Replace("‐", "－");
        //    str = str.Replace("－", "－");
        //        str = str.Replace("―", "－");
        //    str = str.Replace("ー", "－");

        //    str = Strings.StrConv(str, VbStrConv.Uppercase | VbStrConv.Hiragana | VbStrConv.Wide).Replace("　", " ");

        //    return str;

        //}
    //''' <summary>
    //''' 検索用置換
    //''' </summary>
    //''' <param name="str">String</param>
    //''' <returns></returns>

    //Public Shared Function gfConvertForSearch(ByVal str As String) As String

    //    If String.IsNullOrWhiteSpace(str) Then
    //        Return ""
    //    End If

    //    str = str.Trim

    //    str = StrConv(str, VbStrConv.Uppercase Or VbStrConv.Hiragana Or VbStrConv.Wide).Replace("　", " ")

    //    'ハイフン統一
    //    str = str.Replace("‐", "－")
    //    str = str.Replace("-", "－")
    //    str = str.Replace("－", "－")
    //    str = str.Replace("―", "－")
    //    str = str.Replace("ー", "－")
    //    Return str
    //End Function
    }
}
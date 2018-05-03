using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMP_ZD.Utils
{
    /// <summary>
    /// ログインユーザ情報クラス
    /// </summary>
    public class UserInfo
    {
        /// <summary>ユーザID</summary>
        public string UserID { get; private set; }
        /// <summary>ユーザ名</summary>
        public string UserName { get; private set; }

        public UserInfo()
        {
            GetData();
        }

        private void GetData()
        {
#if DEBUG
            UserName = "テストユーザ";
            UserID = "12345678";
#else
            ;
#endif
        }
    }
}
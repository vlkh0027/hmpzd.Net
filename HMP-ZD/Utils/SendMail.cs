using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMP_ZD.Utils
{
    public class SendMail
    {
/*        private static void SendMail2(BlogLogger logger)
        {
            var body = new StringBuilder();
            body.Append("システム管理者　様<br/><br/>");
            body.Append("バッチ実行が完了いたしました。<br/><br/>");
            body.Append($"■日時：<br/>{DateTime.UtcNow.ToLocal():yyyy/MM/dd HH:mm:ss}<br/><br/>");
            body.Append($"■組織：<br/>{Batch.Organizations:N0} 件<br/><br/>");
            body.Append($"■ユーザー：<br/>{Batch.Users:N0} 件<br/><br/>");
            body.Append($"■施設権限：<br/>{Batch.FacilityPermissions:N0} 件<br/><br/>");
            body.Append($"■処理時間：<br/>{Batch.Time:hh\\:mm\\:ss\\.fff}<br/><br/>");
            body.Append("--------------------------------------------------------<br/>");
            body.Append("このメールはシステムから自動送信しています。");

            EmailProperties prop = new EmailProperties()
            {
                Subject = "【Keep】バッチ実行が完了いたしました",
                Body = body.ToString(),
            };

	        var settings = ConfigurationManager.AppSettings;

	        var siteUri = new Uri(settings["ClientSiteUrl"]);
	        var realm = TokenHelper.GetRealmFromTargetUrl(siteUri);
	        var accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUri.Authority, realm)
		        .AccessToken;
	        using (var ctx = TokenHelper.GetClientContextWithAccessToken(siteUri.ToString(), accessToken))
	        {
                try
                {
                    var emailp = new EmailProperties();
                    var recipients = settings["MailTo"].ToString();
                    var toList = recipients.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach(var i in toList)
                    {
                        ctx.Web.EnsureUser(i);
                    }

                    prop.To = toList;
                    Utility.SendEmail(ctx, prop);
                    ctx.ExecuteQuery();
                }
                catch(Exception ex)
                {
                    logger.Error("Error!!! Failed to send email: {0}", ex.ToString());
                }
	        }
        }
*/
    }
}
//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはテンプレートから生成されました。
//
//     このファイルを手動で変更すると、アプリケーションで予期しない動作が発生する可能性があります。
//     このファイルに対する手動の変更は、コードが再生成されると上書きされます。
// </auto-generated>
//------------------------------------------------------------------------------

namespace HMP_ZD.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Customers
    {
        public string ID { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<int> DeleteFlg { get; set; }
        public Nullable<int> HENDeleteFlg { get; set; }
        public Nullable<int> ExclusionFlg { get; set; }
        public string HENCustomerCode { get; set; }
        public string HENNormalizationKana { get; set; }
        public string HENNormalizationEN { get; set; }
        public string HENCustomerName { get; set; }
        public string HENCustomerAbbrName { get; set; }
        public string HENCustomerNameKana { get; set; }
        public string HENCustomerAbbrNameKana { get; set; }
        public string HENCustomerNameEN { get; set; }
        public string HENCustomerAbbrNameEN { get; set; }
        public string HENCustomerNameCommon { get; set; }
        public string HENCustomerIndustryCode { get; set; }
        public Nullable<int> HENAgencyFlg { get; set; }
        public Nullable<int> HENPublisherFlg { get; set; }
    }
}
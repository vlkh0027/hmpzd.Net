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
    
    public partial class Vehicles
    {
        public int ID { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<int> DeleteFlg { get; set; }
        public Nullable<int> HENDeleteFlg { get; set; }
        public string CustomerID { get; set; }
        public string HENVehicleName { get; set; }
        public string HENVehicleCode { get; set; }
        public string HENVehicleKanaName { get; set; }
    }
}

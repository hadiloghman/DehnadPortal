//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PorShetabLibrary.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SingleChargeTiming
    {
        public int id { get; set; }
        public string mobileNumber { get; set; }
        public Nullable<System.DateTime> timeStartProcessMtnInstallment { get; set; }
        public Nullable<System.DateTime> timeAfterEntity { get; set; }
        public Nullable<System.DateTime> timeAfterWhere { get; set; }
        public Nullable<System.DateTime> timeStartChargeMtnSubscriber { get; set; }
        public Nullable<System.DateTime> timeBeforeHTTPClient { get; set; }
        public Nullable<System.DateTime> timeBeforeSendMTNClient { get; set; }
        public Nullable<System.DateTime> timeAfterSendMTNClient { get; set; }
        public Nullable<System.DateTime> timeBeforeReadStringClient { get; set; }
        public Nullable<System.DateTime> timeAfterReadStringClient { get; set; }
        public Nullable<System.DateTime> timeAfterXML { get; set; }
        public Nullable<System.DateTime> timeFinish { get; set; }
        public Nullable<int> cycleNumber { get; set; }
        public Nullable<int> loopNo { get; set; }
        public Nullable<int> threadNumber { get; set; }
        public Nullable<System.DateTime> timeCreate { get; set; }
        public string guid { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AcharLibrary.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class vw_dailyStatistics
    {
        public long id { get; set; }
        public System.DateTime Date { get; set; }
        public string PersianDate { get; set; }
        public Nullable<int> NumberOfSubscriptions { get; set; }
        public Nullable<int> NumberOfUnSubscriptions { get; set; }
        public Nullable<int> SumOfSinglechargeSuccessfulCharge { get; set; }
        public Nullable<int> NumberOfSubscriptionsPost { get; set; }
        public Nullable<int> NumberOfUnSubscriptionsPost { get; set; }
        public Nullable<int> SumOfSinglechargeSuccessfulChargePost { get; set; }
        public Nullable<int> NumberOfSubscriptionsPre { get; set; }
        public Nullable<int> NumberOfUnSubscriptionsPre { get; set; }
        public Nullable<int> SumOfSinglechargeSuccessfulChargePre { get; set; }
        public Nullable<int> NumberOfSubscriptionsUnknown { get; set; }
        public Nullable<int> NumberOfUnSubscriptionsUnknown { get; set; }
        public Nullable<int> SumOfSinglechargeSuccessfulChargeUnknown { get; set; }
    }
}
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DefendIranLibrary.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AutochargeMessagesBuffer
    {
        public long Id { get; set; }
        public string MobileNumber { get; set; }
        public string Content { get; set; }
        public int ProcessStatus { get; set; }
        public Nullable<System.DateTime> SentDate { get; set; }
        public string PersianSentDate { get; set; }
        public System.DateTime DateAddedToQueue { get; set; }
        public string PersianDateAddedToQueue { get; set; }
        public int MessageType { get; set; }
        public long AggregatorId { get; set; }
        public long ServiceId { get; set; }
        public string ReferenceId { get; set; }
        public Nullable<long> ContentId { get; set; }
        public Nullable<int> MessagePoint { get; set; }
        public Nullable<int> ImiChargeCode { get; set; }
        public Nullable<int> ImiMessageType { get; set; }
        public Nullable<long> SubscriberId { get; set; }
        public string ImiChargeKey { get; set; }
        public string SubUnSubMoMssage { get; set; }
        public Nullable<byte> SubUnSubType { get; set; }
        public Nullable<int> Tag { get; set; }
        public Nullable<int> Price { get; set; }
        public Nullable<bool> DeliveryStatus { get; set; }
        public string DeliveryDescription { get; set; }
        public Nullable<System.DateTime> DateLastTried { get; set; }
        public Nullable<int> RetryCount { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DehnadNotificationService.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SentMessage
    {
        public long Id { get; set; }
        public Nullable<long> ChatId { get; set; }
        public string MobileNumber { get; set; }
        public string Content { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public string PersianDateCreated { get; set; }
        public string UserType { get; set; }
        public string Channel { get; set; }
        public Nullable<bool> IsSent { get; set; }
        public Nullable<System.DateTime> DateSent { get; set; }
        public string TelegramKeyboardData { get; set; }
    }
}

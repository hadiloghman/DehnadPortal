//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SharedLibrary.Models
{
    using System;
    
    public partial class sp_getServerTPS_Result
    {
        public int Id { get; set; }
        public string sourceIPStart { get; set; }
        public string sourceIPEnd { get; set; }
        public Nullable<int> sourceServerId { get; set; }
        public string sourceIdentifier { get; set; }
        public string destinationIP { get; set; }
        public string destinationMask { get; set; }
        public Nullable<int> destinationServerId { get; set; }
        public string methodName { get; set; }
        public Nullable<int> TPS { get; set; }
        public Nullable<int> TPSType { get; set; }
        public Nullable<System.DateTime> startDate { get; set; }
        public Nullable<System.DateTime> endDate { get; set; }
        public Nullable<int> state { get; set; }
        public Nullable<int> action { get; set; }
    }
}

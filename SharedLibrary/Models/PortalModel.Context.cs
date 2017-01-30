﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class PortalEntities : DbContext
    {
        public PortalEntities()
            : base("name=PortalEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<Aggregator> Aggregators { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Operator> Operators { get; set; }
        public virtual DbSet<OperatorsPlan> OperatorsPlans { get; set; }
        public virtual DbSet<OperatorsPrefix> OperatorsPrefixs { get; set; }
        public virtual DbSet<ReceievedMessage> ReceievedMessages { get; set; }
        public virtual DbSet<ReceivedMessagesArchive> ReceivedMessagesArchives { get; set; }
        public virtual DbSet<ServiceInfo> ServiceInfoes { get; set; }
        public virtual DbSet<ServiceKeyword> ServiceKeywords { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Subscriber> Subscribers { get; set; }
        public virtual DbSet<SubscribersHistory> SubscribersHistories { get; set; }
        public virtual DbSet<SubscribersPoint> SubscribersPoints { get; set; }
        public virtual DbSet<vw_ReceivedMessages> vw_ReceivedMessages { get; set; }
        public virtual DbSet<Delivery> Deliveries { get; set; }
        public virtual DbSet<ParidsShortCode> ParidsShortCodes { get; set; }
        public virtual DbSet<vw_DehnadAllSentMessages> vw_DehnadAllSentMessages { get; set; }
    
        public virtual int ArchiveReceivedMessages()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ArchiveReceivedMessages");
        }
    
        public virtual ObjectResult<GetUserLog_Result> GetUserLog(string mobileNumber)
        {
            var mobileNumberParameter = mobileNumber != null ?
                new ObjectParameter("MobileNumber", mobileNumber) :
                new ObjectParameter("MobileNumber", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetUserLog_Result>("GetUserLog", mobileNumberParameter);
        }
    }
}
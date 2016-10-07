﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SepidRoodLibrary.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class SepidRoodEntities : DbContext
    {
        public SepidRoodEntities()
            : base("name=SepidRoodEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AutochargeContent> AutochargeContents { get; set; }
        public virtual DbSet<AutochargeContentsSendedToUser> AutochargeContentsSendedToUsers { get; set; }
        public virtual DbSet<AutochargeMessagesBuffer> AutochargeMessagesBuffers { get; set; }
        public virtual DbSet<AutochargeTimeTable> AutochargeTimeTables { get; set; }
        public virtual DbSet<DailyStatistic> DailyStatistics { get; set; }
        public virtual DbSet<EventbaseContent> EventbaseContents { get; set; }
        public virtual DbSet<EventbaseMessagesBuffer> EventbaseMessagesBuffers { get; set; }
        public virtual DbSet<ImiChargeCode> ImiChargeCodes { get; set; }
        public virtual DbSet<LeagueList> LeagueLists { get; set; }
        public virtual DbSet<MessagesArchive> MessagesArchives { get; set; }
        public virtual DbSet<MessagesMonitoring> MessagesMonitorings { get; set; }
        public virtual DbSet<MessagesTemplate> MessagesTemplates { get; set; }
        public virtual DbSet<OnDemandMessagesBuffer> OnDemandMessagesBuffers { get; set; }
        public virtual DbSet<PointsTable> PointsTables { get; set; }
        public virtual DbSet<ServiceOffReason> ServiceOffReasons { get; set; }
        public virtual DbSet<SubscribersAdditionalInfo> SubscribersAdditionalInfoes { get; set; }
        public virtual DbSet<SubscribersLeague> SubscribersLeagues { get; set; }
        public virtual DbSet<vw_SentMessages> vw_SentMessages { get; set; }
        public virtual DbSet<OperatorsRechargeCode> OperatorsRechargeCodes { get; set; }
        public virtual DbSet<ServiceRechargeKeyword> ServiceRechargeKeywords { get; set; }
        public virtual DbSet<TimedTempMessagesBuffer> TimedTempMessagesBuffers { get; set; }
    
        public virtual int AggregateDailyStatistics(Nullable<System.DateTime> miladiDate, Nullable<long> serviceId)
        {
            var miladiDateParameter = miladiDate.HasValue ?
                new ObjectParameter("MiladiDate", miladiDate) :
                new ObjectParameter("MiladiDate", typeof(System.DateTime));
    
            var serviceIdParameter = serviceId.HasValue ?
                new ObjectParameter("ServiceId", serviceId) :
                new ObjectParameter("ServiceId", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AggregateDailyStatistics", miladiDateParameter, serviceIdParameter);
        }
    
        public virtual int ArchiveMessages()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ArchiveMessages");
        }
    
        public virtual int ChangeMessageStatus(Nullable<int> messageType, Nullable<long> contentId, Nullable<int> tag, string persianDate, Nullable<int> currentStatus, Nullable<int> desiredProcessStatus, Nullable<long> monitoringId)
        {
            var messageTypeParameter = messageType.HasValue ?
                new ObjectParameter("MessageType", messageType) :
                new ObjectParameter("MessageType", typeof(int));
    
            var contentIdParameter = contentId.HasValue ?
                new ObjectParameter("ContentId", contentId) :
                new ObjectParameter("ContentId", typeof(long));
    
            var tagParameter = tag.HasValue ?
                new ObjectParameter("Tag", tag) :
                new ObjectParameter("Tag", typeof(int));
    
            var persianDateParameter = persianDate != null ?
                new ObjectParameter("PersianDate", persianDate) :
                new ObjectParameter("PersianDate", typeof(string));
    
            var currentStatusParameter = currentStatus.HasValue ?
                new ObjectParameter("CurrentStatus", currentStatus) :
                new ObjectParameter("CurrentStatus", typeof(int));
    
            var desiredProcessStatusParameter = desiredProcessStatus.HasValue ?
                new ObjectParameter("DesiredProcessStatus", desiredProcessStatus) :
                new ObjectParameter("DesiredProcessStatus", typeof(int));
    
            var monitoringIdParameter = monitoringId.HasValue ?
                new ObjectParameter("MonitoringId", monitoringId) :
                new ObjectParameter("MonitoringId", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ChangeMessageStatus", messageTypeParameter, contentIdParameter, tagParameter, persianDateParameter, currentStatusParameter, desiredProcessStatusParameter, monitoringIdParameter);
        }
    }
}

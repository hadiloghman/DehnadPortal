//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Portal.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Service
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Service()
        {
            this.RPS_ServiceAdditionalInfo = new HashSet<RPS_ServiceAdditionalInfo>();
            this.ServiceInfoes = new HashSet<ServiceInfo>();
            this.Subscribers = new HashSet<Subscriber>();
        }
    
        public long Id { get; set; }
        public string Name { get; set; }
        public string ServiceCode { get; set; }
        public System.DateTime DateCreated { get; set; }
        public string OnKeywords { get; set; }
        public bool IsServiceActive { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RPS_ServiceAdditionalInfo> RPS_ServiceAdditionalInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ServiceInfo> ServiceInfoes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Subscriber> Subscribers { get; set; }
    }
}

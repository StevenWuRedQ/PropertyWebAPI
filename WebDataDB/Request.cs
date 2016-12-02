//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebDataDB
{
    using System;
    using System.Collections.Generic;
    
    public partial class Request
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Request()
        {
            this.DataRequestLogs = new HashSet<DataRequestLog>();
        }
    
        public long RequestId { get; set; }
        public string JobId { get; set; }
        public Nullable<int> RequestTypeId { get; set; }
        public Nullable<int> Priorty { get; set; }
        public Nullable<System.DateTime> DateTimeSubmitted { get; set; }
        public Nullable<int> AttemptNumber { get; set; }
        public Nullable<System.DateTime> DateTimeStarted { get; set; }
        public Nullable<System.DateTime> DateTimeEnded { get; set; }
        public int RequestStatusTypeId { get; set; }
        public string RequestData { get; set; }
        public string ResponseData { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DataRequestLog> DataRequestLogs { get; set; }
        public virtual DexiRequest DexiRequest { get; set; }
        public virtual RequestStatusType RequestStatusType { get; set; }
        public virtual RequestType RequestType { get; set; }
    }
}
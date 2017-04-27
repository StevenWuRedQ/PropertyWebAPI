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
    
    public partial class DataRequestLog
    {
        public string BBL { get; set; }
        public int RequestTypeId { get; set; }
        public System.DateTime RequestDateTime { get; set; }
        public string ExternalReferenceId { get; set; }
        public Nullable<bool> ServedFromCache { get; set; }
        public Nullable<bool> WebDataRequestMade { get; set; }
        public Nullable<long> RequestId { get; set; }
        public Nullable<int> RequestStatusTypeId { get; set; }
        public string RequestParameters { get; set; }
        public string JobId { get; set; }
        public string AccountId { get; set; }
    
        public virtual Request Request { get; set; }
        public virtual RequestStatusType RequestStatusType { get; set; }
        public virtual RequestType RequestType { get; set; }
    }
}

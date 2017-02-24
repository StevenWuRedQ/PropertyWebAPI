﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class WebDataEntities : DbContext
    {
        public WebDataEntities()
            : base("name=WebDataEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<DataRequestLog> DataRequestLogs { get; set; }
        public virtual DbSet<DOBViolation> DOBViolations { get; set; }
        public virtual DbSet<FannieMortgage> FannieMortgages { get; set; }
        public virtual DbSet<FreddieMortgage> FreddieMortgages { get; set; }
        public virtual DbSet<MortgageServicer> MortgageServicers { get; set; }
        public virtual DbSet<TaxBill> TaxBills { get; set; }
        public virtual DbSet<WaterBill> WaterBills { get; set; }
        public virtual DbSet<DexiRequest> DexiRequests { get; set; }
        public virtual DbSet<DexiRequestType> DexiRequestTypes { get; set; }
        public virtual DbSet<Request> Requests { get; set; }
        public virtual DbSet<RequestStatusType> RequestStatusTypes { get; set; }
        public virtual DbSet<RequestType> RequestTypes { get; set; }
        public virtual DbSet<Zillow> Zillows { get; set; }
        public virtual DbSet<NoticeOfProperyValue> NoticeOfProperyValues { get; set; }
        public virtual DbSet<Mortgage> Mortgages { get; set; }
    }
}

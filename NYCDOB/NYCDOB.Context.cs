﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NYCDOB
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class NYCDOBEntities : DbContext
    {
        public NYCDOBEntities()
            : base("name=NYCDOBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Complaint> Complaints { get; set; }
        public virtual DbSet<Violation> Violations { get; set; }
        public virtual DbSet<vwComplaintsSummary> vwComplaintsSummaries { get; set; }
        public virtual DbSet<vwViolationSummary> vwViolationSummaries { get; set; }
    }
}
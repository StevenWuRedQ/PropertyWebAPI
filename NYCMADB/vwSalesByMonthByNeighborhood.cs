//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NYCMADB
{
    using System;
    using System.Collections.Generic;
    
    public partial class vwSalesByMonthByNeighborhood
    {
        public int PKID { get; set; }
        public string Neighborhood { get; set; }
        public Nullable<System.DateTime> YearMonth { get; set; }
        public Nullable<int> NumberofSales { get; set; }
    }
}
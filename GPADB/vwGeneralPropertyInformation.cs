//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GPADB
{
    using System;
    using System.Collections.Generic;
    
    public partial class vwGeneralPropertyInformation
    {
        public string BBLE { get; set; }
        public string TaxClass { get; set; }
        public Nullable<double> LotFrontage { get; set; }
        public Nullable<double> LotDepth { get; set; }
        public Nullable<double> LotArea { get; set; }
        public string BuildingClass { get; set; }
        public string BuildingClassDescription { get; set; }
        public Nullable<double> BuildingForntage { get; set; }
        public Nullable<double> BuildingDepth { get; set; }
        public Nullable<double> Stories { get; set; }
        public Nullable<double> NumberOfBuildingsOnLot { get; set; }
        public Nullable<double> BuildingGrossArea { get; set; }
        public Nullable<double> BuiltFAR { get; set; }
        public Nullable<double> MaxResidentialFAR { get; set; }
        public Nullable<decimal> UnbuiltArea { get; set; }
        public Nullable<double> YearBuilt { get; set; }
        public Nullable<double> Borough { get; set; }
        public string BoroughName { get; set; }
        public Nullable<double> Block { get; set; }
        public Nullable<double> Lot { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string ZipCode { get; set; }
        public string NeighborhoodName { get; set; }
        public string NTACode { get; set; }
        public string Zoning { get; set; }
        public string UnitNumber { get; set; }
        public Nullable<decimal> East { get; set; }
        public Nullable<decimal> North { get; set; }
        public Nullable<decimal> Longitude { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<double> ResidentialUnits { get; set; }
        public Nullable<int> TaxLienList { get; set; }
        public Nullable<bool> PoolA { get; set; }
        public Nullable<int> TaxLienLPs { get; set; }
        public Nullable<int> TaxLienActiveCases { get; set; }
        public Nullable<int> MortgageForeclosureLPs { get; set; }
        public Nullable<int> MortgageForeclosureActiveCases { get; set; }
        public Nullable<int> COSPresent { get; set; }
        public string FormattedAddress { get; set; }
        public string IsVacant { get; set; }
        public string IsMailingAddressActive { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string County { get; set; }
        public Nullable<decimal> ECBAmountDue { get; set; }
        public Nullable<int> ECBOpenTickets { get; set; }
        public Nullable<decimal> DOBCivilPenaltiesDue { get; set; }
        public Nullable<int> DOBOpenViolations { get; set; }
        public Nullable<int> DOBOpenClass1Violations { get; set; }
        public Nullable<int> HasFannieMaeMortgage { get; set; }
        public Nullable<int> HasFreddieMacMortgage { get; set; }
        public Nullable<int> HasFHAMortgage { get; set; }
        public string LeadGrade { get; set; }
        public decimal Equity { get; set; }
        public decimal LTV { get; set; }
        public Nullable<decimal> UnbuiltAreaAboveThreshold { get; set; }
        public string LotType { get; set; }
        public string Proximity { get; set; }
        public Nullable<int> NumberOfBuildings { get; set; }
        public string Style { get; set; }
        public string ExteriorCondition { get; set; }
        public Nullable<decimal> FinishedSquareFootage { get; set; }
        public Nullable<decimal> UnfinishedSquareFootage { get; set; }
        public Nullable<int> CommercialUnits { get; set; }
        public Nullable<decimal> CommercialSquareFootage { get; set; }
        public string GarageType { get; set; }
        public Nullable<decimal> GarageSquareFootage { get; set; }
        public string BasementGrade { get; set; }
        public Nullable<decimal> BasementSquareFootage { get; set; }
        public string BasementType { get; set; }
        public string ConstructionType { get; set; }
        public string ExteriorWall { get; set; }
        public string StructureType { get; set; }
        public string Grade { get; set; }
        public Nullable<int> IsSRORestricted { get; set; }
        public Nullable<int> IsLandmark { get; set; }
        public string ServicerName { get; set; }
        public Nullable<int> ExistingOrMinMortgageAge { get; set; }
        public Nullable<int> DeedAgeInMonths { get; set; }
        public Nullable<int> LivingOrMinOwnerAge { get; set; }
        public Nullable<decimal> TaxLienSoldAmount { get; set; }
        public Nullable<int> TaxLiensSold { get; set; }
    }
}

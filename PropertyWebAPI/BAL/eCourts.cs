﻿//-----------------------------------------------------------------------
// <copyright file="eCourts.cs" company="Redq Technologies, Inc.">
//     Copyright (c) Redq Technologies, Inc. All rights reserved.
// </copyright>
// <author>Raj Sethi</author>
//-----------------------------------------------------------------------

namespace PropertyWebAPI.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using eCourtsDB;
    using AutoMapper;

    public class MotionDetails: vwMotionExpanded
    {
        // blank class used to hide DB abstractions
    }

    public class AppearanceDetails : vwAppearanceExpanded
    {
        // blank class used to hide DB abstractions
    }

    public class AttorneyDetails : vwAttorneyExpanded
    {
        // blank class used to hide DB abstractions
    }

    public class CaseDetails : vwCaseExpanded
    {
        // blank class used to hide DB abstractions
    }

    public class CaseUpdate : tfnGetCaseUpdates_Result
    {

    }

    public class LPCaseDetails: tfnGetMortgageForeclosureLPsForaProperty_Result
    {

    }

    public class CaseBasicInformation: tfnGetMortgageForeclosureCasesForaProperty_Result
    {

    }

    public class CaseBasicInformationWithBBL: tfnGetNewMortgageForeclosureCases_Result
    {

    }

    public class CaseDataChange: tfnGetCaseColumnChanges_Result
    {

    }



    /// <summary>
    ///     All business level abstractions for both JDLS and CCIs systems from eCourts are defined in this class
    /// </summary>
    public class eCourts
    {
        public static bool IsValidCountyId(string countyId)
        {   uint sInt;
            return string.IsNullOrEmpty(countyId) && countyId.Length==2 && uint.TryParse(countyId, out sInt);
        }

        public static bool IsValidCaseIndexNumber(string caseIndexNumber)
        {
            ulong sLong;
            return string.IsNullOrEmpty(caseIndexNumber) && caseIndexNumber.Length == 11 && ulong.TryParse(caseIndexNumber, out sLong);
        }

        public static vwCaseExpanded GetCaseDetails(string countyId, string caseIndexNumber)
        {
            using (NYCOURTSEntities nycourtsE = new NYCOURTSEntities())
            {
                return Mapper.Map<CaseDetails>(nycourtsE.vwCaseExpandeds.Find(countyId, caseIndexNumber));
            }
        }

        public static List<MotionDetails> GetAllMotionsForACase(string countyId, string caseIndexNumber)
        {
            using (NYCOURTSEntities nycourtsE = new NYCOURTSEntities())
            {
                return Mapper.Map<List<vwMotionExpanded>, List<MotionDetails>>(nycourtsE.vwMotionExpandeds
                                                                                .Where(i => i.CountyId == countyId && i.CaseIndexNumber == caseIndexNumber)
                                                                                .OrderByDescending(m => m.SeqNumber).ToList());
            }
        }

        public static List<AppearanceDetails> GetAllAppearancesForACase(string countyId, string caseIndexNumber)
        {
            using (NYCOURTSEntities nycourtsE = new NYCOURTSEntities())
            {
                return Mapper.Map<List<vwAppearanceExpanded>, List<AppearanceDetails>>(nycourtsE.vwAppearanceExpandeds
                                .Where(i => i.CountyId == countyId && i.CaseIndexNumber == caseIndexNumber)
                                .OrderBy(m => m.AppearanceDate).ToList());
            }
        }

        public static List<AttorneyDetails> GetAllAttorneysForACase(string countyId, string caseIndexNumber)
        {
            using (NYCOURTSEntities nycourtsE = new NYCOURTSEntities())
            {
                return Mapper.Map<List<vwAttorneyExpanded>, List<AttorneyDetails>>(nycourtsE.vwAttorneyExpandeds.Where(i => i.CountyId == countyId && i.CaseIndexNumber == caseIndexNumber)
                                                    .OrderByDescending(m => m.SeqNumber).ToList());
            }
        }

        public static List<CaseUpdate> GetRecordedCaseHistory(string countyId, string caseIndexNumber)
        {
            using (NYCOURTSEntities nycourtsE = new NYCOURTSEntities())
            {
                return Mapper.Map<List<tfnGetCaseUpdates_Result>, List<CaseUpdate>>(nycourtsE.tfnGetCaseUpdates(countyId, caseIndexNumber)
                                .OrderBy(m => m.TransactionDateTime)
                                .ThenBy(m => m.DateTimeProcessed).ToList());
            }
        }
    }

}
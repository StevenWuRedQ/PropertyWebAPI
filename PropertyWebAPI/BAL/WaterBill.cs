﻿//-----------------------------------------------------------------------
// <copyright file="WaterBill.cs" company="Redq Technologies, Inc.">
//     Copyright (c) Redq Technologies, Inc. All rights reserved.
// </copyright>
// <author>Raj Sethi</author>
//-----------------------------------------------------------------------

namespace PropertyWebAPI.BAL
{
    using System;
    using System.Linq;
    using Newtonsoft.Json;
    using WebDataDB;
    using System.IO;
    using System.Text;
    using System.Runtime.Serialization.Json;

    #region Local Helper Classes
    /// <summary>
    /// Helper class used to capture Water bill details and used for serialization into JSON object 
    /// </summary>
    public class WaterBillDetails
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? requestId;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string externalReferenceId;
        public string status;
        public string BBL;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Decimal? billAmount;
    }

    /// <summary>
    /// Helper class used for serialization and deserialization of parameters necessary to get Water bill 
    /// </summary>
    public class WaterBillParameters
    {
        public string BBL;
    }
    #endregion

    /// <summary>
    ///     This class deals with all the details associated with either returning waterbill details or creating the 
    ///     request for getting is scrapped from the web 
    /// </summary>
    public static class WaterBill
    {
        /// <summary>
        ///     This methods converts all paramters required for Water Bill into a JSON object
        /// </summary>
        /// <param name="propertyBBL"></param>
        /// <returns>JSON string</returns>
        public static string ParametersToJSON(string propertyBBL)
        {
            WaterBillParameters waterParams = new WaterBillParameters();
            waterParams.BBL = propertyBBL;
            return JsonConvert.SerializeObject(waterParams);
        }

        /// <summary>
        ///     This method converts a JSON back into WaterBillParameters Object
        /// </summary>
        /// <param name="jsonParameters"></param>
        /// <returns>waterBillParameters</returns>
        public static WaterBillParameters JSONToParameters(string jsonParameters)
        {
            WaterBillParameters waterParams = new WaterBillParameters();
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonParameters));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(waterParams.GetType());
            waterParams = (WaterBillParameters)serializer.ReadObject(ms);
            ms.Close();
            return waterParams;
        }
        /// <summary>
        ///     This method deals with all the details associated with either returning the waterbill details or creating the 
        ///     request for getting is scrapped from the web 
        /// </summary>
        /// <param name="propertyBBL"></param>
        /// <param name="externalReferenceId"></param>
        /// <returns></returns>
        public static WaterBillDetails Get(string propertyBBL, string externalReferenceId)
        {
            WaterBillDetails waterBill = new WaterBillDetails();
            waterBill.externalReferenceId = externalReferenceId;
            waterBill.status = RequestStatus.Pending.ToString();
            waterBill.BBL = propertyBBL;
            waterBill.billAmount = null;

            using (WebDataEntities webDBEntities = new WebDataEntities())
            {
                using (var webDBEntitiestransaction = webDBEntities.Database.BeginTransaction())
                {
                    try
                    {
                        string jsonBillParams = ParametersToJSON(propertyBBL);

                        //check if data available
                        WebDataDB.WaterBill waterBillObj = webDBEntities.WaterBills.FirstOrDefault(i => i.BBL == propertyBBL);

                        // record in database and data is not stale
                        if (waterBillObj != null && DateTime.UtcNow.Subtract(waterBillObj.LastUpdated).Days <= 30)
                        {
                            waterBill.billAmount = waterBillObj.BillAmount;
                            waterBill.status = RequestStatus.Success.ToString();

                            DAL.DataRequestLog.InsertForCacheAccess(webDBEntities, propertyBBL, (int)RequestTypes.WaterBill, externalReferenceId, jsonBillParams);
                        }
                        else
                        {

                            //check if pending request in queue
                            DataRequestLog dataRequestLogObj = DAL.DataRequestLog.GetPendingRequest(webDBEntities, propertyBBL, (int)RequestTypes.WaterBill, jsonBillParams);

                            if (dataRequestLogObj == null) //No Pending Request Create New Request
                            {
                                string requestStr = propertyBBL; // we need a helper class to convert propertyBBL into a correct format so that the webscrapping service can read

                                Request requestObj = DAL.Request.Insert(webDBEntities, requestStr, (int)RequestTypes.WaterBill, null);

                                dataRequestLogObj = DAL.DataRequestLog.InsertForWebDataRequest(webDBEntities, propertyBBL, (int)RequestTypes.WaterBill, requestObj.RequestId,
                                                                                               externalReferenceId, jsonBillParams);

                                waterBill.status = RequestStatus.Pending.ToString();
                                waterBill.requestId = requestObj.RequestId;
                            }
                            else //Pending request in queue
                            {
                                waterBill.status = RequestStatus.Pending.ToString();
                                //Send the RequestId for the pending request back
                                waterBill.requestId = dataRequestLogObj.RequestId;
                                dataRequestLogObj = DAL.DataRequestLog.InsertForWebDataRequest(webDBEntities, propertyBBL, (int)RequestTypes.WaterBill,
                                                                                               dataRequestLogObj.RequestId.GetValueOrDefault(), externalReferenceId, jsonBillParams);
                            }
                        }
                        webDBEntitiestransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        webDBEntitiestransaction.Rollback();
                        waterBill.status = RequestStatus.Error.ToString();
                        //log externalReferenceId error from exception
                    }
                }
            }
            return waterBill;
        }

        /// <summary>
        ///     This method gets the data or current status for a request 
        /// </summary>
        /// <param name="dataRequestLogObj"></param>
        /// <returns></returns>
        public static WaterBillDetails ReRun(DataRequestLog dataRequestLogObj)
        {
            WaterBillDetails waterBill = new WaterBillDetails();
            waterBill.BBL = dataRequestLogObj.BBL;
            waterBill.requestId = dataRequestLogObj.RequestId;
            waterBill.externalReferenceId = dataRequestLogObj.ExternalReferenceId;
            waterBill.status = ((RequestStatus)dataRequestLogObj.RequestStatusTypeId).ToString();
            waterBill.billAmount = null;

            WaterBillParameters taxParams = JSONToParameters(dataRequestLogObj.RequestParameters);

            using (WebDataEntities webDBEntities = new WebDataEntities())
            {
                if (dataRequestLogObj.RequestStatusTypeId == (int)RequestStatus.Success)
                {
                    //check if data available
                    WebDataDB.WaterBill waterBillObj = webDBEntities.WaterBills.FirstOrDefault(i => i.BBL == taxParams.BBL);

                    if (waterBillObj != null && DateTime.UtcNow.Subtract(waterBillObj.LastUpdated).Days <= 30)
                        waterBill.billAmount = waterBillObj.BillAmount;
                    else
                        waterBill.status = RequestStatus.Error.ToString();
                }
            }
            return waterBill;
        }
     
        /// <summary>
        ///     This method updates the WaterBill table based on the information received from the Request Object
        /// </summary>
        /// <param name="requestObj"></param>
        /// <returns></returns>
        public static void UpdateData(Request requestObj)
        {
            using (WebDataEntities webDBEntities = new WebDataEntities())
            {
                using (var webDBEntitiestransaction = webDBEntities.Database.BeginTransaction())
                {
                    try
                    {
                        switch (requestObj.RequestStatusTypeId)
                        {
                            case (int)RequestStatus.Error:
                                DAL.DataRequestLog.SetAsError(webDBEntities, requestObj.RequestId);
                                break;
                            case (int)RequestStatus.Success:
                                {
                                    DataRequestLog dataRequestLogObj = DAL.DataRequestLog.GetFirst(webDBEntities, requestObj.RequestId);
                                    if (dataRequestLogObj == null)
                                        return;
                                    WaterBillParameters waterBillParams = JSONToParameters(dataRequestLogObj.RequestParameters);

                                    //check if data available
                                    WebDataDB.WaterBill waterBillObj = webDBEntities.WaterBills.FirstOrDefault(i => i.BBL == waterBillParams.BBL);
                                    if (waterBillObj != null)
                                    {
                                        waterBillObj.BillAmount = 100; //parse from Request requestObj.ResponseData with helper class
                                        waterBillObj.LastUpdated = requestObj.DateTimeEnded.GetValueOrDefault();
                                    }
                                    else
                                    {
                                        waterBillObj = new WebDataDB.WaterBill();
                                        waterBillObj.BBL = waterBillParams.BBL;
                                        waterBillObj.BillAmount = 100; //parse from Request requestObj.ResponseData with helper class
                                        waterBillObj.LastUpdated = requestObj.DateTimeEnded.GetValueOrDefault();

                                        webDBEntities.WaterBills.Add(waterBillObj);
                                    }

                                    webDBEntities.SaveChanges();

                                    DAL.DataRequestLog.SetAsSuccess(webDBEntities, requestObj.RequestId);
                                    break;
                                }
                            default:
                                break;
                        }

                        webDBEntitiestransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        webDBEntitiestransaction.Rollback();
                        //Log something 
                    }
                }
            }
        }
    }
}
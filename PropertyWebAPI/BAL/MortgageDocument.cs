﻿
namespace PropertyWebAPI.BAL
{
    using System;
    using System.Linq;
    using Newtonsoft.Json;
    using WebDataDB;
    using System.Net;
    using System.Runtime.Serialization;
    using RequestResponseBuilder;
    using RequestResponseBuilder.RequestObjects;
    using RequestResponseBuilder.ResponseObjects;
    using System.Collections.Generic;
    using AutoMapper;
    using System.Data.Entity;
    using Common;
    using System.Data.Entity.Validation;

    public class MortgageDocumentResult : NYCBaseResult
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string mortgageDocumentURI;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public WebDataDB.Mortgage mortgageDetails;
    }

    public class MortgageDocument
    {
        private const int RequestTypeId = (int)RequestTypes.NYCMortgageDocumentDetails;

        /// <summary>
        /// Helper class used for serialization and deserialization of parameters necessary to retrieve data
        /// </summary>
        [DataContract]
        private class Parameters
        {
            [DataMember]
            public string BBL;
            [DataMember]
            public string URI;
        }

        /// <summary>
        ///     This methods converts all parameters required into a JSON object
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="propertyBBL"></param>
        /// <returns>JSON string</returns>
        private static string ParametersToJSON(string propertyBBL, string URI)
        {
            Parameters parameters = new Parameters();
            parameters.URI = URI;
            parameters.BBL= propertyBBL;
            return JsonConvert.SerializeObject(parameters);
        }

        /// <summary>
        ///     This method converts a JSON back into Parameters Object
        /// </summary>
        /// <param name="jsonParameters"></param>
        /// <returns>Parameters</returns>
        private static Parameters JSONToParameters(string jsonParameters)
        {
            return JsonConvert.DeserializeObject<Parameters>(jsonParameters);
        }

        /// <summary>
        ///     Use this method in the controller to log failures that are processed before calling any 
        ///     other business methods of this class
        /// </summary>
        /// <param name="propertyBBL"></param>
        /// <param name="externalReferenceId"></param>
        /// <param name="httpErrorCode"></param>
        /// <returns></returns>
        public static void LogFailure(string propertyBBL, string externalReferenceId, int httpErrorCode)
        {
            DAL.DataRequestLog.InsertForFailure(propertyBBL, RequestTypeId, externalReferenceId, "Error Code: " + ((HttpStatusCode)httpErrorCode).ToString());
        }

        private static WebDataDB.Mortgage CopyData(RequestResponseBuilder.ResponseObjects.Mortgage valObj)
        {
            
            WebDataDB.Mortgage resultObj = new WebDataDB.Mortgage();
            
            resultObj.FHACaseNumber = Conversions.GetSingleValue(valObj.FHACaseNo);
            resultObj.MortgageAmounts = Conversions.Concat(valObj.MortgageAmount);
           
            return resultObj;
        }

        /// <summary>
        ///     This method calls back portal for every log record in the list
        /// </summary>
        private static void MakeCallBacks(Common.Context appContext, List<DataRequestLog> logs, WebDataDB.Mortgage mortgageDocumentResultObj)
        {
            if (!CallingSystem.isAnyCallBack(appContext))
                return;

            var resultObj = new BAL.Results();
            resultObj.mortgageDocumentResult = new MortgageDocumentResult();
            resultObj.mortgageDocumentResult.mortgageDetails = mortgageDocumentResultObj;

            foreach (var rec in logs)
            {
                resultObj.mortgageDocumentResult.BBL = rec.BBL;
                resultObj.mortgageDocumentResult.requestId = rec.RequestId;
                resultObj.mortgageDocumentResult.status = ((RequestStatus)rec.RequestStatusTypeId).ToString();
                resultObj.mortgageDocumentResult.externalReferenceId = rec.ExternalReferenceId;
                CallingSystem.PostCallBack(appContext, resultObj);
            }
        }
        /// <summary>
        ///     This method deals with all the details associated with either returning the Notice Of Property Value details or creating the 
        ///     request for getting the data from the web 
        /// </summary>
        /// <param name="propertyBBL"></param>
        /// <param name="externalReferenceId"></param>
        /// <param name="documentURI"></param>
        /// <returns></returns>
        public static MortgageDocumentResult GetDetails(string propertyBBL, string documentURI, string externalReferenceId)
        {
            MortgageDocumentResult mortgageDocumentResultObj = new MortgageDocumentResult();
            mortgageDocumentResultObj.BBL = propertyBBL;
            mortgageDocumentResultObj.mortgageDocumentURI = documentURI;
            mortgageDocumentResultObj.externalReferenceId = externalReferenceId;
            mortgageDocumentResultObj.status = RequestStatus.Pending.ToString();

            string jsonBillParams = ParametersToJSON(propertyBBL, documentURI);

            using (WebDataEntities webDBEntities = new WebDataEntities())
            {
                using (var webDBEntitiestransaction = webDBEntities.Database.BeginTransaction())
                {
                    try
                    {
                        //check if data available
                        var mortgageDocumentObj = webDBEntities.Mortgages.FirstOrDefault(i => i.BBL==propertyBBL && i.MortgageDocumentURI==documentURI);

                        // record in database and data is not stale
                        if (mortgageDocumentObj != null && DateTime.UtcNow.Subtract(mortgageDocumentObj.LastUpdated).Days <= 30)
                        {
                            mortgageDocumentResultObj.mortgageDetails = mortgageDocumentObj;
                            mortgageDocumentResultObj.status = RequestStatus.Success.ToString();

                            DAL.DataRequestLog.InsertForCacheAccess(webDBEntities, propertyBBL, RequestTypeId, externalReferenceId, jsonBillParams);
                        }
                        else
                        {   //check if pending request in queue
                            DataRequestLog dataRequestLogObj = DAL.DataRequestLog.GetPendingRequest(webDBEntities, propertyBBL, RequestTypeId, jsonBillParams);

                            if (dataRequestLogObj == null) //No Pending Request Create New Request
                            {
                                string requestStr = RequestResponseBuilder.RequestObjects.RequestData.CreateRequestDataForMortgage(documentURI);

                                Request requestObj = DAL.Request.Insert(webDBEntities, requestStr, RequestTypeId, null);

                                dataRequestLogObj = DAL.DataRequestLog.InsertForWebDataRequest(webDBEntities, propertyBBL, RequestTypeId, requestObj.RequestId,
                                                                                               externalReferenceId, jsonBillParams);

                                mortgageDocumentResultObj.status = RequestStatus.Pending.ToString();
                                mortgageDocumentResultObj.requestId = requestObj.RequestId;
                            }
                            else //Pending request in queue
                            {
                                mortgageDocumentResultObj.status = RequestStatus.Pending.ToString();
                                //Send the RequestId for the pending request back
                                mortgageDocumentResultObj.requestId = dataRequestLogObj.RequestId;
                                dataRequestLogObj = DAL.DataRequestLog.InsertForWebDataRequest(webDBEntities, propertyBBL, RequestTypeId,
                                                                                               dataRequestLogObj.RequestId.GetValueOrDefault(), externalReferenceId, jsonBillParams);
                            }
                        }
                        webDBEntitiestransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        webDBEntitiestransaction.Rollback();
                        mortgageDocumentResultObj.status = RequestStatus.Error.ToString();
                        DAL.DataRequestLog.InsertForFailure(propertyBBL, RequestTypeId, externalReferenceId, jsonBillParams);
                        Common.Logs.log().Error(string.Format("Exception encountered processing {0} with externalRefId {1}{2}",
                                                propertyBBL, externalReferenceId, Common.Logs.FormatException(e)));
                    }
                }
            }
            return mortgageDocumentResultObj;
        }

        /// <summary>
        ///     This method gets the data or current status for a request 
        /// </summary>
        /// <param name="dataRequestLogObj"></param>
        /// <returns></returns>
        public static MortgageDocumentResult ReRun(DataRequestLog dataRequestLogObj)
        {
            MortgageDocumentResult resultObj = new MortgageDocumentResult();
            resultObj.BBL = dataRequestLogObj.BBL;
            resultObj.requestId = dataRequestLogObj.RequestId;
            resultObj.externalReferenceId = dataRequestLogObj.ExternalReferenceId;
            resultObj.status = ((RequestStatus)dataRequestLogObj.RequestStatusTypeId).ToString();

            try
            {
                using (WebDataEntities webDBEntities = new WebDataEntities())
                {
                    if (dataRequestLogObj.RequestStatusTypeId == (int)RequestStatus.Success)
                    {
                        Parameters parameters = JSONToParameters(dataRequestLogObj.RequestParameters);
                        //check if data available
                        WebDataDB.Mortgage mortgageDocumentObj = webDBEntities.Mortgages.FirstOrDefault(i => i.BBL == parameters.BBL && i.MortgageDocumentURI == parameters.URI);

                        if (mortgageDocumentObj != null && DateTime.UtcNow.Subtract(mortgageDocumentObj.LastUpdated).Days <= 30)
                            resultObj.mortgageDetails = mortgageDocumentObj;
                        else
                            resultObj.status = RequestStatus.Error.ToString();
                    }
                }
                return resultObj;
            }
            catch (Exception e)
            {
                Common.Logs.log().Error(string.Format("Exception encountered processing request log for {0} with externalRefId {1}{2}",
                                                       dataRequestLogObj.BBL, dataRequestLogObj.ExternalReferenceId, Common.Logs.FormatException(e)));
                return null;
            }
        }

        /// <summary>
        ///     This method updates the Mortgage Servicer table based on the information received from the Request Object
        /// </summary>
        /// <param name="requestObj"></param>
        /// <param name="appContext"></param>
        /// <returns>True if successful else false</returns>
        public static bool UpdateData(Common.Context appContext, Request requestObj)
        {
            using (WebDataEntities webDBEntities = new WebDataEntities())
            {
                using (var webDBEntitiestransaction = webDBEntities.Database.BeginTransaction())
                {
                    try
                    {
                        List<DataRequestLog> logs = null;
                        WebDataDB.Mortgage mortgageDocumentObj = null;

                        switch (requestObj.RequestStatusTypeId)
                        {
                            case (int)RequestStatus.Error:
                                logs = DAL.DataRequestLog.SetAsError(webDBEntities, requestObj.RequestId);
                                break;
                            case (int)RequestStatus.Success:
                                {
                                    DataRequestLog dataRequestLogObj = DAL.DataRequestLog.GetFirst(webDBEntities, requestObj.RequestId);
                                    if (dataRequestLogObj != null)
                                    {
                                        var resultObj = ResponseData.ParseMortgage(requestObj.ResponseData);

                                         Parameters parameters = JSONToParameters(dataRequestLogObj.RequestParameters);
                                        //check if old data in the DB
                                        mortgageDocumentObj = webDBEntities.Mortgages.FirstOrDefault(i => i.BBL == parameters.BBL && i.MortgageDocumentURI==parameters.URI);

                                        if (mortgageDocumentObj != null)
                                        {   //Update data with new results
                                            mortgageDocumentObj = CopyData(resultObj);
                                            mortgageDocumentObj.BBL = parameters.BBL;
                                            mortgageDocumentObj.MortgageDocumentURI = parameters.URI;
                                            mortgageDocumentObj.LastUpdated = requestObj.DateTimeEnded.GetValueOrDefault();
                                            webDBEntities.Mortgages.Attach(mortgageDocumentObj);
                                            webDBEntities.Entry(mortgageDocumentObj).State = EntityState.Modified;
                                        }
                                        else
                                        {   // add an entry into cache or DB
                                            mortgageDocumentObj = CopyData(resultObj);
                                            mortgageDocumentObj.BBL = parameters.BBL;
                                            mortgageDocumentObj.MortgageDocumentURI = parameters.URI;
                                            mortgageDocumentObj.LastUpdated = requestObj.DateTimeEnded.GetValueOrDefault();
                                            webDBEntities.Mortgages.Add(mortgageDocumentObj);
                                        }

                                        webDBEntities.SaveChanges();

                                        logs = DAL.DataRequestLog.SetAsSuccess(webDBEntities, requestObj.RequestId);
                                    }
                                    else
                                        throw (new Exception("Cannot locate Request Log Record(s)"));
                                    break;
                                }
                            default:
                                Common.Logs.log().Warn(string.Format("Update called for a Request Object Id {0} with incorrect Status Id {1}", requestObj.RequestId, requestObj.RequestStatusTypeId));
                                break;
                        }

                        webDBEntitiestransaction.Commit();
                        if (logs != null)
                            MakeCallBacks(appContext, logs, mortgageDocumentObj);
                        return true;
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Common.Logs.log().Error(string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage));
                            }
                        }
                        return false;
                    }
                    catch (Exception e)
                    {
                        webDBEntitiestransaction.Rollback();
                        Common.Logs.log().Error(string.Format("Exception encountered updating request with id {0}{1}", requestObj.RequestId, Common.Logs.FormatException(e)));
                        return false;
                    }
                }
            }
        }
    }
}
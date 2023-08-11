using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Dapper;
using System.ComponentModel;
using DocumentExtraction.BLL.Model;

namespace DocumentExtraction.BLL.Service
{
    public static class DocumentProcessor
    {      
        public static List<WorkItem> GetWorkItemLogos()
        {
            List<WorkItem> workItems = new List<WorkItem>();
            workItems.Add(new WorkItem { WorkItemID = "4059282", Name = "Logo001" });
            workItems.Add(new WorkItem { WorkItemID = "4132416", Name = "Logo002" });
            workItems.Add(new WorkItem { WorkItemID = "4134707", Name = "Logo003" });
            workItems.Add(new WorkItem { WorkItemID = "4240214", Name = "Logo004" });
            return workItems;
        }       
        public static List<WorkItem> GetworItems(string InstanceName,int BatchID,int RowCount)
        {
            //wfyear IS NOT NULL AND object_server = 'IMGSVR' AND
            List<WorkItem> workItems = new List<WorkItem>();
            using (OracleConnection oracleConnection = new OracleConnection(BaseSettings.OracleDbConnection))
            {                 
                string strSql = @"SELECT * FROM (SELECT object_ID AS ID, object_name AS workitemid,batchid AS batchid, CASE WHEN object_class = 'CDESIGN01' THEN 1 ELSE 0 END As workItemType,haspages AS haspages,
                      documentcount AS DocumentCount,documentCount_tiff as TiffDocumentCount, documentCount_nontiff as NonTiffDocumentCount,hasannotation AS hasannotation,haserror AS haserror,isprocessed AS isprocessed,comments AS comments, processed_on AS processedon FROM
                      READONLY.STG_RDPMS_ATTRIBUTES_PROD WHERE  nvl(status, 'PENDING') = 'PENDING' " + " AND nvl(BATCHID,0)="+ BatchID.ToString() + " AND PROCESSINSTANCE='" + InstanceName + "' ORDER BY object_name asc) workitem  ";                
                oracleConnection.Open();
                
                workItems = oracleConnection.Query<WorkItem>(strSql,commandType:CommandType.Text).ToList();
                oracleConnection.Close();
            }
            return workItems;
        }
        public static List<WorkItem> GetworItems(int BatchID, int RowCount)
        {
            List<WorkItem> workItems = new List<WorkItem>();
            using (OracleConnection oracleConnection = new OracleConnection(BaseSettings.OracleDbConnection))
            {
                //object_server = 'IMGSVR' AND
                //('02127ESWM288291')
                //('02101ESWM268420','02248ESWM365122','02127ESWM288733','02126ESWM287521','02127ESWM288267')
                //nvl(status, 'PENDING') = 'PENDING'  AND nvl(batchid, 0) <> 0
                string strSql = @"SELECT * FROM (SELECT object_ID AS ID, object_name AS workitemid,batchid AS batchid, CASE WHEN object_class = 'CDESIGN01' THEN 1 ELSE 0 END As workItemType,haspages AS haspages,
                      documentcount AS DocumentCount,documentCount_tiff as TiffDocumentCount, documentCount_nontiff as NonTiffDocumentCount,hasannotation AS hasannotation,haserror AS haserror,isprocessed AS isprocessed,comments AS comments, processed_on AS processedon FROM
                      READONLY.STG_RDPMS_ATTRIBUTES_PROD WHERE nvl(batchid, 0) <> 0 AND object_id in ('01254ESWM125636','15222ESWM000304')) workitem WHERE rownum <= " + RowCount.ToString();
                oracleConnection.Open();                
                workItems = oracleConnection.Query<WorkItem>(strSql, commandType: CommandType.Text).ToList();
                oracleConnection.Close();
            }
            return workItems;
        }
        public static List<WorkItem> GetworItems(int BatchID)
        {
            List<WorkItem> workItems = new List<WorkItem>();
            using (OracleConnection oracleConnection = new OracleConnection(BaseSettings.OracleDbConnection))
            {
                oracleConnection.Open();
                oracleConnection.FlushCache();
                //wfyear IS NOT NULL AND object_server = 'IMGSVR' AND
                string strSql = @"SELECT object_ID AS ID,  object_name   AS workitemid,batchid   AS batchid, CASE WHEN object_class = 'CDESIGN01' THEN 1 ELSE 0 END As workItemType,haspages AS haspages,
                      documentcount AS DocumentCount,documentCount_tiff as TiffDocumentCount, documentCount_nontiff as NonTiffDocumentCount,hasannotation AS hasannotation,haserror AS haserror,isprocessed AS isprocessed,comments AS comments, processed_on AS processedon FROM
                      READONLY.STG_RDPMS_ATTRIBUTES_PROD WHERE  nvl(status, 'PENDING') = 'PENDING' AND batchid=" + BatchID.ToString() + " ORDER BY object_name asc" ;
                workItems = oracleConnection.Query<WorkItem>(strSql, commandType: CommandType.Text).ToList();
                oracleConnection.Close();
            }
            return workItems;
        }
        public static bool UpdateWorkItem(WorkItem workItem)
        {
            bool IsSuccess = false;
            try
            {            
                using (OracleConnection oracleConnection = new OracleConnection(BaseSettings.OracleDbConnection))
                {
                    string strSql = " UPDATE READONLY.STG_RDPMS_ATTRIBUTES_PROD  SET " +
                                    "  haspages = '" + workItem.HasPages + "'" +
                                    ", hasannotation = '" + workItem.HasAnnotation + "'" +
                                    ", haserror = '" + workItem.HasError + "'" +
                                    ", isprocessed = '" + workItem.IsProcessed + "'" +
                                    ", comments = '" + workItem.Comments + "'" +
                                    ", processed_on = '" + DateTime.Now.ToString("dd-MMM-yyyy") + "'" +
                                    ", status ='" + workItem.Status + "'"+
                                    ", documentCount = " + workItem.DocumentCount +
                                    ", documentCount_tiff = " + workItem.TiffDocumentCount +
                                    ", documentCount_nontiff = " + workItem.NonTiffDocumentCount +
                                    ", documentCount_tiff_wa = " + workItem.TiffDocumentWithAnnotationCount +
                                    " WHERE object_ID = '" + workItem.ID +"'";
                    oracleConnection.Open();
                    OracleTransaction oracleTransaction;
                    oracleTransaction = oracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                    /*DynamicParameters dynamicParameters = new DynamicParameters();                    
                    dynamicParameters.Add("i_work_item_id", workItem.WorkItemID, DbType.String,ParameterDirection.Input);
                    dynamicParameters.Add("i_batch_id", workItem.BatchId, DbType.Int32, ParameterDirection.Input);
                    dynamicParameters.Add("i_has_pages", workItem.HasPages, DbType.String, ParameterDirection.Input);
                    dynamicParameters.Add("i_has_annotation", workItem.HasAnnotation, DbType.String, ParameterDirection.Input);
                    dynamicParameters.Add("i_has_error", workItem.HasError, DbType.String, ParameterDirection.Input);
                    dynamicParameters.Add("i_is_processed", workItem.IsProcessed, DbType.String, ParameterDirection.Input);
                    dynamicParameters.Add("i_comments", workItem.Comments, DbType.String, ParameterDirection.Input);
                    dynamicParameters.Add("i_status", workItem.Status, DbType.String, ParameterDirection.Input);
                    dynamicParameters.Add("i_documentCount", workItem.DocumentCount, DbType.String, ParameterDirection.Input);
                    var result = oracleConnection.Execute("usp_UpdateWorkItems", dynamicParameters, commandType: CommandType.StoredProcedure);*/
                    OracleCommand oracleCommand = new OracleCommand(strSql, oracleConnection);
                    oracleCommand.Transaction = oracleTransaction;
                    var result = oracleCommand.ExecuteNonQuery();
                    oracleTransaction.Commit();
                    IsSuccess = result > 0 ? true:false;
                    oracleConnection.Close();
                }
                }
                catch (Exception ex)
                {
                    throw;
                }
            return IsSuccess;
        }
        public static bool UpdateWorkItemAfterBurning(WorkItem workItem)
        {
            bool IsSuccess = false;
            try
            {
                using (OracleConnection oracleConnection = new OracleConnection(BaseSettings.OracleDbConnection))
                {
                    string strSql = " UPDATE READONLY.STG_RDPMS_ATTRIBUTES_PROD  SET " +                                   
                                    " status ='" + workItem.Status + "'" +
                                    " ,Comments ='" + workItem.Comments + "'" +
                                    " WHERE object_ID = '" + workItem.ID + "'";

                    oracleConnection.Open();
                    OracleTransaction oracleTransaction;
                    oracleTransaction = oracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                    OracleCommand oracleCommand = new OracleCommand(strSql, oracleConnection);
                    oracleCommand.Transaction = oracleTransaction;
                    var result = oracleCommand.ExecuteNonQuery();
                    oracleTransaction.Commit();
                    IsSuccess = result > 0 ? true : false;
                    oracleConnection.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return IsSuccess;
        }
        public static bool UpdateWorkItemAsHold(WorkItem workItem)
        {
            bool IsSuccess = false;
            try
            {
                using (OracleConnection oracleConnection = new OracleConnection(BaseSettings.OracleDbConnection))
                {
                    string strSql = " UPDATE READONLY.STG_RDPMS_ATTRIBUTES_PROD  SET " +
                                    " status ='" + workItem.Status + "'" +
                                    " ,Comments ='" + workItem.Comments + "'" +
                                    " WHERE object_ID = '" + workItem.ID + "'";

                    oracleConnection.Open();
                    OracleTransaction oracleTransaction;
                    oracleTransaction = oracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                    OracleCommand oracleCommand = new OracleCommand(strSql, oracleConnection);
                    oracleCommand.Transaction = oracleTransaction;
                    var result = oracleCommand.ExecuteNonQuery();
                    oracleTransaction.Commit();
                    IsSuccess = result > 0 ? true : false;
                    oracleConnection.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return IsSuccess;
        }
        public static List<WorkItem> GetWorkItemForBurning(string InstanceName, int RowCount, string ProcessedDate)
        {
            List<WorkItem> workItems = new List<WorkItem>();
            //AND PROCESSED_ON ='21-FEB-23'
            using (OracleConnection oracleConnection = new OracleConnection(BaseSettings.OracleDbConnection))
            {

                string strSql = "SELECT * FROM (SELECT object_ID AS ID,object_name AS workitemid,batchid AS batchid,CASE WHEN object_class = 'CDESIGN01' THEN 1 ELSE 0 END As workItemType,haspages AS haspages," +
                                "documentcount AS DocumentCount,documentCount_tiff as TiffDocumentCount, documentCount_nontiff as NonTiffDocumentCount,hasannotation AS hasannotation,haserror AS haserror,isprocessed AS isprocessed,comments AS comments,processed_on AS processedon," +
                                 "status AS status FROM READONLY.STG_RDPMS_ATTRIBUTES_PROD WHERE nvl(batchid, 0) <> 0 AND nvl(isprocessed, 'N') = 'Y' AND nvl(status, 'PENDING') = 'Extracted' AND PROCESSED_ON >='" + ProcessedDate +"' AND PROCESSINSTANCE='" + InstanceName + "'  ORDER BY PROCESSED_ON asc) workitem WHERE rownum <= " + RowCount.ToString();
                oracleConnection.Open();    

                workItems = oracleConnection.Query<WorkItem>(strSql, commandType: CommandType.Text).ToList();
                oracleConnection.Close();
            }
            return workItems;
        }
        public static bool RemoveBatchWorkItem(int BatchID)
        {
            bool IsSuccess = false;
            try
            { 
                using (OracleConnection oracleConnection = new OracleConnection(BaseSettings.OracleDbConnection))
                {
                    oracleConnection.Open();

                    DynamicParameters dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("i_batch_id", BatchID, DbType.Int32, ParameterDirection.Input);
                    var result = oracleConnection.Execute("usp_ClearBatchItems", dynamicParameters, commandType: CommandType.StoredProcedure);
                    IsSuccess = result > 0 ? true : false;
                    oracleConnection.Close();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return IsSuccess;
        }
        public static List<int> GetBatchToProcess(string InstanceName)
        {
            List<int> batches = new List<int>();
            using (OracleConnection oracleConnection = new OracleConnection(BaseSettings.OracleDbConnection))
            {
                string strSql = @"SELECT distinct nvl(BATCHID,0) AS batchID FROM READONLY.STG_RDPMS_ATTRIBUTES_PROD WHERE nvl(status, 'PENDING') = 'PENDING' AND nvl(BATCHID,0)<>0 AND PROCESSINSTANCE='" + InstanceName + "'";
                oracleConnection.Open();
                batches = oracleConnection.Query<int>(strSql, commandType: CommandType.Text).ToList();
                oracleConnection.Close();
            }
            return batches;
        }
        public static List<WorkItem> GetListOfProcessedWorkItemWithOutCount()
        {
            List<WorkItem> workItems = new List<WorkItem>();
            using (OracleConnection oracleConnection = new OracleConnection(BaseSettings.OracleDbConnection))
            {                
                string strSql = @"SELECT * FROM (SELECT object_ID AS ID, object_name AS workitemid,batchid AS batchid, CASE WHEN object_class = 'CDESIGN01' THEN 1 ELSE 0 END As workItemType,haspages AS haspages,
                      documentcount AS DocumentCount,documentCount_tiff as TiffDocumentCount, documentCount_nontiff as NonTiffDocumentCount,hasannotation AS hasannotation,haserror AS haserror,isprocessed AS isprocessed,comments AS comments, processed_on AS processedon FROM
                      READONLY.STG_RDPMS_ATTRIBUTES_PROD WHERE nvl(batchid, 0) <> 0 AND nvl(HASANNOTATION,'N')='Y' AND nvl(DOCUMENTCOUNT_TIFF_WA,0)=0) workitem WHERE rownum <= 2000";
                oracleConnection.Open();
                workItems = oracleConnection.Query<WorkItem>(strSql, commandType: CommandType.Text).ToList();
                oracleConnection.Close();
            }
            return workItems;
        }
        public static List<WorkItem> GetListOfHoldDocs()
        {
            List<WorkItem> workItems = new List<WorkItem>();
            using (OracleConnection oracleConnection = new OracleConnection(BaseSettings.OracleDbConnection))
            {
                string strSql = @"SELECT * FROM (SELECT object_ID AS ID, object_name AS workitemid,batchid AS batchid, CASE WHEN object_class = 'CDESIGN01' THEN 1 ELSE 0 END As workItemType,haspages AS haspages,
                      documentcount AS DocumentCount,documentCount_tiff as TiffDocumentCount, documentCount_nontiff as NonTiffDocumentCount,hasannotation AS hasannotation,haserror AS haserror,isprocessed AS isprocessed,comments AS comments, processed_on AS processedon,status as Status FROM
                      READONLY.STG_RDPMS_ATTRIBUTES_PROD WHERE nvl(batchid, 0) <> 0 AND nvl(HASANNOTATION,'N')='Y' AND nvl(status, 'PENDING') = 'Extracted_Hold' AND ProcessINstance='VM1') workitem ";
                oracleConnection.Open();
                workItems = oracleConnection.Query<WorkItem>(strSql, commandType: CommandType.Text).ToList();
                oracleConnection.Close();
            }
            return workItems;
        }
        public static List<WorkItem> GetListOfCompletedAnnotationDocs()
        {
            List<WorkItem> workItems = new List<WorkItem>();
            using (OracleConnection oracleConnection = new OracleConnection(BaseSettings.OracleDbConnection))
            {
                string strSql = @"SELECT * FROM (SELECT object_ID AS ID, object_name AS workitemid,batchid AS batchid, CASE WHEN object_class = 'CDESIGN01' THEN 1 ELSE 0 END As workItemType,haspages AS haspages,
                      documentcount AS DocumentCount,documentCount_tiff as TiffDocumentCount, documentCount_nontiff as NonTiffDocumentCount,hasannotation AS hasannotation,haserror AS haserror,isprocessed AS isprocessed,comments AS comments, processed_on AS processedon,status as Status FROM
                      READONLY.STG_RDPMS_ATTRIBUTES_PROD WHERE nvl(batchid, 0) <> 0 AND nvl(HASANNOTATION,'N')='Y' AND nvl(status, 'PENDING') = 'Completed' AND ProcessINstance='VM1') workitem ";
                oracleConnection.Open();
                workItems = oracleConnection.Query<WorkItem>(strSql, commandType: CommandType.Text).ToList();
                oracleConnection.Close();
            }
            return workItems;
        }
        public static bool UpdateDocStatustoExtracted(WorkItem workItem)
        {
            bool isUpdated = false;

            try
            {
                using (OracleConnection oracleConnection = new OracleConnection(BaseSettings.OracleDbConnection))
                {
                    string strSql = " UPDATE READONLY.STG_RDPMS_ATTRIBUTES_PROD  SET " +
                                    " COMMENTS= 'Document extracted successfully', STATUS= 'Extracted'" +
                                    " WHERE nvl(batchid, 0) <> 0 AND STATUS='Extracted_Hold' AND nvl(HASANNOTATION,'N')='Y' AND object_ID = '" + workItem.ID + "'";

                    oracleConnection.Open();
                    OracleTransaction oracleTransaction;
                    oracleTransaction = oracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                    OracleCommand oracleCommand = new OracleCommand(strSql, oracleConnection);
                    oracleCommand.Transaction = oracleTransaction;
                    var result = oracleCommand.ExecuteNonQuery();
                    oracleTransaction.Commit();
                    isUpdated = result > 0 ? true : false;
                    oracleConnection.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return isUpdated;
        }
        public static bool UpdateDocCountOfWorkItemWithAnnotation(WorkItem workItem)
        {
            bool isUpdated = false;

            try
            {
                using (OracleConnection oracleConnection = new OracleConnection(BaseSettings.OracleDbConnection))
                {
                    string strSql = " UPDATE READONLY.STG_RDPMS_ATTRIBUTES_PROD  SET " +
                                    " DOCUMENTCOUNT_TIFF_WA =" + workItem.TiffDocumentWithAnnotationCount +
                                    " WHERE nvl(batchid, 0) <> 0 AND nvl(HASANNOTATION,'N')='Y' AND nvl(DOCUMENTCOUNT_TIFF_WA,0)=0 AND object_ID = '" + workItem.ID + "'";

                    oracleConnection.Open();
                    OracleTransaction oracleTransaction;
                    oracleTransaction = oracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                    OracleCommand oracleCommand = new OracleCommand(strSql, oracleConnection);
                    oracleCommand.Transaction = oracleTransaction;
                    var result = oracleCommand.ExecuteNonQuery();
                    oracleTransaction.Commit();
                    isUpdated = result > 0 ? true : false;
                    oracleConnection.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return isUpdated;
        }
        public static List<WorkItemTransaction> GetDocumentsForUpload(int RowCount)
        {
            List<WorkItemTransaction> workItems = new List<WorkItemTransaction>();
            using (OracleConnection oracleConnection = new OracleConnection(BaseSettings.OracleDbConnection))
            {
                string strSql = @"SELECT * FROM (SELECT atrib.OBJECT_ID AS ObjectID,CASE WHEN atrib.OBJECT_CLASS = 'CDESIGN01' THEN 1 ELSE 0 END As workItemType, 'ET1' AS EntityType, tran.BREG_TRANS_TYPE__C AS TransactionType,tran.BREG_BUSINESS_NAME__C AS BusinessName,
  tran.BREG_FILE_NO_SUFFIX__C AS FileNumber,atrib.WFRECEIVE_DATE AS DocumentDate,nvl(tran.BREG_REMARKS__C,'Doc_Title') As DocumentTitle, '' AS BusinessRegistrationDivision,tran.BREG_FILE_SUFFIX__C AS Suffix,'' As ""public"", tran.NAME As TransactionNumber,
  '' As AlphaRange, '' As Quarter, atrib.WFYEAR As ""Year"",'' As PartnershipName, tran.BREG_RECEIVED_DT__C As DocumentDateReceived, '' As AnnualStatementType, '' As AssignmentDate, '' As TradeName, tran.BREG_CERTIFICATE_NO__C As CertificateNumber,
  '' As Assignor, '' As Assignee, '' As ApplicantName, tran.BREG_TRADE_MARK__C As TNName, '' As RegistrationDate,'' As TNNameFirstLetter FROM readonly.transactions_partialcopy_dev tran
INNER JOIN stg_rdpms_attributes_prod atrib ON tran.BREG_LEGACY_WORK_ITEM_ID__C=atrib.WFWorkItem_ID WHERE nvl(atrib.status,'PENDING')='Completed' AND atrib.DocumentCount " + BaseSettings.DocCount +" AND tran.BREG_TRANS_TYPE__C NOT IN ("+ BaseSettings.TransType +") AND nvl(atrib.ISUPLOADED,'N')='N') workitem WHERE rownum <= " + RowCount.ToString();
                //AND atrib.DocumentCount_tiff>0 AND atrib.DocumentCount_tiff_wa >0  AND atrib.DocumentCount_nontiff>0
                oracleConnection.Open();
                workItems = oracleConnection.Query<WorkItemTransaction>(strSql, commandType: CommandType.Text).ToList();
                oracleConnection.Close();
            }
            return workItems;
        }
        public static bool UpdateUploadFlag(WorkItemTransaction workItem)
        {
            bool isUpdated = false;

            try
            {
                using (OracleConnection oracleConnection = new OracleConnection(BaseSettings.OracleDbConnection))
                {
                    string strSql = " UPDATE READONLY.STG_RDPMS_ATTRIBUTES_PROD  SET " +
                                    " ISUPLOADED='Y'" +
                                    ", UPLOADED_ON = '" + DateTime.Now.ToString("dd-MMM-yyyy") + "'" +
                                    " WHERE object_ID = '" + workItem.ObjectID + "'";

                    oracleConnection.Open();
                    OracleTransaction oracleTransaction;
                    oracleTransaction = oracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                    OracleCommand oracleCommand = new OracleCommand(strSql, oracleConnection);
                    oracleCommand.Transaction = oracleTransaction;
                    var result = oracleCommand.ExecuteNonQuery();
                    oracleTransaction.Commit();
                    isUpdated = result > 0 ? true : false;
                    oracleConnection.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return isUpdated;
        }
        public static List<WorkItem> GetListOfErrorItems()
        {
            List<WorkItem> workItems = new List<WorkItem>();
            using (OracleConnection oracleConnection = new OracleConnection(BaseSettings.OracleDbConnection))
            {
                string strSql = @"SELECT * FROM (SELECT object_ID AS ID, object_name AS workitemid,batchid AS batchid, CASE WHEN object_class = 'CDESIGN01' THEN 1 ELSE 0 END As workItemType,haspages AS haspages,
                      documentcount AS DocumentCount,documentCount_tiff as TiffDocumentCount, documentCount_nontiff as NonTiffDocumentCount,hasannotation AS hasannotation,haserror AS haserror,isprocessed AS isprocessed,comments AS comments, processed_on AS processedon FROM
                      READONLY.STG_RDPMS_ATTRIBUTES_PROD WHERE nvl(batchid, 0) <> 0 AND object_ID in ('01036ESWM000504',	'01055ESWM006196',	'08191ESWM101094',	'09337ESWM15FA17',	'15050ESWM000281')) workitem WHERE rownum <= 2000";
                oracleConnection.Open();
                workItems = oracleConnection.Query<WorkItem>(strSql, commandType: CommandType.Text).ToList();
                oracleConnection.Close();
            }
            return workItems;
        }

        public static WorkItem GetworItemsDetails(string objectName)
        {
            List<WorkItem> workItems ;
            using (OracleConnection oracleConnection = new OracleConnection(BaseSettings.OracleDbConnection))
            {

                string strSql = @"SELECT object_ID AS ID, object_name AS workitemid,batchid AS batchid, CASE WHEN object_class = 'CDESIGN01' THEN 1 ELSE 0 END As workItemType,haspages AS haspages,
                      documentcount AS DocumentCount,documentCount_tiff as TiffDocumentCount, documentCount_nontiff as NonTiffDocumentCount,hasannotation AS hasannotation,haserror AS haserror,isprocessed AS isprocessed,comments AS comments, processed_on AS processedon FROM
                      READONLY.STG_RDPMS_ATTRIBUTES_PROD WHERE nvl(batchid, 0) <> 0 AND Comments ='Error while processing the workitem. Exception: Workitem open failed.' AND object_Name ='" + objectName + "'";
                oracleConnection.Open();
                workItems = oracleConnection.Query<WorkItem>(strSql, commandType: CommandType.Text).ToList();
                oracleConnection.Close();
            }
            return workItems[0];
        }

        public static List<WorkItem> GetErrordworItemsDuringExtraction()
        {
            List<WorkItem> workItems;
            using (OracleConnection oracleConnection = new OracleConnection(BaseSettings.OracleDbConnection))
            {

                string strSql = @"SELECT object_ID AS ID, object_name AS workitemid,batchid AS batchid, CASE WHEN object_class = 'CDESIGN01' THEN 1 ELSE 0 END As workItemType,haspages AS haspages,
                      documentcount AS DocumentCount,documentCount_tiff as TiffDocumentCount, documentCount_nontiff as NonTiffDocumentCount,hasannotation AS hasannotation,haserror AS haserror,isprocessed AS isprocessed,comments AS comments, processed_on AS processedon FROM
                      READONLY.STG_RDPMS_ATTRIBUTES_PROD WHERE nvl(batchid, 0) <> 0 AND Comments ='Error while processing the workitem. Exception: Workitem open failed.' AND object_Name IN ('B0216201010004','B0530200910846','B1210200810003')";
                oracleConnection.Open();
                workItems = oracleConnection.Query<WorkItem>(strSql, commandType: CommandType.Text).ToList();
                oracleConnection.Close();
            }
            return workItems;
        }

        public static bool UpdateDocsFromNIC(WorkItem workItem)
        {
            bool IsSuccess = false;
            try
            {
                using (OracleConnection oracleConnection = new OracleConnection(BaseSettings.OracleDbConnection))
                {
                    string strSql = " UPDATE READONLY.STG_RDPMS_ATTRIBUTES_PROD  SET " +
                                    "  haspages = '" + workItem.HasPages + "'" +
                                    ", hasannotation = '" + workItem.HasAnnotation + "'" +
                                    ", haserror = '" + workItem.HasError + "'" +
                                    ", isprocessed = '" + workItem.IsProcessed + "'" +
                                    ", comments = '" + workItem.Comments + "'" +
                                    ", processed_on = '" + DateTime.Now.ToString("dd-MMM-yyyy") + "'" +
                                    ", status ='" + workItem.Status + "'" +
                                    ", documentCount = " + workItem.DocumentCount +
                                    ", documentCount_tiff = " + workItem.TiffDocumentCount +
                                    ", documentCount_nontiff = " + workItem.NonTiffDocumentCount +
                                    ", documentCount_tiff_wa = " + workItem.TiffDocumentWithAnnotationCount +
                                    " WHERE object_ID = '" + workItem.ID + "'";
                    oracleConnection.Open();
                    OracleTransaction oracleTransaction;
                    oracleTransaction = oracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);                   
                    OracleCommand oracleCommand = new OracleCommand(strSql, oracleConnection);
                    oracleCommand.Transaction = oracleTransaction;
                    var result = oracleCommand.ExecuteNonQuery();
                    oracleTransaction.Commit();
                    IsSuccess = result > 0 ? true : false;
                    oracleConnection.Close();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return IsSuccess;
        }

    }   
}

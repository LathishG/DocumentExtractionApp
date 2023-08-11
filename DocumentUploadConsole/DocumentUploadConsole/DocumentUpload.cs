using CsvHelper;
using DocumentExtraction.BLL.Model;
using DocumentExtraction.BLL.Service;
using DocumentProcess.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace DocumentUploadConsole
{
    public class DocumentUpload
    {
        private Logger _logger;
        private DateTime _timeStart;
        private DateTime _timeEnd;
        private int _documentCount;
        private double _timeElapsed;
        private string _strTotalTime;
        private List<List<UploadDocuments>> lstRemainingDocs = new List<List<UploadDocuments>>();
        private List<List<WorkItemTransaction>> lstRemainingtransactionBatches = new List<List<WorkItemTransaction>>();

        private List<WorkItemTransaction> GetWorkItems()
        {
            List<WorkItemTransaction> transactions = new List<WorkItemTransaction>();
            transactions.Add(new WorkItemTransaction
            {
                ObjectID = "00320RBE1102286",
                TransactionType = "T1",
                EntityType = "LLC",
                BusinessName = "Abc5 Corp",
                FileNumber = "78978978",
                DocumentDate = "01/12/2003",
                DocumentTitle = "Article5",
                BusinessRegistrationDivision = "BRD5",
                Suffix = "A5",
                Public = "Y",
                TransactionNumber = "456665465456",
                AlphaRange = "KK",
                Quarter = "Q1",
                Year = "2003",
                PartnershipName = "PartnershipName5",
                DocumentDateReceived = "01/12/2003",
                AnnualStatementType = "A5",
                AssignmentDate = "01/12/2003",
                TradeName = "T5",
                CertificateNumber = "C868687688",
                Assignor = "User9",
                Assignee = "user10",
                ApplicantName = "App Name5",
                TNName = "TName5",
                RegistrationDate = "01/12/2003",
                TNNameFirstLetter = "E"
            });
            transactions.Add(new WorkItemTransaction
            {
                ObjectID = "00320RBE1064812",
                TransactionType = "T2",
                EntityType = "LLC",
                BusinessName = "Abc2 Corp",
                FileNumber = "78978978",
                DocumentDate = "01/12/2003",
                DocumentTitle = "Article5",
                BusinessRegistrationDivision = "BRD5",
                Suffix = "A2",
                Public = "Y",
                TransactionNumber = "456665465456",
                AlphaRange = "LL",
                Quarter = "Q2",
                Year = "2003",
                PartnershipName = "PartnershipName5",
                DocumentDateReceived = "01/12/2003",
                AnnualStatementType = "A5",
                AssignmentDate = "01/12/2003",
                TradeName = "T2",
                CertificateNumber = "C868687688",
                Assignor = "User19",
                Assignee = "user110",
                ApplicantName = "App Name2",
                TNName = "TName2",
                RegistrationDate = "01/12/2003",
                TNNameFirstLetter = "K"
            });
            transactions.Add(new WorkItemTransaction
            {
                ObjectID = "00321RBE1215383",
                TransactionType = "T3",
                EntityType = "LLC",
                BusinessName = "Abc3 Corp",
                FileNumber = "78978978",
                DocumentDate = "01/12/2003",
                DocumentTitle = "Article5",
                BusinessRegistrationDivision = "BRD5",
                Suffix = "A2",
                Public = "Y",
                TransactionNumber = "456665465456",
                AlphaRange = "LL",
                Quarter = "Q3",
                Year = "2003",
                PartnershipName = "PartnershipName5",
                DocumentDateReceived = "01/12/2003",
                AnnualStatementType = "A5",
                AssignmentDate = "01/12/2003",
                TradeName = "T2",
                CertificateNumber = "C868687688",
                Assignor = "User19",
                Assignee = "user110",
                ApplicantName = "App Name2",
                TNName = "TName2",
                RegistrationDate = "01/12/2003",
                TNNameFirstLetter = "K"
            });
            return transactions;
        }
        private List<IAttribute> GenerateMetaData(List<WorkItemTransaction> transactions)
        {
            List<IAttribute> documentMetaData = new List<IAttribute>();

            return documentMetaData;
        }
        public DocumentUpload()
        {
            IntiProcess();
        }
        private void IntiProcess()
        {
            _logger = Logger.Instance;
            _documentCount = 0;
            _logger.CreateLogFile("Document_Upload_console");
            _timeStart = DateTime.Now;            
        }
        private  async Task StartProcessAsync()
        {
            _logger.WriteLine(string.Format(" {0}", "--------------------------------------------------------------------------"));
            _logger.WriteLine(string.Format(" > Process Started @ {0} ", DateTime.Now.ToLongTimeString()));
            _logger.WriteLine(string.Format(" > Reading list of transcations from the database {0} ", DateTime.Now.ToLongTimeString()));
            List<Task> _InitalTaskList = new List<Task>();

            List<WorkItemTransaction> _transactions = DocumentProcessor.GetDocumentsForUpload(Convert.ToInt32(BaseSettings.BatchRecordCount));
            int _totalRecords = Convert.ToInt32(BaseSettings.BatchRecordCount);
            int _maxThreadCount = Convert.ToInt32(BaseSettings.MaxThreadCount);
            int _recordPerBatch = (int)Math.Ceiling(_totalRecords / (double)_maxThreadCount);
            List<List<WorkItemTransaction>> _transactionBatches = _transactions.partition(_recordPerBatch);
            _documentCount = 0;
            _logger.WriteLine(string.Format(" > Creating batches for upload {0} ", DateTime.Now.ToLongTimeString()));

            bool runDatabaseUpdate = false;
            for (int i = 0; i < _maxThreadCount; i++)
            {
                if (i < _transactionBatches.Count)
                {
                    //List<UploadDocuments> _uploadDocuments = ProcessTransactions(_transactionBatches[i]);
                    var uploadDocumentDetails = ProcessTransactions(_transactionBatches[i]);
                    List<UploadDocuments> InitialUploadDocuments = uploadDocumentDetails.Item1;
                    List<UploadDocuments> remainingUploadDocuments = uploadDocumentDetails.Item2;
                    if (remainingUploadDocuments.Count > 0)
                    {
                        lstRemainingDocs.Add(remainingUploadDocuments);
                        lstRemainingtransactionBatches.Add(_transactionBatches[i]);
                    }

                    _documentCount = _documentCount + InitialUploadDocuments.Count + remainingUploadDocuments.Count;
                    if (InitialUploadDocuments.Count > 0)
                    {
                        runDatabaseUpdate = remainingUploadDocuments.Count > 0 ? false : true;

                        List<dynamic> InitialBatchMetaData = InitialUploadDocuments.Select(d => d.MetaData).ToList<dynamic>();
                        List<DocumentDetail> InitialBatchFiles = InitialUploadDocuments.Select(d => d.documentDetail).ToList<DocumentDetail>();
                        string InitialMetaDataFile = String.Format(BaseSettings.MetadataLocation + "/SampleInitalMetaData_Thread_{0}_{1}", i.ToString(), DateTime.Now.ToString("ddMMyyyyhhmmssfff")) + ".csv";
                        bool metaDataFileCreated = GenerateMetaDataFile(InitialBatchMetaData, InitialMetaDataFile);

                        var task = UploadDocs(i, InitialBatchFiles, InitialMetaDataFile, _transactionBatches[i], "Initial", runDatabaseUpdate);
                        _InitalTaskList.Add(task);
                    }

                }
            }
            _logger.WriteLine(string.Format(" > Started Uploading the files @ {0} ", DateTime.Now.ToLongTimeString()));
            await Task.WhenAll(_InitalTaskList);
            _logger.WriteLine(string.Format(" > Completed Uploading the files @ {0} ", DateTime.Now.ToLongTimeString()));

        }
        private  async Task StartProcessSecondaryDocAsync()
        {
            List<Task> _SecondaryTaskList = new List<Task>();
            int _maxThreadCount = Convert.ToInt32(BaseSettings.MaxThreadCount);
            for (int i = 0; i < _maxThreadCount; i++)
            {
                if (i < lstRemainingDocs.Count)
                {
                    if (lstRemainingDocs[i].Count > 0)
                    {
                        List<dynamic> BatchMetaData = lstRemainingDocs[i].Select(d => d.MetaData).ToList<dynamic>();
                        List<DocumentDetail> BatchFiles = lstRemainingDocs[i].Select(d => d.documentDetail).ToList<DocumentDetail>();
                        string MetaDataFile = String.Format(BaseSettings.MetadataLocation + "/SampleSecondaryMetaData_Thread_{0}_{1}", i.ToString(), DateTime.Now.ToString("ddMMyyyyhhmmssfff")) + ".csv";
                        bool metaDataFileCreated = GenerateMetaDataFile(BatchMetaData, MetaDataFile);

                        var Stask = UploadDocs(i, BatchFiles, MetaDataFile, lstRemainingtransactionBatches[i], "Secondary", true);
                        _SecondaryTaskList.Add(Stask);
                    }
                }

            }

            await Task.WhenAll(_SecondaryTaskList);
        }       
        private void CalculateTotalTime()
        {
            _timeEnd = DateTime.Now;
            _timeElapsed = _timeEnd.Subtract(_timeStart).TotalHours;
            TimeSpan span = _timeEnd.Subtract(_timeStart);
            _strTotalTime = String.Format("{0}:{1}:{2}", span.Hours, span.Minutes, span.Seconds);
        }      
        private bool GenerateMetaDataFile(List<dynamic> documentAttributes, string metaDataFilePath)
        {
            bool isFileGenerated = true;
            try
            {
                using (var writer = new StreamWriter(metaDataFilePath))
                {
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecords(documentAttributes);
                    }
                }

            }
            catch (Exception ex)
            {
                isFileGenerated = false;
            }

            return isFileGenerated;
        }
        private bool UpdateUploadFlag(List<WorkItemTransaction> transactions)
        {
            bool IsSuccess = true;
            try
            {
                foreach (WorkItemTransaction tran in transactions)
                {
                    DocumentProcessor.UpdateUploadFlag(tran);
                }
            }
            catch (Exception ex)
            {
                IsSuccess = false;
            }

            return IsSuccess;
        }
        private string GenerateFileName(WorkItemTransaction transaction, int fileIndex, string fileExtension)
        {
            switch (transaction.TransactionType)
            {
                case "CN":
                case "GPSC":
                case "NC":
                case "PDS":
                case "PNC":
                case "PRS":
                    return string.Format("{0}_{1}_{2}{3}", transaction.PartnershipName==null?"": transaction.PartnershipName.Replace(" ", "_").Replace("/", "-").Replace(@"\", "_").Replace(".", "_").Replace("\n", "").Replace(":", "_").Replace("<", "_").Replace("*", "_").Replace("?", "_"), transaction.FileNumber, fileIndex.ToString(), fileExtension);
                case "ANNBX":
                    return string.Format("{0}_{1}_{2}_{3}{4}", transaction.PartnershipName.Replace(" ", "_").Replace("/", "-").Replace(@"\", "_").Replace(".", "_").Replace("\n", "").Replace(":", "_").Replace("<", "_").Replace("*", "_").Replace("?", "_"), transaction.FileNumber, transaction.Year, fileIndex.ToString(), fileExtension);
                case "TNAS":
                case "TNTR":
                    return string.Format("{0}_{1}{2}", transaction.TradeName.Replace(" ", "_").Replace("/", "-").Replace(@"\", "_").Replace(".", "_").Replace("\n", "").Replace(":", "_").Replace("<", "_").Replace("*", "_").Replace("?", "_"), fileIndex.ToString(), fileExtension);
                case "TNNA":
                    return string.Format("{0}_{1}_{2}{3}", transaction.PartnershipName, transaction.DocumentDateReceived, fileIndex.ToString(), fileExtension);
                default:
                    return string.Format("{0}_{1}_{2}_{3}{4}", transaction.ObjectID, transaction.DocumentDate != null ? transaction.DocumentDate.Replace(" ", "_").Replace("/", "-").Replace(@"\", "_") : transaction.DocumentDate, transaction.DocumentTitle != null ? transaction.DocumentTitle.Replace(" ", "_").Replace("/", "-").Replace(@"\", "_").Replace(".", "_").Replace("\n", "").Replace(":", "_").Replace("<", "_").Replace("*", "_").Replace("?", "_") : transaction.DocumentTitle, fileIndex.ToString(), fileExtension);
                    //return string.Format("{0}_{1}{2}",transaction.ObjectID, fileIndex.ToString(),fileExtension);                    
                    //return string.Format("{0}_{1}_{2}_{3}{4}", transaction.ObjectID, transaction.DocumentDate != null ? transaction.DocumentDate.Replace(" ", "_").Replace("/", "-").Replace(@"\", "_") : transaction.DocumentDate, transaction.DocumentTitle != null ? transaction.DocumentTitle.Replace(" ", "_").Replace("/", "-").Replace(@"\", "_") : transaction.DocumentTitle, fileIndex.ToString(), fileExtension);
            }
        }
        private async Task UploadDocs(int i, List<DocumentDetail> documents, string metaDataFile, List<WorkItemTransaction> transactions, string fileType, bool runDatabaseUpdate)
        {
            await Task.Run(() =>
            {
                try
                {
                    ConnectSftp(documents, metaDataFile, fileType);

                    if (runDatabaseUpdate)
                        UpdateUploadFlag(transactions);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
        }
        private Tuple<List<UploadDocuments>, List<UploadDocuments>> ProcessTransactions(List<WorkItemTransaction> transactions)
        {
            List<UploadDocuments> uploadDocuments = new List<UploadDocuments>();
            List<UploadDocuments> uploadMainDocuments = new List<UploadDocuments>();

            string[] documentToUpload;
            bool hasMultipleDocuments = false;
            List<string> lstDocumentWithAnnotation = new List<string>();
            List<string> lstDocumentsWithOutAnnotation = new List<string>();
            List<string> lstBrandingWithAnnotation = new List<string>();
            List<string> lstBrandingWithOutAnnotation = new List<string>();

            List<string> lstOtherTypeDocuments = new List<string>();
            List<string> lstDocumentsToUpload = new List<string>();

            int docIndex = 0;
            DocumentAttributeModel1 documentMetaData;

            foreach (WorkItemTransaction transaction in transactions)
            {
                docIndex = 1;
                documentToUpload = new string[100];
                //_logger.WriteLine(string.Format(" > Reading the documents of workitem ID:- {1} {0} ", DateTime.Now.ToLongTimeString(), transaction.ObjectID));
                if (transaction.WorkItemType == ItemType.Document)
                {
                    lstDocumentWithAnnotation = Directory.GetFiles(BaseSettings.FinalLocation + @"\Document\Pdf_With_Annotation", transaction.ObjectID + "*").ToList<string>();
                    lstDocumentsWithOutAnnotation = Directory.GetFiles(BaseSettings.FinalLocation + @"\Document\Pdf_WithOut_Annotation", transaction.ObjectID + "*").ToList<string>();
                }
                else
                {
                    lstBrandingWithAnnotation = Directory.GetFiles(BaseSettings.FinalLocation + @"\Branding\Pdf_With_Annotation", transaction.ObjectID + "*").ToList<string>();
                    lstBrandingWithOutAnnotation = Directory.GetFiles(BaseSettings.FinalLocation + @"\Branding\Pdf_WithOut_Annotation", transaction.ObjectID + "*").ToList<string>();
                }
                lstOtherTypeDocuments = Directory.GetFiles(BaseSettings.FinalLocation + @"\Document\Other_Type_Document", transaction.ObjectID + "*").ToList<string>();

                if (transaction.WorkItemType == ItemType.Document)
                {
                    if (lstDocumentWithAnnotation.Count > 0)
                        lstDocumentsToUpload.AddRange(lstDocumentWithAnnotation);
                    if (lstDocumentsWithOutAnnotation.Count > 0)
                        lstDocumentsToUpload.AddRange(lstDocumentsWithOutAnnotation);
                }
                else
                {
                    if (lstBrandingWithAnnotation.Count > 0)
                        lstDocumentsToUpload.AddRange(lstBrandingWithAnnotation);
                    if (lstBrandingWithOutAnnotation.Count > 0)
                        lstDocumentsToUpload.AddRange(lstBrandingWithOutAnnotation);
                }

                if (lstOtherTypeDocuments.Count > 0)
                    lstDocumentsToUpload.AddRange(lstOtherTypeDocuments);

                hasMultipleDocuments = lstDocumentsToUpload.Count > 1 ? true : false;

                //Creating the list of  metadata and document to upload
                foreach (string file in lstDocumentsToUpload)
                {
                    if (file != null)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        if (fileInfo != null)
                        {
                            //IAttribute iAttribute = AttributeMetaDataFactory.GetAttributeDataModel(transaction);

                            dynamic fileMetadata = AttributeMetaDataFactory.GetAttributeDataModel(transaction);

                            //DocumentAttributeModel1 fileMetadata = new DocumentAttributeModel1
                            //{
                            //    EntityType = transaction.EntityType,
                            //    BussinessName = transaction.BusinessName,
                            //    FileNumber = transaction.FileNumber,
                            //    DocumentDate = transaction.DocumentDate,
                            //    DocumentTitle = transaction.DocumentTitle,
                            //    TransactionNumber = transaction.TransactionNumber,
                            //    BusinessRegistrationDivision = "DV1",
                            //    Public = "Y",
                            //    Suffix = transaction.Suffix
                            //};
                            //DocumentAttributeModel2 fileMetadata = new DocumentAttributeModel2
                            //{
                            //    AlphaRange = "A-B",
                            //    BusinessRegistrationDivision = "DV1",
                            //    DateReceived = transaction.DocumentDate,
                            //    DocumentType = transaction.TransactionType,
                            //    FileNumber = transaction.FileNumber,
                            //    PartnershipName = "PN",
                            //    Public = "P",
                            //    Quarter = "",
                            //    Suffix = transaction.Suffix,
                            //    TransactionNumber = transaction.TransactionNumber,
                            //    Year = transaction.Year
                            //};

                            fileMetadata.FileName = GenerateFileName(transaction, docIndex, fileInfo.Extension);

                            if ((hasMultipleDocuments && docIndex == 1) || (!hasMultipleDocuments && docIndex == 1))
                                uploadMainDocuments.Add(new UploadDocuments { documentDetail = new DocumentDetail { DocumentName = fileMetadata.FileName, DocumentPath = file }, MetaData = fileMetadata });
                            else
                                uploadDocuments.Add(new UploadDocuments { documentDetail = new DocumentDetail { DocumentName = fileMetadata.FileName, DocumentPath = file }, MetaData = fileMetadata });

                            docIndex++;
                        }
                    }
                }

                lstDocumentsToUpload.Clear();
            }

            return Tuple.Create(uploadMainDocuments, uploadDocuments);
        }
        private bool ConnectSftp(List<DocumentDetail> files, string metaDataFile, string fileType)
        {
            bool isConnected = false;
            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = BaseSettings.SFTPHostName,
                UserName = BaseSettings.SFTPUserName,
                Password = BaseSettings.SFTPPassword,
                SshHostKeyPolicy = SshHostKeyPolicy.GiveUpSecurityAndAcceptAny
                //SshHostKeyFingerprint = BaseSettings.SshHostKey,
                //SshPrivateKeyPath = BaseSettings.SshPrivateKeyPath,
                
            };

            using (Session session = new Session())
            {
                _logger.WriteLine(string.Format(" > Connecting to the SFTP {0} ", DateTime.Now.ToLongTimeString()));
                //session.SessionLogPath = BaseSettings.MetadataLocation;
                session.Open(sessionOptions);
                
                //Uploading documents to sftp
                foreach (DocumentDetail file in files)
                {
                    //_logger.WriteLine(string.Format(" > Uploading the file {1} {0} ", DateTime.Now.ToLongTimeString(), file.DocumentName));
                    //UpdateStatus("Uploading the file " + file.DocumentName);
                    using (FileStream fileStream = new FileStream(file.DocumentPath, FileMode.Open))
                    {
                        if (fileType == "Initial")
                        {
                            if (!session.FileExists(BaseSettings.InitialRemoteFolderLocation + file.DocumentName))
                                session.PutFile(fileStream, BaseSettings.InitialRemoteFolderLocation + file.DocumentName);
                        }
                        else
                        {
                            if (!session.FileExists(BaseSettings.RemoteFolderLocation + file.DocumentName))
                                session.PutFile(fileStream, BaseSettings.RemoteFolderLocation + file.DocumentName);
                        }
                    }
                }

                //Uploading metadata file.
                _logger.WriteLine(string.Format(" > Uploading the metadata file {1} {0} ", DateTime.Now.ToLongTimeString(), metaDataFile));
                //UpdateStatus("Uploading the metadata file ");
                FileInfo metaDataFileInfo = new FileInfo(metaDataFile);
                string remoteFileName = metaDataFileInfo.Name;
                using (FileStream fileStream = new FileStream(metaDataFile, FileMode.Open))
                {
                    if (fileType == "Initial")
                    {
                        session.PutFile(fileStream, BaseSettings.InitialRemoteFolderLocation + remoteFileName);
                    }
                    else
                    {
                        session.PutFile(fileStream, BaseSettings.RemoteFolderLocation + remoteFileName);
                    }
                }
            }

            return isConnected;
        }
        public async Task DoProcess()
        {
            await StartProcessAsync();

            if (lstRemainingDocs.Count > 0)
            {
                await StartProcessSecondaryDocAsync();
            }
        }
        public void EndProcess()
        {
            CalculateTotalTime();
            _logger.WriteLine(string.Format(" > Total documents Uploaded {0}. Total time taken to complete the process is {1}, Process Starts @ {2} and Ends @ {3}", _documentCount.ToString(), _strTotalTime, _timeStart.ToString(), _timeEnd.ToString()));
            _logger.WriteLine(string.Format(" > Process Ended  "));
            _logger.WriteLine(string.Format(" {0}", "--------------------------------------------------------------------------"));
            _logger.CloseLogger();

            GenerateProcessSummary();
        }
        private void GenerateProcessSummary()
        {
            List<DocumentUploadSummary> _processSummary = new List<DocumentUploadSummary>();
            string summaryContent = "";
            string summaryFile = "UploadSummary.json";
            if (File.Exists(BaseSettings.ProcessSummaryLocation + summaryFile))
            {
                summaryContent = File.ReadAllText(BaseSettings.ProcessSummaryLocation + summaryFile);
                if (summaryContent != "")
                    _processSummary = JsonConvert.DeserializeObject<List<DocumentUploadSummary>>(summaryContent);
            }
            else
            {
                File.Create(BaseSettings.ProcessSummaryLocation + summaryFile);
            }
            DocumentUploadSummary summary = new DocumentUploadSummary { DocumentCount = _documentCount.ToString(), ProcessedDate = _timeStart.ToString(), ThreadCount = BaseSettings.MaxThreadCount, TotalTime = _strTotalTime };
            _processSummary.Add(summary);
            
            var processSummary = JsonConvert.SerializeObject(_processSummary, Formatting.Indented);
            using (StreamWriter sw = File.CreateText(BaseSettings.ProcessSummaryLocation + summaryFile))
            {
                sw.WriteLine(processSummary);
            }
        }
    }

    public class DocumentUploadSummary
    {
        public string ProcessedDate { get; set; }
        public string ThreadCount { get; set; }
        public string DocumentCount { get; set; }
        public string TotalTime { get; set; }
    }
}

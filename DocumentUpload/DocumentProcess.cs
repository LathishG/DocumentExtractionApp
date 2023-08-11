using CAL;
using DocumentProcess.Common;
using DocumentExtraction.BLL.Model;
using DocumentExtraction.BLL.Service;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinSCP;
using CsvHelper;
using System.Globalization;

namespace DocumentProcess
{
    public partial class DocumentProcess : Form
    {
        private Logger _logger;
        private DateTime _timeStart;
        private DateTime _timeEnd;
        private int _documentCount;
        private double _timeElapsed;
        private string _strTotalTime;

        private List<List<UploadDocuments>> lstRemainingDocs = new List<List<UploadDocuments>>();
        private List<List<WorkItemTransaction>> lstRemainingtransactionBatches = new List<List<WorkItemTransaction>>();
        public DocumentProcess()
        {
            InitializeComponent();                      
        }
        private async void btnProcess_Click(object sender, EventArgs e)
        { 
           this.IntiProcess();
                     
           if (chkbMultiThread.Checked)
           {
                await StartProcessAsync();

                if(lstRemainingDocs.Count>0)
                {
                    await StartProcessSecondaryDocAsync();
                }
            }
           else
           {
                 StartProcess();
           }
            EndProcess();
        }
        private void IntiProcess()
        {  
            _logger = Logger.Instance;
            _documentCount = 0;
            _logger.CreateLogFile("Document_Upload");
            _timeStart = DateTime.Now;
            lblHostName.Text = BaseSettings.SFTPHostName;
            lblHostName.Update();
            lblRemoteFolder.Text = BaseSettings.RemoteFolderLocation;
            lblRemoteFolder.Update();                        
            UpdateStatus("Idle......");
        }
        private void StartProcess()
        {

            _logger.WriteLine(string.Format(" {0}", "--------------------------------------------------------------------------"));
            _logger.WriteLine(string.Format(" > Process Started @ {0} ", DateTime.Now.ToLongTimeString())); 
            //List<WorkItemTransaction> transactions = this.GetWorkItems();
            _logger.WriteLine(string.Format(" > Reading list of documents from database {0} ", DateTime.Now.ToLongTimeString()));
            UpdateStatus("Reading list of documents from database");
            List<WorkItemTransaction> transactions = DocumentProcessor.GetDocumentsForUpload(Convert.ToInt32(BaseSettings.BatchRecordCount));

            if(transactions.Count>0)
            {
               int foldercount = transactions.Select(x => x.TransactionNumber).Distinct().Count();

                //List<UploadDocuments> uploadDocuments = ProcessTransactions(transactions);
                var uploadDocumentDetails = ProcessTransactions(transactions);
                //Processing the Initial File
                List<UploadDocuments> InitialUploadDocuments = uploadDocumentDetails.Item1;
                _documentCount = InitialUploadDocuments.Count;
                if (InitialUploadDocuments.Count > 0)
                { 
                    //Generating Metadata for the batch
                    List<dynamic> InitialBatchMetaData = InitialUploadDocuments.Select(d => d.MetaData).ToList<dynamic>();
                    List<DocumentDetail> InitialBatchFiles = InitialUploadDocuments.Select(d => d.documentDetail).ToList<DocumentDetail>();
                    _logger.WriteLine(string.Format(" > Creating the metadata file. {0} ", DateTime.Now.ToLongTimeString()));
                    UpdateStatus("Creating the metadata file.");
                    string InitialMetaDataFile = BaseSettings.MetadataLocation + "/SampleInitialMetaData" + DateTime.Now.ToString("ddMMyyyyhhmmssfff") +".csv";
                    bool metaDataFileCreated =GenerateMetaDataFile(InitialBatchMetaData, InitialMetaDataFile);
                    if(metaDataFileCreated)
                    {
                            _logger.WriteLine(string.Format(" > Started Uploading the files @ {0} ", DateTime.Now.ToLongTimeString()));
                            ConnectSftp(InitialBatchFiles, InitialMetaDataFile,"Initial");
                            _logger.WriteLine(string.Format(" > Started Uploading the files @ {0} ", DateTime.Now.ToLongTimeString()));
                     }
                }
                //Processing the Remaining files.
                List<UploadDocuments> remainingUploadDocuments = uploadDocumentDetails.Item2;
                _documentCount = _documentCount + remainingUploadDocuments.Count;
                if (remainingUploadDocuments.Count > 0)
                {
                    //Generating Metadata for the batch
                    List<dynamic> batchMetaData = remainingUploadDocuments.Select(d => d.MetaData).ToList<dynamic>();
                    List<DocumentDetail> batchFiles = remainingUploadDocuments.Select(d => d.documentDetail).ToList<DocumentDetail>();
                    _logger.WriteLine(string.Format(" > Creating the metadata file. {0} ", DateTime.Now.ToLongTimeString()));
                    UpdateStatus("Creating the metadata file.");
                    string metaDataFile = BaseSettings.MetadataLocation + "/SampleMetaData" + DateTime.Now.ToString("ddMMyyyyhhmmssfff") + ".csv";
                    bool metaDataFileCreated = GenerateMetaDataFile(batchMetaData, metaDataFile);
                    if (metaDataFileCreated)
                    {
                        _logger.WriteLine(string.Format(" > Started Uploading the files @ {0} ", DateTime.Now.ToLongTimeString()));
                        ConnectSftp(batchFiles, metaDataFile,"Secondary");
                        _logger.WriteLine(string.Format(" > Started Uploading the files @ {0} ", DateTime.Now.ToLongTimeString()));
                    }
                }
            }
            _logger.WriteLine(string.Format(" > Started updating the database @ {0} ", DateTime.Now.ToLongTimeString()));
            UpdateUploadFlag(transactions);
            _logger.WriteLine(string.Format(" > Completed updating the database @ {0} ", DateTime.Now.ToLongTimeString()));
            EndProcess();
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
                if(transaction.WorkItemType == ItemType.Document)
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
                    if (lstBrandingWithOutAnnotation.Count>0 )
                        lstDocumentsToUpload.AddRange(lstBrandingWithOutAnnotation);
                }
                                
                if(lstOtherTypeDocuments.Count>0)
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
                            DocumentAttributeModel1 fileMetadata= new DocumentAttributeModel1
                            {
                                EntityType = transaction.EntityType,
                                BussinessName = transaction.BusinessName,
                                FileNumber = transaction.FileNumber,
                                DocumentDate = transaction.DocumentDate,
                                DocumentTitle = transaction.DocumentTitle,
                                TransactionNumber = transaction.TransactionNumber,
                                BusinessRegistrationDivision = "DV1",
                                Public = "Y",
                                Suffix = transaction.Suffix
                            };

                           
                            fileMetadata.FileName = GenerateFileName(transaction, docIndex, fileInfo.Extension);
                            
                            if((hasMultipleDocuments && docIndex==1) || (!hasMultipleDocuments && docIndex == 1))
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
        private string GenerateFileName(WorkItemTransaction transaction,int fileIndex, string fileExtension)
        {            
            switch (transaction.TransactionType)
            {
                case "CN":
                case "GPSC":
                case "NC":
                case "PDS":
                case "PNC":
                case "PRS":
                    return string.Format("{0}_{1}_{2}{3}", transaction.PartnershipName.Replace(" ","_"), transaction.FileNumber, fileIndex.ToString(), fileExtension);
                case "ANNBX":
                    return string.Format("{0}_{1}_{2}_{3}{4}", transaction.PartnershipName, transaction.FileNumber,transaction.Year, fileIndex.ToString(), fileExtension);
                case "TNAS":
                case "TNTR":
                    return string.Format("{0}_{1}{2}", transaction.TradeName, fileIndex.ToString(), fileExtension);
                case "TNNA":
                    return string.Format("{0}_{1}_{2}{3}", transaction.PartnershipName,transaction.DocumentDateReceived, fileIndex.ToString(), fileExtension);
                default:
                    return string.Format("{0}_{1}_{2}_{3}{4}", transaction.ObjectID, transaction.DocumentDate != null ? transaction.DocumentDate.Replace(" ", "_").Replace("/", "-").Replace(@"\", "_") : transaction.DocumentDate, transaction.DocumentTitle != null ? transaction.DocumentTitle.Replace(" ", "_").Replace("/", "-").Replace(@"\", "_").Replace(".","_").Replace("\n","").Replace(":","_").Replace("<","_").Replace("*","_").Replace("?","_") : transaction.DocumentTitle, fileIndex.ToString(), fileExtension);
                    //return string.Format("{0}_{1}{2}",transaction.ObjectID, fileIndex.ToString(),fileExtension);                    
                    //return string.Format("{0}_{1}_{2}_{3}{4}", transaction.ObjectID, transaction.DocumentDate != null ? transaction.DocumentDate.Replace(" ", "_").Replace("/", "-").Replace(@"\", "_") : transaction.DocumentDate, transaction.DocumentTitle != null ? transaction.DocumentTitle.Replace(" ", "_").Replace("/", "-").Replace(@"\", "_") : transaction.DocumentTitle, fileIndex.ToString(), fileExtension);
            }            
        }
        private async Task UploadDocs(int i, List<DocumentDetail> documents, string metaDataFile,List<WorkItemTransaction> transactions,string fileType,bool runDatabaseUpdate)
        {
            await Task.Run(() =>
            {
                try
                {
                    ConnectSftp(documents, metaDataFile, fileType);

                    if(runDatabaseUpdate)
                       UpdateUploadFlag(transactions);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
        } 
        private void EndProcess()
        {
            CalculateTotalTime();
            UpdateStatus(String.Format("Processing Completed"));
            //string strMessage = String.Format("Document Upload Completed");
            //string strMessage = String.Format("Uploaded {0} documents. Total time taken to complete the process is {1}, Process Starts @ {2} and Ends @ {3}", _documentCount.ToString(), Math.Round(_timeElapsed, 2).ToString(), _timeStart.ToString(), _timeEnd.ToString());           
            string strMessage = String.Format("Uploaded {0} documents. Total time taken to complete the process is {1}, Process Starts @ {2} and Ends @ {3}", _documentCount.ToString(), _strTotalTime, _timeStart.ToString(), _timeEnd.ToString());
            _logger.WriteLine(string.Format(" > Total documents Uploaded {0}. Total time taken to complete the process is {1}, Process Starts @ {2} and Ends @ {3}", _documentCount.ToString(), _strTotalTime, _timeStart.ToString(), _timeEnd.ToString()));
            _logger.WriteLine(string.Format(" > Process Ended  "));
            _logger.WriteLine(string.Format(" {0}", "--------------------------------------------------------------------------"));
            MessageBox.Show(strMessage);
        }
        private void UpdateStatus(string Message)
        {
            //_processSummary.Summary = Message;
            if (chkbMultiThread.Checked)
            {
                lblStatus.Invoke((MethodInvoker)delegate ()
                {
                    //lblStatus.Text = _processSummary.Summary;
                });
            }
            else
            {
                //lblStatus.Text = _processSummary.Summary;
                lblStatus.Update();
            }

        }
        private void CalculateTotalTime()
        {           
            _timeEnd = DateTime.Now;
            _timeElapsed = _timeEnd.Subtract(_timeStart).TotalHours;
            TimeSpan span = _timeEnd.Subtract(_timeStart);
            _strTotalTime = String.Format("{0}:{1}:{2}", span.Hours, span.Minutes, span.Seconds);
        }
        private void btnCancel_Click(object sender, EventArgs e)
        { 
            if(_logger!=null)
              _logger.CloseLogger();

            Application.Exit();
        }
        private bool ConnectSftp(List<DocumentDetail> files,string metaDataFile, string fileType)
        {
            bool isConnected = false;            
            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = BaseSettings.SFTPHostName,
                UserName = BaseSettings.SFTPUserName,
                SshHostKeyFingerprint = BaseSettings.SshHostKey,
                SshPrivateKeyPath = BaseSettings.SshPrivateKeyPath
            };

            using (Session session = new Session())
            {
                _logger.WriteLine(string.Format(" > Connecting to the SFTP {0} ", DateTime.Now.ToLongTimeString()));
                session.Open(sessionOptions);

                //Uploading documents to sftp
                foreach(DocumentDetail file in files)
                {
                    //_logger.WriteLine(string.Format(" > Uploading the file {1} {0} ", DateTime.Now.ToLongTimeString(), file.DocumentName));
                    UpdateStatus("Uploading the file " + file.DocumentName);
                    using (FileStream fileStream = new FileStream(file.DocumentPath, FileMode.Open))
                    {
                        if (fileType=="Initial")
                        {
                            if (!session.FileExists(BaseSettings.InitialRemoteFolderLocation + file.DocumentName))
                                session.PutFile(fileStream, BaseSettings.InitialRemoteFolderLocation + file.DocumentName);
                        }
                        else
                        { 
                            if (! session.FileExists(BaseSettings.RemoteFolderLocation + file.DocumentName))
                               session.PutFile(fileStream, BaseSettings.RemoteFolderLocation + file.DocumentName);
                        }
                    }
                }

                //Uploading metadata file.
                _logger.WriteLine(string.Format(" > Uploading the metadata file {1} {0} ", DateTime.Now.ToLongTimeString(), metaDataFile));
                UpdateStatus("Uploading the metadata file ");
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
        private bool GenerateMetaDataFile(List<dynamic> documentAttributes , string metaDataFilePath)
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
        private List<WorkItemTransaction> GetWorkItems()
        {
            List<WorkItemTransaction> transactions = new List<WorkItemTransaction>();
            transactions.Add(new WorkItemTransaction
            {
                ObjectID = "00320RBE1102286",
                TransactionType ="T1",
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
        private async Task StartProcessAsync()
        {
            _logger.WriteLine(string.Format(" {0}", "--------------------------------------------------------------------------"));
            _logger.WriteLine(string.Format(" > Process Started @ {0} ", DateTime.Now.ToLongTimeString())); 
            _logger.WriteLine(string.Format(" > Reading list of transcations from the database {0} ", DateTime.Now.ToLongTimeString()));
            UpdateStatus("Reading list of documents from database");
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
                    if(remainingUploadDocuments.Count>0)
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
        private async Task StartProcessSecondaryDocAsync()
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
            catch(Exception ex)
            {
                IsSuccess = false;
            }

            return IsSuccess;
        }
    }
}

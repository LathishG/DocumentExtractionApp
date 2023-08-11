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
using ceTe.DynamicPDF;
using ceTe.DynamicPDF.Imaging;

using BitMiracle.LibTiff.Classic;
namespace DocumentProcess
{
    public partial class DocumentProcess : Form
    {
        private Logger _logger;
        private DateTime _timeStart;
        private DateTime _timeEnd;
        private int _workItemCount;
        private double _timeElapsed;
        private static object obj=new object();
        private ProcessSummary _processSummary = null;
        Control _control;
        private bool buttonPressed4 = false;
        public DocumentProcess()
        {
            InitializeComponent();
            _processSummary = new ProcessSummary();
            //_control = lblCompleted;
        }
        private void btnProcess_Click(object sender, EventArgs e)
        {

            //MessageBox.Show("Hello");
            //ProcessNICDocuments();
            ProcessNICDocumentsBasedOnDocsInFolder();
         //this.IntiProcess();

            //if (chkbMultiThread.Checked)
            //{
            //   await StartProcessAsMultiThreaded();
            //}
            //else
            //{
            //  StartProcess();
            //} 

        }

        private void ProcessNICDocumentsBasedOnDocsInFolder()
        {
            WorkItem objWorkItem=null;
            string[] nicfiles;
            bool HasError = true;
            string strFilName = "";
            string strObjectName = "";
            string sourceLocation = @"F:\DocumentProcess\NIC_Docs\Source\";
            int tiffDocCount = 0;

            tiffDocCount = 0;
            //read the files from the folder.
            nicfiles = Directory.GetFiles(sourceLocation);
            if (nicfiles.Length > 0)
            {
                foreach (string file in nicfiles)
                {
                    try
                    {
                        tiffDocCount = 0;
                        strFilName = Path.GetFileName(file);
                        strObjectName = "P" + strFilName.Replace(".tiff", "");
                        tiffDocCount = tiffDocCount + 1;
                        objWorkItem = DocumentExtraction.BLL.Service.DocumentProcessor.GetworItemsDetails(strObjectName);
                        if (objWorkItem != null)
                        {
                            //Convert to pdf
                            ConvertImageWithPdfusingItext(file, ItemType.Document, objWorkItem, tiffDocCount);
                            // Move the tiff to final location
                            if (objWorkItem.WorkItemType == ItemType.Document)
                            {
                                File.Move(file, @"F:\DocumentProcess\NIC_Docs\Archive\Document\" + Path.GetFileName(file));
                            }
                            else
                            {
                                File.Move(file, @"F:\DocumentProcess\NIC_Docs\Archive\Branding\" + Path.GetFileName(file));
                            }
                            objWorkItem.HasError = "N";
                            objWorkItem.HasPages = "Y";
                            objWorkItem.TiffDocumentCount = tiffDocCount;
                            objWorkItem.DocumentCount = tiffDocCount;
                            objWorkItem.Comments = "Document extracted successfully ";
                            //Update the record in the database.
                            DocumentExtraction.BLL.Service.DocumentProcessor.UpdateDocsFromNIC(objWorkItem);
                        }

                        
                    }
                    catch(Exception ex)
                    {
                        objWorkItem.HasError = "Y";
                        objWorkItem.HasPages = "N";
                        objWorkItem.TiffDocumentCount = 0;
                        objWorkItem.DocumentCount = 0;
                        objWorkItem.Comments = "Error while processing the workitem. Exception: " + ex.Message;
                        DocumentExtraction.BLL.Service.DocumentProcessor.UpdateDocsFromNIC(objWorkItem);
                    }
                    
                }
                
            }            
        }
        private void ProcessNICDocuments()
        {
            List<WorkItem> objlstWorkItem = new List<WorkItem>();
            string[] nicfiles;
            bool HasError = true;
            string sourceLocation = @"F:\DocumentProcess\NIC_Docs\Source";
            int tiffDocCount = 0;  

            objlstWorkItem = DocumentExtraction.BLL.Service.DocumentProcessor.GetErrordworItemsDuringExtraction();
            foreach (WorkItem objWorkItem in objlstWorkItem)
            {
                try
                {
                    tiffDocCount = 0;
                    //read the files from the folder.
                    nicfiles = Directory.GetFiles(sourceLocation, objWorkItem.WorkItemID + "*");
                    if (nicfiles.Length > 0)
                    {
                        foreach (string file in nicfiles)
                        {
                            tiffDocCount = tiffDocCount + 1;
                            if (file != null)
                            {
                                //Convert to pdf
                                ConvertImageWithPdfusingItext(file, ItemType.Document, objWorkItem, tiffDocCount);
                                // Move the tiff to final location
                            }
                        }
                        objWorkItem.HasError = "N";
                        objWorkItem.HasPages = "Y";
                        objWorkItem.TiffDocumentCount = tiffDocCount;
                        objWorkItem.DocumentCount = tiffDocCount;
                        objWorkItem.Comments = "Document extracted successfully ";
                        //Update the record in the database.
                        DocumentExtraction.BLL.Service.DocumentProcessor.UpdateDocsFromNIC(objWorkItem);
                    }                    
                }
                catch (Exception ex)
                {
                    objWorkItem.HasError = "Y";
                    objWorkItem.HasPages = "N";
                    objWorkItem.TiffDocumentCount = 0;
                    objWorkItem.DocumentCount = 0;
                    objWorkItem.Comments = "Error while processing the workitem. Exception: " + ex.Message;
                    DocumentExtraction.BLL.Service.DocumentProcessor.UpdateDocsFromNIC(objWorkItem);
                }

            }
        }
        private void IntiProcess()
        {  
            _logger = Logger.Instance;
            _logger.CreateLogFile("Process_util");
           
            _processSummary.Total = 0;
            _processSummary.Error = 0;
            _processSummary.Burned = 0;
            _processSummary.Completed = 0;
            _processSummary.TimeElasped = 0;
            //lblStatus.Text = _processSummary.Summary;
            //lblDomain.Text = BaseSettings.WorkflowDomain;
            //lblDomain.Update();
            //lblUser.Text = BaseSettings.WorkflowUser;
            //lblUser.Update();
            //lblImageFile.Text = BaseSettings.FinalLocation + @"\Image";
            //lblImageFile.Update();
            //lblAnnotationFile.Text = BaseSettings.ArchiveLocation+ @"\Image";
            //lblAnnotationFile.Update();
            //lblLogFile.Text = BaseSettings.LogfilePath;
            //lblLogFile.Update();
            //lblDocCount.Text = _processSummary.Total.ToString();
            //lblDocCount.Update();
            //lblCompleted.Text = _processSummary.Completed.ToString();
            //lblCompleted.Update();
            //lblError.Text = _processSummary.Error.ToString();
            //lblError.Update();
            //lblBurned.Text = _processSummary.Burned.ToString();
            //lblBurned.Update();
            //lblTime.Text = _processSummary.TimeElasped.ToString();
            //lblTime.Update();            
            UpdateStatus("Idle......");
        }
        private void StartProcess()
        {
            _timeStart = DateTime.Now;
            CALMaster _objCALMaster = null;
            CALClient _objCALClient = null;
            try
            {
                _logger.WriteLine(string.Format(" {0}", "=========================================================================="));
                _logger.WriteLine(string.Format(" > Process Started @ {0} ", DateTime.Now.ToLongTimeString()));                
                List<WorkItem> workItems = DocumentProcessor.GetworItems(Convert.ToInt32(BaseSettings.Batch), Convert.ToInt32(BaseSettings.BatchRecordCount));                
                _workItemCount = workItems.Count;
                //lblDocCount.Text = _workItemCount.ToString();
                //lblDocCount.Update();
                if (workItems.Count > 0)
                {
                    //Open a new login session with a workflow domain using username,Password and domain RETURNS CALClient
                    string strUserName = BaseSettings.WorkflowUser;
                    string strpassword = BaseSettings.WorkflowPassword;
                    string strdomain = BaseSettings.WorkflowDomain;
                    _objCALMaster = new CALMaster();                    
                    _objCALClient = _objCALMaster.CreateClient(strUserName, strpassword, strdomain);
                    if (_objCALClient != null)
                    {
                        UpdateStatus("Logged into opentext...");
                        _logger.WriteLine(string.Format("> Batch {0} Thread {1}  Openes a new login session with a workflow domain", BaseSettings.Batch, "Thread_1"));
                        ProcessWorkItemBatch(workItems, _objCALClient, BaseSettings.Batch, "Thread_1");
                        EndProcess();
                    }
                    else
                    {
                       string strMessage = String.Format("> Batch {0} Thread {1} Error occured when logging in to Opentext.", BaseSettings.Batch, "Thread_1");
                       MessageBox.Show(strMessage);
                    }  
                    
                }
                else
                {                    
                    string strMessage = String.Format("Batch {0} Thread {1} No workitem to process}", BaseSettings.Batch, "Thread_1");
                    MessageBox.Show(strMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLine(" > Error Occured : " + ex.Message);
            }
            finally
            {
                if(_logger!=null)    _logger.CloseLogger();
                if (_objCALClient != null)
                {
                    _objCALClient.Destroy(ClientDestroy.AbortChanges);
                    _objCALClient = null;
                }
                _objCALMaster = null;
            }
        }
        private async Task Process(int i, List<WorkItem> batchWorkItem, CALClient _objCALClient)
        {
            await Task.Run(() =>
            {
                try
                {
                    ProcessWorkItemBatch(batchWorkItem, _objCALClient, BaseSettings.Batch, "Thread_" + i.ToString());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
        }
        private async Task StartProcessAsMultiThreaded()
        {
            _timeStart = DateTime.Now;
            List<Task> _taskList = new List<Task>();
            CALMaster _objCALMaster=null;
            CALClient _objCALClient = null; 
            try
            {
                _logger.WriteLine(string.Format(" {0}", "=========================================================================="));
                _logger.WriteLine(string.Format(" > Process Started @ {0} ", DateTime.Now.ToLongTimeString()));
                //List<WorkItem> _workItems = DocumentProcessor.GetWorkItemSample5();
                List<WorkItem> _workItems = DocumentProcessor.GetworItems(Convert.ToInt32(BaseSettings.Batch), Convert.ToInt32(BaseSettings.BatchRecordCount));
                //List<WorkItem> _workItems = DocumentProcessor.GetworItems(batchID);
               _workItemCount = _workItems.Count;               
                if (_workItems.Count > 0)
                {
                    //Open a new login session with a workflow domain using username,Password and domain RETURNS CALClient
                    string _strUserName = BaseSettings.WorkflowUser;
                    string _strpassword = BaseSettings.WorkflowPassword;
                    string _strdomain = BaseSettings.WorkflowDomain;
                    _objCALMaster = new CALMaster();
                    _objCALClient = _objCALMaster.CreateClient(_strUserName, _strpassword, _strdomain);
                    UpdateStatus("Logged into opentext...");
                    _logger.WriteLine(" > Openes a new login session with a workflow domain");

                    //Read the max thread count from the configuration file.
                    int _maxThreadCount = Convert.ToInt32(BaseSettings.MaxThreadCount);
                    int _recordPerBatch = (int) Math.Ceiling( _workItemCount / (double) _maxThreadCount);
                    List<List<WorkItem>> workItemsArray = _workItems.partition(_recordPerBatch);
                    List<WorkItem> batchWorkItem = null;
                    for (int i=0;i<_maxThreadCount;i++)
                    {                        
                        if (i< workItemsArray.Count)
                        {
                            batchWorkItem = workItemsArray[i];
                            var task = Process(i, batchWorkItem, _objCALClient);
                            _taskList.Add(task);
                        }
                       
                    }
                    await Task.WhenAll(_taskList);
                    UpdateStatus("Started burning the annotations...");
                    EndProcess();
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLine(" > Error Occured : " + ex.Message);
                _logger.CloseLogger();
            }
            finally
            {
                if (_logger != null)  _logger.CloseLogger();
                if (_objCALClient != null)
                {
                    _objCALClient.Destroy(ClientDestroy.AbortChanges);
                    _objCALClient = null;
                }           
                _objCALMaster = null;
            }
        }        
        private void ProcessWorkItemBatch(List<WorkItem> workItems,CALClient cALClient,string batchId,string threadNo)
        {   

            foreach (WorkItem _objWorkItem in workItems)
            {
                _logger.WriteLine(string.Format(" {0}", "--------------------------------------------------------------------------"));
                _logger.WriteLine(string.Format("> Batch {0} Thread {1} Started Processing the workItem - {2}", batchId, threadNo, _objWorkItem.WorkItemID));

                bool hasError = false;
                try
                {
                    UpdateStatus(String.Format("Processing the workitem {0}", _objWorkItem.WorkItemID));
                    ProcessWorkItem(_objWorkItem, cALClient, batchId, threadNo);
                    
                }
                catch (Exception ex)
                {
                    hasError = true;
                    _logger.WriteLine(string.Format("> Batch {0} Thread {1} Error Occured while processing the workitem {2}" , batchId, threadNo, _objWorkItem.WorkItemID + " \n" + ex.Message));                    
                }
                //Update workItem Details
                lock (obj)
                {
                    _processSummary.Completed++;
                    if (hasError)
                    {
                        _processSummary.Error++;                        
                    }    
                   
                }               
                _objWorkItem.BatchId = Convert.ToInt32(BaseSettings.Batch);
                _objWorkItem.IsProcessed = "Y";
                _objWorkItem.HasError = hasError == true ? "Y" : "N";
                _objWorkItem.Comments = hasError == true ? "Error while processing the workitem" : "Document extracted successfully ";
                _objWorkItem.Status = _objWorkItem.HasAnnotation=="Y" ? "Extracted":"Completed";
                DocumentProcessor.UpdateWorkItem(_objWorkItem);
                
                if (hasError) continue;                
            }
            _logger.WriteLine(string.Format("> Batch {0} Thread {1} Process Ended @ {2} ", batchId, threadNo, DateTime.Now.ToLongTimeString()));  
        }
        private void ProcessWorkItem(WorkItem workItem, CALClient objCALClient, string batchId, string threadNo)
        {
            CALClientList _cALClientList =null;
            CALClientListItem _cALClientListItem = null;
            try
            {
                //Look for workitem in the domain that matches a particular name           
                int maxMatches = 1;
                int WorkItemDocumentCount = 0;
                workItem.HasError = "N";
                workItem.HasPages = "N";
                workItem.HasAnnotation = "N";
                workItem.DocumentCount = 0;
                workItem.TiffDocumentCount = 0;
                workItem.NonTiffDocumentCount = 0;
                CALIndexFields _cALIndexFields = new CALIndexFields();//Optional parameter               
                //Looks for workitems in the domain that match a particular name or wildcard, and returns a reference to a list of matching workitems
                CALQueryResults _cALQueryResults = objCALClient.Query(workItem.WorkItemID, ObjType.Document, QueryLocation.Domain, maxMatches, _cALIndexFields);
                if(_cALQueryResults!=null)
                { 
                    //Get the single workitem
                    _logger.WriteLine(String.Format("> Batch {0} Thread {1} Get the single workitem", batchId, threadNo));
                    //Represents a specific workitem in the query results list. 
                    CALEnumItem _cALEnumItem = _cALQueryResults.Item(1);
                    //Check whether the workitem is already in the Clientlist
                    _cALClientList = objCALClient.ClientList;
                    _cALClientListItem = _cALClientList.Find(_cALEnumItem.Info.ObjID);
                   
                    if (_cALClientListItem == null)
                    {
                        //Retrieves the workitem and adds to the client list
                        _logger.WriteLine(String.Format("> Batch {0} Thread {1} Retrieves this workitem and adds to the client list", batchId, threadNo));                        
                        _cALClientListItem = _cALEnumItem.Retrieve(EnumRetrieve.Retain);
                    }
                    
                    //Check if the item is in Archive or not.
                    CALWorkitem _cALWorkitem;
                    if (_cALEnumItem.Info.ServerName == "IMGSVR" || _cALEnumItem.Info.ServerName == "TSTSVR")
                        _cALWorkitem = _cALClientListItem.Open(WorkitemOpen.ReadOnly);
                    else
                        _cALWorkitem = _cALClientListItem.Open(WorkitemOpen.Archive);

                    CALDocument _cALDocument = (CALDocument)_cALWorkitem;
                    if (_cALDocument !=null)
                    { 
                        _logger.WriteLine(String.Format("> Batch {0} Thread {1} Retrieve pages and Annotations", batchId, threadNo));                        

                        //Count of non tiff documents.
                        CALDocumentImports _cALDocumentImports = _cALDocument.Imports;
                        int _documentImports = _cALDocumentImports.Count;
                        WorkItemDocumentCount = _documentImports;
                        workItem.NonTiffDocumentCount = WorkItemDocumentCount;

                        //Count of tiff images.
                        CALPages _cALPages = _cALDocument.Pages;
                        if (_cALPages!=null)
                        {                             
                            int _documentCount = _cALPages.Count;
                            workItem.TiffDocumentCount = _documentCount;
                            WorkItemDocumentCount += _documentCount;
                            workItem.DocumentCount = WorkItemDocumentCount;  
                        }
                        else
                        {
                            _logger.WriteLine("> The document doesn't have page.");                            
                        } 
                        _cALDocument.Close(WorkitemClose.None);
                        _cALClientList.Remove(_cALClientListItem);
                        //_cALClientList.Clear(ClientListClear.None);
                    }
                    else
                    {
                        _logger.WriteLine("> Error in Opening the workItem.");                        
                    }                    
                }
                else
                {
                    _logger.WriteLine("> Workitem Not Found");                    
                }
            }
            catch (Exception)
            {
                _cALClientList.Remove(_cALClientListItem);
                workItem.HasError = "Y";
                workItem.HasPages = "N";
                workItem.HasAnnotation = "N";
                workItem.DocumentCount = 0;                
                throw;
            }
        }
        private void EndProcess()
        {
            CalculateTotalTime(); 
            UpdateStatus(String.Format("Processing Completed"));            
            string strMessage = String.Format("Processed {0} documents. Total time taken to complete the process is {1}, Process Starts @ {2} and Ends @ {3}",_workItemCount.ToString(), Math.Round(_timeElapsed, 2).ToString(), _timeStart.ToString(), _timeEnd.ToString());
            MessageBox.Show(strMessage);
        }
        private void UpdateStatus(string Message)
        {
            _processSummary.Summary = Message;
        }
        private void CalculateTotalTime()
        {           
            _timeEnd = DateTime.Now;
            _timeElapsed = _timeEnd.Subtract(_timeStart).TotalMinutes; 
        }        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if(_logger!=null)
              _logger.CloseLogger();

            Application.Exit();
        }
        private void btnDocCount_Click(object sender, EventArgs e)
        {
            List<WorkItem> _workItems = DocumentProcessor.GetListOfProcessedWorkItemWithOutCount();
            string[] _annotationsforburning;
            string[] _pdfWithAnnotation;
            int docCount = 0;
            foreach (WorkItem workitem in _workItems)
            {
                if (workitem.WorkItemType == ItemType.Document)
                    _annotationsforburning = Directory.GetFiles(BaseSettings.StagingLocation + @"\Document\Annotation", workitem.ID + "*");
                else
                    _annotationsforburning = Directory.GetFiles(BaseSettings.StagingLocation + @"\Branding\Annotation", workitem.ID + "*");

                docCount = _annotationsforburning.Length;

                if (docCount==0)
                {
                   
                    if (workitem.WorkItemType == ItemType.Document)
                        _pdfWithAnnotation = Directory.GetFiles(BaseSettings.FinalLocation + @"\Document\Pdf_With_Annotation", workitem.ID + "*");
                    else
                        _pdfWithAnnotation = Directory.GetFiles(BaseSettings.FinalLocation + @"\Branding\Pdf_With_Annotation", workitem.ID + "*");

                    docCount = _pdfWithAnnotation.Length;
                }                

                if (docCount > 0)
                {
                    if (docCount > 1)
                       MessageBox.Show("WorkItem :" + workitem.ID.ToString());


                    workitem.TiffDocumentWithAnnotationCount = docCount;

                    DocumentProcessor.UpdateDocCountOfWorkItemWithAnnotation(workitem);
                }
                 
            }

            MessageBox.Show("Updation Completed");
        }
        private void btnmove_Click(object sender, EventArgs e)
        {

            string[] annotationfiles = Directory.GetFiles(@"F:\DocumentProcess\Staging\Document\Annotation", "*.Ano");

            foreach (string strfile in annotationfiles)
            {
                if (!File.Exists(@"F:\DocumentProcess\Staging_Extraction\Document\Annotation\" + Path.GetFileName(strfile)))
                {
                    File.Move(strfile, @"F:\DocumentProcess\Staging_Extraction\Document\Annotation\" + Path.GetFileName(strfile));
                }
            }
            //string[] _annotationsforburning;
            //string[] _imagessforburning;
            //foreach (WorkItem _workItem in _workItems)
            //{
            //    //Moving Annotation Files.
            //    if (_workItem.WorkItemType == ItemType.Document)
            //        _annotationsforburning = Directory.GetFiles(@"F:\DocumentProcess\Staging\Document\Annotation", _workItem.ID + " * ");
            //    else
            //        _annotationsforburning = Directory.GetFiles(@"F:\DocumentProcess\Staging\Branding\Annotation", _workItem.ID + "*");
               
            //    if(_annotationsforburning.Length >0)
            //    { 
            //    foreach (string _annofile in _annotationsforburning)
            //    {
            //        if (_workItem.WorkItemType == ItemType.Document)
            //        {
            //            if (!File.Exists(@"F:\DocumentProcess\Staging_Extraction\Document\Annotation\" + Path.GetFileName(_annofile)))
            //            {
            //                File.Move(_annofile, @"F:\DocumentProcess\Staging_Extraction\Document\Annotation\" + Path.GetFileName(_annofile));
            //            }
            //        }
            //        else
            //        {
            //            if (!File.Exists(@"F:\DocumentProcess\Staging_Extraction\Branding\Annotation\" + Path.GetFileName(_annofile)))
            //            {
            //                File.Move(_annofile, @"F:\DocumentProcess\Staging_Extraction\Branding\Annotation\" + Path.GetFileName(_annofile));
            //            }
            //        }                                      
            //    }

            //    //Moving image files.
            //    if (_workItem.WorkItemType == ItemType.Document)
            //        _imagessforburning = Directory.GetFiles(@"F:\DocumentProcess\Staging\Document\Image", _workItem.ID + " * ");
            //    else
            //        _imagessforburning = Directory.GetFiles(@"F:\DocumentProcess\Staging\Branding\Image", _workItem.ID + "*");

            //    foreach (string _imgfile in _imagessforburning)
            //    {
            //        if (_workItem.WorkItemType == ItemType.Document)
            //        {
            //            if (!File.Exists(@"F:\DocumentProcess\Staging_Extraction\Document\Image\" + Path.GetFileName(_imgfile)))
            //            {
            //                File.Move(_imgfile, @"F:\DocumentProcess\Staging_Extraction\Document\Image\" + Path.GetFileName(_imgfile));
            //            }
            //        }
            //        else
            //        {
            //            if (!File.Exists(@"F:\DocumentProcess\Staging_Extraction\Branding\Image\" + Path.GetFileName(_imgfile)))
            //            {
            //                File.Move(_imgfile, @"F:\DocumentProcess\Staging_Extraction\Branding\Image\" + Path.GetFileName(_imgfile));
            //            }
            //        }
            //    }

            //    }
            //}
                
        }
        private void button1_Click(object sender, EventArgs e)
        {
            List<WorkItem> _workItems = DocumentProcessor.GetListOfHoldDocs();
            string[] _annotationsforburning;
            string[] _tifImages;
            string[] _pdfdocuments;
            int docCount = 0;
            bool annotationAvailableinStage1 = true;
            bool imageAvailableinstage1 = true;
            List<string> _objectID = new List<string>();
            int index = 0;
            foreach (WorkItem workitem in _workItems)
            {
                //Get the count of annotations
                if (workitem.WorkItemType == ItemType.Document)
                    _annotationsforburning = Directory.GetFiles(BaseSettings.StagingLocation + @"\Document\Annotation", workitem.ID + "*");
                else
                    _annotationsforburning = Directory.GetFiles(BaseSettings.StagingLocation + @"\Branding\Annotation", workitem.ID + "*");                             

                //Get the count of tif images
                if (workitem.WorkItemType == ItemType.Document)
                    _tifImages = Directory.GetFiles(BaseSettings.StagingLocation + @"\Document\Image", workitem.ID + "*");
                else
                    _tifImages = Directory.GetFiles(BaseSettings.StagingLocation + @"\Branding\Image", workitem.ID + "*");

               
                if (_annotationsforburning.Length>0 )
                {
                    if (_annotationsforburning.Length == _tifImages.Length)
                    {
                        docCount = docCount + 1;
                    }                    
                    _objectID.Add(workitem.ID);
                   
                }            

            }

            MessageBox.Show("Completed");
        }
        private void button2_Click(object sender, EventArgs e)
        {
            List<WorkItem> _workItems = DocumentProcessor.GetListOfCompletedAnnotationDocs();
            string[] _pdfdocuments;
            string[] _tifImages;
            int countMissmarchingdoccount = 0;
            foreach (WorkItem workitem in _workItems)
            {
                //Image files
                if (workitem.WorkItemType == ItemType.Document)
                    _tifImages = Directory.GetFiles(BaseSettings.FinalLocation + @"\Document\Image_with_Annotation", workitem.ID + "*");
                else
                    _tifImages = Directory.GetFiles(BaseSettings.FinalLocation + @"\Branding\Image_with_Annotation", workitem.ID + "*");

                //Pdf documents
                if (workitem.WorkItemType == ItemType.Document)
                    _pdfdocuments = Directory.GetFiles(BaseSettings.FinalLocation + @"\Document\Pdf_With_Annotation", workitem.ID + "*");
                else
                    _pdfdocuments = Directory.GetFiles(BaseSettings.FinalLocation + @"\Branding\Pdf_With_Annotation", workitem.ID + "*");

                if(_tifImages.Length != _pdfdocuments.Length)
                {
                    countMissmarchingdoccount = countMissmarchingdoccount + 1;
                }
            }
         }
        private void button3_Click(object sender, EventArgs e)
        {
            List<WorkItem> _workItems = DocumentProcessor.GetListOfErrorItems();
            string[] _tifImages;
            string[] _pdfImages;
            string[] _tifImageswithannotation;
            string[] _otherDocs;

            string[] _modifiedFiles;
            string _PdfFileName = "";
            string strTempLocation = "";
            foreach (WorkItem workitem in _workItems)
            {
                if (workitem.WorkItemType == ItemType.Document)
                    _tifImages = Directory.GetFiles(BaseSettings.FinalLocation + @"\Document\Image_without_Annotation", workitem.ID + "*");
                else
                    _tifImages = Directory.GetFiles(BaseSettings.FinalLocation + @"\Branding\Image_without_Annotation", workitem.ID + "*");
               
                _otherDocs = Directory.GetFiles(BaseSettings.FinalLocation + @"\Document\Other_Type_Document", workitem.ID + "*");                

                if (workitem.WorkItemType == ItemType.Document)
                    _tifImageswithannotation = Directory.GetFiles(BaseSettings.StagingLocation + @"\Document\Annotation", workitem.ID + "*");
                else
                    _tifImageswithannotation = Directory.GetFiles(BaseSettings.StagingLocation + @"\Branding\Annotation", workitem.ID + "*");

                if (workitem.WorkItemType == ItemType.Document)
                    _pdfImages = Directory.GetFiles(BaseSettings.FinalLocation + @"\Document\pdf_without_Annotation", workitem.ID + "*");
                else
                    _pdfImages = Directory.GetFiles(BaseSettings.FinalLocation + @"\Branding\pdf_without_Annotation", workitem.ID + "*");

                if (_tifImageswithannotation.Length > 0)
                    strTempLocation = "dfgdfg";

                //foreach (string str in _tifImages)
                //{
                //    _PdfFileName = Path.GetFileName(str);

                //    if (File.Exists(str))
                //    {
                //        File.Copy(str, @"D:\Test_Tif\" + _PdfFileName);
                //    }
                //}

                //_modifiedFiles = Directory.GetFiles(@"D:\Test_Tif", workitem.ID + "*");
                //foreach (string mfile in _modifiedFiles)
                //{
                //    ConvertImageWithPdfusingItext(mfile, workitem.WorkItemType);
                //}

                workitem.NonTiffDocumentCount = Convert.ToInt32(_otherDocs.Length);
                workitem.TiffDocumentWithAnnotationCount = Convert.ToInt32(_tifImageswithannotation.Length);
                workitem.TiffDocumentCount = Convert.ToInt32(_tifImageswithannotation.Length) + Convert.ToInt32(_tifImages.Length);
                workitem.DocumentCount = Convert.ToInt32(_tifImageswithannotation.Length) + Convert.ToInt32(_tifImages.Length) + Convert.ToInt32(_otherDocs.Length);
                workitem.HasAnnotation = Convert.ToInt32(_tifImageswithannotation.Length) > 0 ? "Y" : "N";
                workitem.IsProcessed = "Y";
                workitem.HasError = "N";
                workitem.Comments = "Document extracted successfully ";
                workitem.Status = _tifImageswithannotation.Length > 0 ? "Extracted" : "Completed";
                if (workitem.DocumentCount > 0)
                    DocumentProcessor.UpdateWorkItem(workitem);
            }

            

        }
        //private void ConvertImageWithPdfusingItext(string srcImageFile, ItemType workItemType)
        //{
        //    string _PdfFileName = Path.GetFileName(srcImageFile).Replace(".tif", ".pdf");
        //    string _pdfFinalLocation = "";
        //    if (workItemType == ItemType.Document)
        //        _pdfFinalLocation = BaseSettings.FinalLocation + @"\Document\Pdf_With_Annotation\" + _PdfFileName;
        //    else
        //        _pdfFinalLocation = BaseSettings.FinalLocation + @"\Branding\Pdf_With_Annotation\" + _PdfFileName;
        //    _pdfFinalLocation = @"D:\Test_Tif\" + _PdfFileName;
        //    ImageData imageData = null;
        //    using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(_pdfFinalLocation)))
        //    {
        //        Document document = new Document(pdfDocument);
        //        Bitmap _bitMap = new Bitmap(srcImageFile);
        //        int _totalpage = _bitMap.GetFrameCount(System.Drawing.Imaging.FrameDimension.Page);
        //        _bitMap.Dispose();
        //        _bitMap = null;
        //        for (int i = 0; i < _totalpage; i++)
        //        {
        //            using (Bitmap _bitMapTemp = new Bitmap(srcImageFile))
        //            {
        //                _bitMapTemp.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Page, i);
        //                using (var MemoryStream = new MemoryStream())
        //                {
        //                    _bitMapTemp.Save(MemoryStream, System.Drawing.Imaging.ImageFormat.Tiff);
        //                    imageData = ImageDataFactory.Create(MemoryStream.ToArray());
        //                    iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData);
        //                    document.Add(image);
        //                }

        //                _bitMapTemp.Dispose();
        //            }
        //        }
        //        if (!pdfDocument.IsCloseWriter())
        //            pdfDocument.Close();
        //    }
        //}
        private async void button4_Click(object sender, EventArgs e)
        {
            for (int itr=1; itr<=5;itr++)
            {            
                if(!buttonPressed4)
                {
                   await DoProcess();
                }
                if (itr == 5)
                    buttonPressed4 = true;
            }
        }
        private async Task DoProcess()
        {
            await Task.Delay(120000);
            MessageBox.Show("Show");
        }
        private void ConvertImageWithPdfusingItext(string srcImageFile, ItemType workItemType,WorkItem objWorkItem,int docCount)
        {
            // Path.GetFileName(srcImageFile).Replace(".TIFF", ".pdf");
            string _PdfFileName = string.Format(objWorkItem.ID + "_{0}.pdf", docCount.ToString());
            string _pdfFinalLocation = "";
            if (workItemType == ItemType.Document)
                _pdfFinalLocation = BaseSettings.FinalLocation + @"\Document\" + _PdfFileName;
            else
                _pdfFinalLocation = BaseSettings.FinalLocation + @"\Branding\" + _PdfFileName;

            iText.IO.Image.ImageData imageData = null;
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(_pdfFinalLocation)))
            {
                iText.Layout.Document document = new iText.Layout.Document(pdfDocument);
                Bitmap _bitMap = new Bitmap(srcImageFile);
                int _totalpage = _bitMap.GetFrameCount(System.Drawing.Imaging.FrameDimension.Page);
                _bitMap.Dispose();
                _bitMap = null;
                for (int i = 0; i < _totalpage; i++)
                {
                    using (Bitmap _bitMapTemp = new Bitmap(srcImageFile))
                    { 
                        _bitMapTemp.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Page, i);
                        using (var MemoryStream = new MemoryStream())
                        {
                            _bitMapTemp.Save(MemoryStream, System.Drawing.Imaging.ImageFormat.Tiff);
                            imageData = ImageDataFactory.Create(MemoryStream.ToArray());
                            iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData);
                            document.Add(image);
                        }
                        _bitMapTemp.Dispose();
                    }
                }
                if (!pdfDocument.IsCloseWriter())
                    pdfDocument.Close();
            }
        }
        private void imgEdit_Close(object sender, EventArgs e)
        {

        }
        private void btnTiffEdit_Click(object sender, EventArgs e)
        {
            //ceTe.DynamicPDF.Conversion.ImageConverter imageConverter = new ceTe.DynamicPDF.Conversion.ImageConverter(@"D:\Test\Test.tif");
            //imageConverter.Convert(@"D:\Test\Test.pdf");

            string strSourceFilePath = @"D:\Test\Sample.tif";
            string strFinalPath = @"D:\Test\Sample.pdf";
            iText.IO.Image.ImageData imageData = null;
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(strFinalPath)))
            {
                iText.Layout.Document document = new iText.Layout.Document(pdfDocument);
                using (Tiff tiff = Tiff.Open(strSourceFilePath, "r"))
                {
                   
                    int pageCount = tiff.NumberOfDirectories();

                    for (int pageIndex = 0; pageIndex < pageCount; pageIndex++)
                    {
                        tiff.SetDirectory((short)pageIndex);
                        int width = tiff.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                        int height = tiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                        using (Bitmap bitmap = new Bitmap(width, height))
                        {
                            int[] raster = new int[width * height];
                            tiff.ReadRGBAImage(width, height, raster);
                            byte[] bytes = raster.SelectMany(BitConverter.GetBytes).ToArray();
                            using (var MemoryStream = new MemoryStream(bytes))
                            {
                                bitmap.Save(MemoryStream, System.Drawing.Imaging.ImageFormat.Tiff);
                                imageData = ImageDataFactory.Create(MemoryStream.ToArray());
                                iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData);
                                document.Add(image);
                            }
                        }                     

                    }
                }
            }
            
        }

        private void btn_NicDocs_Click(object sender, EventArgs e)
        {
            ProcessNICDocuments();
        }

        private void btn_pdf_Click(object sender, EventArgs e)
        {

        }        
    }
}

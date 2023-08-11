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
using System.Drawing.Imaging;

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
        
        public DocumentProcess()
        {
            InitializeComponent();
            _processSummary = new ProcessSummary();
            _control = lblCompleted;
           
        }
        private async void btnProcess_Click(object sender, EventArgs e)
        { 
           this.IntiProcess();
                     
           if (chkbMultiThread.Checked)
           {
              //await StartProcessAsMultiThreaded();
              await StartProcessAsMultiThreadedWithDbBatchLogic();
           }
           else
           {
             StartProcess();
           }
        }
        private void IntiProcess()
        {  
            _logger = Logger.Instance;
            _logger.CreateLogFile("Doc_Extraction_Code_debug");
           
            _processSummary.Total = 0;
            _processSummary.Error = 0;
            _processSummary.Burned = 0;
            _processSummary.Completed = 0;
            _processSummary.TimeElasped = 0;
            _processSummary.OtherTypeDoc = 0;
            lblStatus.Text = _processSummary.Summary;
            lblDomain.Text = BaseSettings.WorkflowDomain;
            lblDomain.Update();
            lblUser.Text = BaseSettings.WorkflowUser;
            lblUser.Update();
            lblImageFile.Text = BaseSettings.FinalLocation + @"\Image";
            lblImageFile.Update();
            lblAnnotationFile.Text = BaseSettings.StagingLocation+ @"\Annotation";
            lblAnnotationFile.Update();
            lblLogFile.Text = BaseSettings.LogfilePath;
            lblLogFile.Update();
            lblArchiveFiles.Text = BaseSettings.ArchiveLocation;
            lblArchiveFiles.Update();
            lblVmName.Text = BaseSettings.InstanceName;
            lblVmName.Update();
            lblDocCount.Text = _processSummary.Total.ToString();
            lblDocCount.Update();
            lblCompleted.Text = _processSummary.Completed.ToString();
            lblCompleted.Update();
            lblError.Text = _processSummary.Error.ToString();
            lblError.Update();
            lblBurned.Text = _processSummary.Burned.ToString();
            lblBurned.Update();
            lblTime.Text = _processSummary.TimeElasped.ToString();
            lblTime.Update();
            lblOther.Text = _processSummary.OtherTypeDoc.ToString();
            lblOther.Update();
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
                if (!BaseSettings.EnableDetailedLog)
                    _logger.WriteLine(string.Format("Reading batches to process " + DateTime.Now.ToLocalTime(), BaseSettings.Batch, "Thread_1"));
                else
                    _logger.WriteLine(" Reading workitem from DB Started " + DateTime.Now);

                List<WorkItem> workItems = DocumentProcessor.GetworItems(Convert.ToInt32(BaseSettings.Batch), Convert.ToInt32(BaseSettings.BatchRecordCount));
                if (BaseSettings.EnableDetailedLog)
                    _logger.WriteLine(" Reading workitem from DB Completed " + DateTime.Now);

                _workItemCount = workItems.Count;
                lblDocCount.Text = _workItemCount.ToString();
                lblDocCount.Update();
                if (workItems.Count > 0)
                {
                    //Open a new login session with a workflow domain using username,Password and domain RETURNS CALClient
                    string strUserName = BaseSettings.WorkflowUser;
                    string strpassword = BaseSettings.WorkflowPassword;
                    string strdomain = BaseSettings.WorkflowDomain;
                    _objCALMaster = new CALMaster();
                    if (BaseSettings.EnableDetailedLog)
                        _logger.WriteLine(" Login to OpenText " + DateTime.Now);

                    _objCALClient = _objCALMaster.CreateClient(strUserName, strpassword, strdomain);

                    if (BaseSettings.EnableDetailedLog)
                        _logger.WriteLine(" Login  Successful " + DateTime.Now);
                    
                    if (_objCALClient != null)
                    {
                        UpdateStatus("Logged into opentext...");
                        //_logger.WriteLine(string.Format("> Batch {0} Thread {1}  Openes a new login session with a workflow domain", BaseSettings.Batch, "Thread_1"));
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
                List<WorkItem> _workItems = DocumentProcessor.GetworItems(BaseSettings.InstanceName, Convert.ToInt32(BaseSettings.Batch), Convert.ToInt32(BaseSettings.BatchRecordCount));
                //List<WorkItem> _workItems = DocumentProcessor.GetworItems(batchID);
               _workItemCount = _workItems.Count;
                lblDocCount.Text = _workItemCount.ToString();
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
        private async Task StartProcessAsMultiThreadedWithDbBatchLogic()
        {
            _timeStart = DateTime.Now;
            List<Task> _taskList = new List<Task>();
            CALMaster _objCALMaster = null;
            CALClient _objCALClient = null;
            try
            {
                if(BaseSettings.EnableDetailedLog)
                   _logger.WriteLine(" Selecting Batches to Process Start " + DateTime.Now);

                List<int> batchesToProcess = DocumentProcessor.GetBatchToProcess(BaseSettings.InstanceName);

                if (BaseSettings.EnableDetailedLog)
                    _logger.WriteLine(" Selecting Batches to Process Completed" + DateTime.Now);

                if (batchesToProcess.Count > 0)
                {
                    //Open a new login session with a workflow domain using username,Password and domain RETURNS CALClient
                    string _strUserName = BaseSettings.WorkflowUser;
                    string _strpassword = BaseSettings.WorkflowPassword;
                    string _strdomain = BaseSettings.WorkflowDomain;
                    _objCALMaster = new CALMaster();

                    if (BaseSettings.EnableDetailedLog)
                        _logger.WriteLine(" Log in to OpenText " + DateTime.Now);

                     _objCALClient = _objCALMaster.CreateClient(_strUserName, _strpassword, _strdomain);

                    if (BaseSettings.EnableDetailedLog)
                        _logger.WriteLine(" Log in to OpenText Succesfully" + DateTime.Now);

                     UpdateStatus("Logged into opentext...");
                    _logger.WriteLine(" > Openes a new login session with a workflow domain");
                    _workItemCount = 0;
                    //Read the max thread count from the configuration file.
                    int _maxThreadCount = Convert.ToInt32(BaseSettings.MaxThreadCount);
                    int _ThreadCount = 1;
                    List<WorkItem> _workItems = null;
                    foreach (int batchid in batchesToProcess)
                    {
                        if (_ThreadCount <= _maxThreadCount)
                        {
                            if (BaseSettings.EnableDetailedLog)
                                _logger.WriteLine(" Selecting workitems per batch " + batchid.ToString() + " - " + DateTime.Now);

                            _workItems = DocumentProcessor.GetworItems(BaseSettings.InstanceName, batchid, Convert.ToInt32(BaseSettings.BatchRecordCount));

                            if (BaseSettings.EnableDetailedLog)
                                _logger.WriteLine(" Selecting workitems per batch completed " + batchid.ToString() + " - " + DateTime.Now);

                            _workItemCount += _workItems.Count;
                            var task = Process(_ThreadCount, _workItems, _objCALClient);
                            _taskList.Add(task);
                            _ThreadCount++;
                        }                        
                    }

                    lblDocCount.Text = _workItemCount.ToString();
                    lblDocCount.Update();

                    await Task.WhenAll(_taskList);
                    UpdateStatus("Started burning the annotations...");
                    EndProcess();
                }
                else
                {
                    MessageBox.Show("There is no batches to Process");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLine(" > Error Occured : " + ex.Message);
                _logger.CloseLogger();
            }
            finally
            {
                if (_logger != null) _logger.CloseLogger();
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

                if (!BaseSettings.EnableDetailedLog)
                    _logger.WriteLine(string.Format("> Batch {0} Thread {1} Started Processing the workItem - {2}", batchId, threadNo, _objWorkItem.WorkItemID));

                bool hasError = false;
                string errorMsg = "";
                try
                {
                    UpdateStatus(String.Format("Processing Start - workitem {0}", _objWorkItem.WorkItemID));
                    _logger.WriteLine(string.Format("Started Processing the workItem ID {0} --{1}", _objWorkItem.WorkItemID, DateTime.Now));
                    ProcessWorkItem(_objWorkItem, cALClient, batchId, threadNo); 
                }
                catch (Exception ex)
                {
                    hasError = true;
                    errorMsg = ex.Message;
                    _logger.WriteLine(string.Format("> Batch {0} Thread {1} Error Occured while processing the workitem {2}" , batchId, threadNo, _objWorkItem.WorkItemID + " \n" + ex.Message));                    
                }
                //Update workItem Details
                lock (obj)
                {
                    _processSummary.Completed++;
                    if (hasError)
                    {
                        _processSummary.Error++;
                        lblError.Invoke((MethodInvoker)delegate () {
                            lblError.Text = _processSummary.Error.ToString();
                        });
                    }    
                    ProcessSummary("");
                }               
                _objWorkItem.BatchId = Convert.ToInt32(BaseSettings.Batch);
                _objWorkItem.IsProcessed = "Y";
                _objWorkItem.HasError = hasError == true ? "Y" : "N";
                _objWorkItem.Comments = hasError == true ? "Error while processing the workitem. Exception: "+ errorMsg : "Document extracted successfully ";
                _objWorkItem.Status = _objWorkItem.HasAnnotation=="Y" ? "Extracted":"Completed";
                
                if (BaseSettings.EnableDetailedLog)
                    _logger.WriteLine(" Updating WorkItem Details Started- ID -" + _objWorkItem.WorkItemID + " - " + DateTime.Now);
                 
                DocumentProcessor.UpdateWorkItem(_objWorkItem);

                if (BaseSettings.EnableDetailedLog)
                    _logger.WriteLine(" Updating WorkItem Details comepleted - ID -" + _objWorkItem.WorkItemID + " - " + DateTime.Now);

                _logger.WriteLine(string.Format("Completed the Processing WorkItem ID {0} --{1}", _objWorkItem.WorkItemID, DateTime.Now));
                _logger.WriteLine(string.Format(" {0}", "--------------------------------------------------------------------------"));
                if (!chkbMultiThread.Checked)
                      ProcessSummary("");

                if (hasError) continue;
                
            }
            _logger.WriteLine(string.Format("> Batch {0} Thread {1} Process Ended @ {2} ", batchId, threadNo, DateTime.Now.ToLongTimeString()));  
        }
        private void ProcessWorkItem(WorkItem workItem, CALClient objCALClient, string batchId, string threadNo)
        {
            try
            {
                //Look for workitem in the domain that matches a particular name           
                int maxMatches = 1;
                int WorkItemDocumentCount = 0;
                int TiffDocWithAnnotation = 0;
                workItem.HasError = "N";
                workItem.HasPages = "N";
                workItem.HasAnnotation = "N";
                workItem.DocumentCount = 0;
                workItem.TiffDocumentCount = 0;
                workItem.TiffDocumentWithAnnotationCount = 0;
                workItem.NonTiffDocumentCount = 0;
                CALIndexFields _cALIndexFields = new CALIndexFields();//Optional parameter
                if (BaseSettings.EnableDetailedLog)
                    _logger.WriteLine(" Querying the workitem id in the OpenText - ID-"+ workItem.WorkItemID + "-" + DateTime.Now);

                CALQueryResults _cALQueryResults = objCALClient.Query(workItem.WorkItemID, ObjType.Document, QueryLocation.Domain, maxMatches, _cALIndexFields);

                if (BaseSettings.EnableDetailedLog)
                    _logger.WriteLine(" Querying Completed" + DateTime.Now);

                if (_cALQueryResults!=null)
                {
                    //Get the single workitem
                    if (!BaseSettings.EnableDetailedLog)
                       _logger.WriteLine(String.Format("> Batch {0} Thread {1} Get the single workitem", batchId, threadNo));

                    if (BaseSettings.EnableDetailedLog)
                        _logger.WriteLine(" Read the workItemID from query result- ID-" + workItem.WorkItemID + "-" + DateTime.Now);

                    CALEnumItem _cALEnumItem = _cALQueryResults.Item(1);
                    
                    //Check whether the workitem is already in the Clientlist
                    CALClientList _cALClientList = objCALClient.ClientList;

                    /*
                     * Removing the below line of codes to improve the performance.*/
                    if (BaseSettings.EnableDetailedLog)
                        _logger.WriteLine(" Checking whether the workitem id is in the client list - ID-" + workItem.WorkItemID + "-" + DateTime.Now);

                    CALClientListItem _cALClientListItem = _cALClientList.Find(_cALEnumItem.Info.ObjID);

                    if (BaseSettings.EnableDetailedLog)
                        _logger.WriteLine(" Checking Completed - ID-" + workItem.WorkItemID + "-" + DateTime.Now);

                    if (_cALClientListItem == null)
                    {
                        //Retrieves the workitem and adds to the client list
                        if (!BaseSettings.EnableDetailedLog)
                            _logger.WriteLine(String.Format("> Batch {0} Thread {1} Retrieves this workitem and adds to the client list", batchId, threadNo));

                        if (BaseSettings.EnableDetailedLog)
                            _logger.WriteLine(" Retrieves and add the workitem to the client list - ID-" + workItem.WorkItemID + "-" + DateTime.Now);

                        _cALClientListItem = _cALEnumItem.Retrieve(EnumRetrieve.Retain);

                        if (BaseSettings.EnableDetailedLog)
                            _logger.WriteLine(" Retrieval Completed - ID-" + workItem.WorkItemID + "-" + DateTime.Now);
                    }

                    //CALClientListItem _cALClientListItem = _cALEnumItem.Retrieve(EnumRetrieve.Retain);

                    //Check if the item is in Archive or not.
                    CALWorkitem _cALWorkitem;
                    if (_cALEnumItem.Info.ServerName == "IMGSVR" || _cALEnumItem.Info.ServerName == "TSTSVR")
                        _cALWorkitem = _cALClientListItem.Open(WorkitemOpen.ReadOnly);
                    else
                        _cALWorkitem = _cALClientListItem.Open(WorkitemOpen.Archive);
                    
                    CALDocument _cALDocument = (CALDocument)_cALWorkitem;
                    if (_cALDocument !=null)
                    {
                        //Retrieve pages and Annotations
                        if (!BaseSettings.EnableDetailedLog)
                            _logger.WriteLine(String.Format("> Batch {0} Thread {1} Retrieve pages and Annotations", batchId, threadNo));

                        //Commenting temporary
                        //Extracting the documents other than tiff images.
                        CALDocumentImports _cALDocumentImports = _cALDocument.Imports;
                        CALDocumentImport _cALDocumentImport = null;
                        int _documentImports = _cALDocumentImports.Count;
                        WorkItemDocumentCount = _documentImports;
                        workItem.NonTiffDocumentCount = WorkItemDocumentCount;
                        if (_documentImports > 0)
                        {
                            for (int cnt = 1; cnt <= _documentImports; cnt++)
                            {
                                _processSummary.OtherTypeDoc++;

                                lblOther.Invoke((MethodInvoker)delegate ()
                                {
                                    lblOther.Text = _processSummary.OtherTypeDoc.ToString();
                                });

                                if (BaseSettings.EnableDetailedLog)
                                    _logger.WriteLine(" Extraction of Other Type Document Started- ID-" + workItem.WorkItemID + "-" + DateTime.Now);

                                _cALDocumentImport = _cALDocumentImports.Item(cnt);

                                if (!Path.HasExtension(BaseSettings.FinalLocation + @"\Document\Other_Type_Document\" + workItem.ID + "_" + cnt.ToString() + "_" + _cALDocumentImport.Name.Replace("<", "").Replace(",", "").Replace(">", "").Replace(@"""", "")))
                                {
                                    if (!File.Exists(BaseSettings.FinalLocation + @"\Document\Other_Type_Document\" + workItem.ID + "_" + cnt.ToString() + "_" + _cALDocumentImport.Name.Replace("<", "").Replace(",", "").Replace(">", "").Replace(@"""", "") + "." + _cALDocumentImport.Class.ToLower()))
                                    _cALDocumentImport.Extract(BaseSettings.FinalLocation + @"\Document\Other_Type_Document\" + workItem.ID + "_" + cnt.ToString() + "_" + _cALDocumentImport.Name.Replace("<", "").Replace(",", "").Replace(">", "").Replace(@"""", "") + "." + _cALDocumentImport.Class.ToLower());
                                }                                    
                                else
                                {
                                    if(!File.Exists(BaseSettings.FinalLocation + @"\Document\Other_Type_Document\" + workItem.ID + "_" + cnt.ToString() + "_" + _cALDocumentImport.Name.Replace("<", "").Replace(",", "").Replace(">", "").Replace(@"""", "")))
                                    _cALDocumentImport.Extract(BaseSettings.FinalLocation + @"\Document\Other_Type_Document\" + workItem.ID + "_" + cnt.ToString() + "_" + _cALDocumentImport.Name.Replace("<", "").Replace(",", "").Replace(">", "").Replace(@"""", ""));
                                }
                                   

                                if (BaseSettings.EnableDetailedLog)
                                    _logger.WriteLine(" Extraction of Other Type Document Completed - ID-" + workItem.WorkItemID + "-" + DateTime.Now);
                            }
                        }
                        //============================================
                        CALPages _cALPages = _cALDocument.Pages;
                        if (_cALPages!=null)
                        { 
                            CALPage _cALPage;
                            int _documentCount = _cALPages.Count;
                            workItem.TiffDocumentCount = _documentCount;
                            WorkItemDocumentCount += _documentCount;
                            workItem.DocumentCount = WorkItemDocumentCount;
                            string strimgFileName ="";
                            string stranofileName = "";
                            string strimgfilePath = "";
                            string stranofilePath = "";
                            if (_cALPages.Count > 0)
                            {
                                //Retrive each document from the workItem
                                for (int cnt=1;cnt<= _documentCount;cnt++)
                                {                                                                      

                                    _cALPage = _cALPages.Item(cnt);
                                    if (_cALPage != null)
                                    {
                                        strimgFileName = @"\" + workItem.ID + "_" + _cALPage.Name.Replace("<", "").Replace(",","").Replace(">", "").Replace(@"""", "") + "_"+ cnt.ToString() + ".TIF";
                                        stranofileName = @"\" + workItem.ID + "_" + _cALPage.Name.Replace("<", "").Replace(",", "").Replace(">", "").Replace(@"""", "") + "_" + cnt.ToString() + ".Ano";
                                        if (workItem.WorkItemType == ItemType.Document)
                                        {
                                            strimgfilePath = BaseSettings.StagingLocation + @"\Document\Image" + strimgFileName;
                                            stranofilePath = BaseSettings.StagingLocation + @"\Document\Annotation" + stranofileName;
                                        }
                                        else
                                        {
                                            strimgfilePath = BaseSettings.StagingLocation + @"\Branding\Image" + strimgFileName;
                                            stranofilePath = BaseSettings.StagingLocation + @"\Branding\Annotation" + stranofileName;
                                        }

                                        workItem.HasPages = "Y";
                                        //If the page has an annotation then create the image and annotation files in the staging location, After the 
                                        //burning process the final image and the annotation file will be moved to the final and archive locations respectively.
                                        if (!BaseSettings.EnableDetailedLog)
                                            _logger.WriteLine(String.Format("> Batch {0} Thread {1} Created image file {2}", batchId, threadNo, strimgfilePath)); 
                                        
                                        if (_cALPage.MarkupType == MarkupFormat.Version2)
                                        {
                                            //Retrieves the image file for this page and save it in the staging location. Overwrite the image file if Exist.
                                            if (BaseSettings.EnableDetailedLog)
                                                _logger.WriteLine(" Extracting tiff image and its annotation file Started - ID-" + workItem.WorkItemID + "-" + DateTime.Now);

                                            _cALPage.GetPage(strimgfilePath, PageGet.OverwriteExisting);

                                            //Retrieves the 14W markups file for this page. 
                                            _cALPage.GetMarkups(stranofilePath, PageGetMarkups.OverwriteExisting);

                                            if (BaseSettings.EnableDetailedLog)
                                                _logger.WriteLine(" Extracting tiff image and its annotation file Completed - ID-" + workItem.WorkItemID + "-" + DateTime.Now);

                                            if (!BaseSettings.EnableDetailedLog)
                                                _logger.WriteLine(String.Format("> Batch {0} Thread {1} Created annotation file {2}", batchId, threadNo, stranofilePath));

                                                workItem.HasAnnotation = "Y";
                                                TiffDocWithAnnotation++;

                                            _processSummary.Burned++;
                                            lblBurned.Invoke((MethodInvoker)delegate () {
                                                lblBurned.Text = _processSummary.Burned.ToString();
                                            });
                                        }
                                        else
                                        {
                                            workItem.HasAnnotation = "N";
                                            if (workItem.WorkItemType== ItemType.Document)
                                                strimgfilePath = BaseSettings.FinalLocation + @"\Document\Image_withOut_Annotation" + strimgFileName;
                                            else
                                                strimgfilePath = BaseSettings.FinalLocation + @"\Branding\Image_withOut_Annotation" + strimgFileName;

                                            //Retrieves the image file for this page and save it in the final location. Overwrite the image file if Exist.
                                            if (BaseSettings.EnableDetailedLog)
                                                _logger.WriteLine(" Extracting tiff image Started - ID-"+ workItem.WorkItemID + "-" + DateTime.Now);

                                             _cALPage.GetPage(strimgfilePath, PageGet.OverwriteExisting);

                                            if (BaseSettings.EnableDetailedLog)
                                                _logger.WriteLine(" Extracting tiff image Completed- ID-" + workItem.WorkItemID + "-" + DateTime.Now);

                                            //Convert the TIF image to pdf.
                                            if (BaseSettings.EnableDetailedLog)
                                                _logger.WriteLine(" Pdf conversion Started- ID-" + workItem.WorkItemID + "-" + DateTime.Now);

                                            //ConvertImageWithMultiplePagesToPdf(strimgfilePath, workItem.WorkItemType);
                                            ConvertImageWithPdfusingItext(strimgfilePath, workItem.WorkItemType);
                                            //ConvertImagetoPdf(strimgfilePath, workItem.WorkItemType);
                                            if (BaseSettings.EnableDetailedLog)
                                                _logger.WriteLine(" Pdf conversion Completed - ID-" + workItem.WorkItemID + "-" + DateTime.Now);
                                        }
                                    }
                                }

                                workItem.TiffDocumentWithAnnotationCount = TiffDocWithAnnotation;

                            }  
                            _cALPages = null;
                        }
                        else
                        {
                            _logger.WriteLine("> The document doesn't have page.");                            
                        } 
                        _cALDocument.Close(WorkitemClose.None);
                        //_cALClientList.Remove(_cALClientListItem);
                        _cALClientList.Clear(ClientListClear.None);
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
            catch (Exception ex)
            {
                
                workItem.HasError = "Y";
                workItem.HasPages = "N";
                workItem.HasAnnotation = "N";
                workItem.DocumentCount = 0;
                workItem.TiffDocumentCount = 0;
                workItem.TiffDocumentWithAnnotationCount = 0;
                //_error++;               
                //ProcessSummary("");
                throw;
            }
        }
        private void EndProcess()
        {
            CalculateTotalTime();
            ProcessSummary("final");
            //Removing the workitems from the batch.
            //DocumentProcessor.RemoveBatchWorkItem(Convert.ToInt32(BaseSettings.Batch));
            UpdateStatus(String.Format("Processing Completed"));            
            string strMessage = String.Format("Processed {0} documents. Total time taken to complete the process is {1}, Process Starts @ {2} and Ends @ {3}",_workItemCount.ToString(), Math.Round(_timeElapsed, 2).ToString(), _timeStart.ToString(), _timeEnd.ToString());
            MessageBox.Show(strMessage);
        }
        private void UpdateStatus(string Message)
        {
            _processSummary.Summary = Message;
            if (chkbMultiThread.Checked)
            {
                lblStatus.Invoke((MethodInvoker)delegate ()
                {
                    lblStatus.Text = _processSummary.Summary;
                });
            }
            else
            {
                lblStatus.Text = _processSummary.Summary;
                lblStatus.Update();
            }

        }
        private void CalculateTotalTime()
        {           
            _timeEnd = DateTime.Now;
            _timeElapsed = _timeEnd.Subtract(_timeStart).TotalMinutes; 
        }
        private void BurnInAnnotation(string imagefile, string annotationfile)
        {
            //Using the ImageEdit Control of "Imaging for windows"
            try
            {
                //_burned++;
                _processSummary.Burned ++;
                lblBurned.Invoke((MethodInvoker)delegate () {
                    lblBurned.Text = _processSummary.Burned.ToString();
                });

                imgEdit.Image = imagefile;
                imgEdit.Page = 1;
                imgEdit.Display();
                int pageCount = imgEdit.PageCount;
                imgEdit.LoadAnnotations(annotationfile, 1, pageCount, 0);
                imgEdit.Save();
                //Burn-In Annotations
                for (int pageCnt = 1; pageCnt <= pageCount; pageCnt++)
                {
                    imgEdit.Page = pageCnt;
                    Application.DoEvents();
                    imgEdit.Display();                    
                    Task.Delay(1000);
                    Application.DoEvents();
                    imgEdit.BurnInAnnotations(0, 2);
                    Application.DoEvents();
                    Application.DoEvents();
                    imgEdit.Save();
                    Application.DoEvents();
                }
                imgEdit.Image = "";
                imgEdit.ClearDisplay();
                _logger.WriteLine(String.Format(" > Burned the annotation into the file"));
            }
            catch(Exception)
            {
                throw;
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if(_logger!=null)
              _logger.CloseLogger();

            Application.Exit();
        }
        private void ProcessSummary(string stage)
        {              
            lblCompleted.Invoke((MethodInvoker)delegate ()  {
                lblCompleted.Text = _processSummary.Completed.ToString();                
                lblBurned.Text = _processSummary.Burned.ToString();
                lblOther.Text = _processSummary.OtherTypeDoc.ToString();
                lblError.Text = _processSummary.Error.ToString();
                if (stage == "final")
                   lblTime.Text = Math.Round(_timeElapsed,2).ToString()+" min";
            });            
            

            //lblCompleted.Text = _processed.ToString();
            //lblCompleted.Update();
            //lblError.Text = _processSummary.Error.ToString();
            //lblError.Update();
            //lblBurned.Text = _processSummary.Burned.ToString();
            //lblBurned.Update();
            //if (stage=="final")
            //  lblTime.Text = Math.Round(_timeElapsed,2).ToString()+" min";
        }        
        private void ProcessDocuments()
        {
            string _strimagefile = "";
            string _strannotation = "";            
            foreach(string annotationFile in Directory.GetFiles(BaseSettings.StagingLocation + @"\Annotation","*.Ano"))
            {
               _strannotation = Path.GetFileName(annotationFile);
               _strimagefile = BaseSettings.StagingLocation + @"\Image\"+ _strannotation.Replace(".Ano",".TIF");
               bool _hasError = false;
               try
               { 
                  if (File.Exists(_strimagefile) && File.Exists(annotationFile))
                  {
                     BurnInAnnotation(_strimagefile, annotationFile);

                     //Convert the TIF image to pdf.
                     ConvertImageToPdf(_strimagefile);

                     //Move the annotation file to the archive location.
                     File.Move(annotationFile, BaseSettings.ArchiveLocation + @"\Annotation\" + _strannotation);
                     //Move the final image file to the final location.
                     File.Move(_strimagefile, BaseSettings.ArchiveLocation + @"\Image\" + Path.GetFileName(_strimagefile));
                   }
               }
               catch (Exception ex)
               {
                _hasError = true;
                _logger.WriteLine("> Error in moving annotation file to archive location. " + ex.Message);
               }
               if (_hasError) continue;
            }
            
        }
        private void ConvertImageToPdf(string srcImageFile)
        {            
            string _PdfFileName = Path.GetFileName(srcImageFile).Replace(".TIF", ".pdf");
            
            ImageData imageData = ImageDataFactory.Create(srcImageFile);
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(BaseSettings.FinalLocation + @"\Pdf_WithOut_Annotation\" + _PdfFileName));
            Document document = new Document(pdfDocument);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData);
           
            image.SetWidth(pdfDocument.GetDefaultPageSize().GetWidth() - 50);
            image.SetAutoScaleHeight(true);
            document.Add(image);

            pdfDocument.Close();
        }
        private void ConvertImageWithMultiplePagesToPdf(string srcImageFile,ItemType workItemType)
        {
            string _PdfFileName = Path.GetFileName(srcImageFile).Replace(".TIF", ".pdf");
            string _pdfFinalLocation = "";
            if (workItemType == ItemType.Document)
                _pdfFinalLocation = BaseSettings.FinalLocation + @"\Document\Pdf_WithOut_Annotation\" + _PdfFileName;
            else
                _pdfFinalLocation = BaseSettings.FinalLocation + @"\Branding\Pdf_WithOut_Annotation\" + _PdfFileName;

            ImageData imageData = null; 
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(_pdfFinalLocation));
            Document document = new Document(pdfDocument);
            Bitmap _bitMap = new Bitmap(srcImageFile);
            int _totalpage = _bitMap.GetFrameCount(System.Drawing.Imaging.FrameDimension.Page);
            _bitMap.Dispose();
            _bitMap = null;
            var MemoryStream = new MemoryStream();
            for (int i=0;i< _totalpage;i++)
            {
                 Bitmap _bitMapTemp = new Bitmap(srcImageFile);
                _bitMapTemp.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Page, i);

                //   using (var MemoryStream= new MemoryStream())
                //   {
              
                    _bitMapTemp.Save(MemoryStream, System.Drawing.Imaging.ImageFormat.Tiff);
                     imageData = ImageDataFactory.Create(MemoryStream.ToArray());
                    iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData);
                    document.Add(image);
                //     MemoryStream.Dispose();
                //  }       

                _bitMapTemp.Dispose();
                _bitMapTemp = null;
            }
            _bitMap.Dispose();
            if (!pdfDocument.IsCloseWriter())
                pdfDocument.Close();
        }

        private void ConvertImageWithPdfusingItext(string srcImageFile, ItemType workItemType)
        {
            string _PdfFileName = Path.GetFileName(srcImageFile).Replace(".tif", ".pdf");
            string _pdfFinalLocation = "";
            if (workItemType == ItemType.Document)
            {
                //_pdfFinalLocation = BaseSettings.FinalLocation + @"\Document\Pdf_WithOut_Annotation\" + _PdfFileName;
                _pdfFinalLocation = @"D:\Test\final\Document\Pdf_WithOut_Annotation\" + _PdfFileName;
            }
                
            else
            {
                //_pdfFinalLocation = BaseSettings.FinalLocation + @"\Branding\Pdf_WithOut_Annotation\" + _PdfFileName;
                _pdfFinalLocation =  @"D:\Test\final\Branding\Pdf_WithOut_Annotation\" + _PdfFileName;
            }



            ImageData imageData = null;
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(_pdfFinalLocation)))
            {
                Document document = new Document(pdfDocument);
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

        private void ConvertImagetoPdf(string srcImageFile, ItemType workItemType)
        {

            string _PdfFileName = Path.GetFileName(srcImageFile).Replace(".TIF", ".pdf");
            string _pdfFinalLocation = "";
            if (workItemType == ItemType.Document)
                _pdfFinalLocation = BaseSettings.FinalLocation + @"\Document\Pdf_WithOut_Annotation\" + _PdfFileName;
            else
                _pdfFinalLocation = BaseSettings.FinalLocation + @"\Branding\Pdf_WithOut_Annotation\" + _PdfFileName;

            List<System.Drawing.Image> images = new List<System.Drawing.Image>();
            Bitmap bitmap = (Bitmap)System.Drawing.Image.FromFile(srcImageFile);
            int count = bitmap.GetFrameCount(System.Drawing.Imaging.FrameDimension.Page);
            //for (int idx = 0; idx < count; idx++)
            //{
                // save each frame to a bytestream               
                bitmap.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Page, 1);
                MemoryStream byteStream = new MemoryStream();
                bitmap.Save(byteStream, ImageFormat.Tiff);

                // and then create a new Image from it
                images.Add(System.Drawing.Image.FromStream(byteStream));
            //}
           
        }

        private void btn_pdf_Click(object sender, EventArgs e)
        {
            string sourceFolder = @"D:\Test";
            string[] _tiffFiles;
            _tiffFiles = Directory.GetFiles(sourceFolder,"*.tif" );  
            foreach(string file in _tiffFiles)
            {
                ConvertImageWithPdfusingItext(file, ItemType.BrandingImage);
            }      
                
        }
    }
}

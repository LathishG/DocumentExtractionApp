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
using iText.IO.Source;
using iText.Kernel.Pdf.Canvas;


namespace DocumentProcess
{
    public partial class DocumentProcess : Form
    {
        private Logger _logger;
        private DateTime _timeStart;
        private DateTime _timeEnd;
        private int _documentCount;
        private double _timeElapsed;
        private ProcessSummary _processSummary = null;
        private bool disableButtonClick = false;
        public DocumentProcess()
        {
            InitializeComponent();
            _processSummary = new ProcessSummary();
            IntiProcess();
        }       
        private void IntiProcess()
        {  
            _logger = Logger.Instance;
            _logger.CreateLogFile("DocBurning_debug");

            lblDomain.Text = BaseSettings.WorkflowDomain;
            lblDomain.Update();
            lblUser.Text = BaseSettings.WorkflowUser;
            lblUser.Update();
            lblVmName.Text = BaseSettings.InstanceName;
            lblVmName.Update();

            _processSummary.Total = 0;
            _processSummary.Error = 0;
            _processSummary.Burned = 0;
            _processSummary.Completed = 0;
            _processSummary.TimeElasped = 0;
            lblStatus.Text = _processSummary.Summary;                       
            UpdateStatus("Idle......");
        }
        private void BurnInAnnotation(string imagefile, string annotationfile)
        {
            //Using the ImageEdit Control of "Imaging for windows"
            try
            {
                //_burned++;
                _processSummary.Burned++;               

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
            catch (Exception)
            {
                throw;
            }
        }
        private void ProcessDocuments()
        {
            string _strimagefile = "";
            string _strannotationfile = "";
            _timeStart = DateTime.Now;
            List<WorkItem> _workItems = DocumentProcessor.GetWorkItemForBurning(BaseSettings.InstanceName,Convert.ToInt32(BaseSettings.BatchRecordCount),BaseSettings.ProcessedDate);
            string[] _annotationsforburning;

            //_documentCount = _fileforburning.Length;

            _documentCount = _workItems.Count;
            if (_documentCount > 0)
            {                 
                foreach (WorkItem _workItem in _workItems)
                {
                    bool _hasError = false;
                    string _errormessage = "";
                    bool _hasErrorInReadingAnnotation = false;
                    
                    string annotationFile = "";
                    try
                    {
                        //If the workitem has multiple annotations, burn the annotation one by one. Get all annotations by workitem id.
                        if (_workItem.WorkItemType == ItemType.Document)
                            _annotationsforburning = Directory.GetFiles(BaseSettings.StagingLocation + @"\Document\Annotation", _workItem.ID + "*");
                        else
                            _annotationsforburning = Directory.GetFiles(BaseSettings.StagingLocation + @"\Branding\Annotation", _workItem.ID + "*");

                        if (_annotationsforburning.Length > 0)
                        { 
                            foreach (string _annotationFile in _annotationsforburning)
                            {
                                _strannotationfile = Path.GetFileName(_annotationFile);
                                UpdateStatus(String.Format("Processing the document {0}", _strannotationfile));

                                //Check whether the workitem is a document or a branding image.
                                if (_workItem.WorkItemType == ItemType.Document)
                                    _strimagefile = BaseSettings.StagingLocation + @"\Document\Image\" + _strannotationfile.Replace(".Ano", ".TIF");
                                else
                                    _strimagefile = BaseSettings.StagingLocation + @"\Branding\Image\" + _strannotationfile.Replace(".Ano", ".TIF");

                                try
                                {
                                    if (File.Exists(_strimagefile) && File.Exists(_annotationFile))
                                    {
                                        BurnInAnnotation(_strimagefile, _annotationFile);

                                        //Convert the TIF image to pdf.
                                        //ConvertImageWithMultiplePagesToPdf(_strimagefile, _workItem.WorkItemType);
                                        //ConvertImageToPdf(_strimagefile);
                                        //ConvertImageWithPdfusingDynamicPdf(_strimagefile, _workItem.WorkItemType);
                                        ConvertImageWithPdfusingItext(_strimagefile, _workItem.WorkItemType);
                                        
                                        //Move the annotation file to the archive location.
                                        File.Move(_annotationFile, BaseSettings.ArchiveLocation + @"\Annotation\" + _strannotationfile);

                                        //Move the final image file to the final location.
                                        if (_workItem.WorkItemType == ItemType.Document)
                                            File.Move(_strimagefile, BaseSettings.FinalLocation + @"\Document\Image_with_Annotation\" + Path.GetFileName(_strimagefile));
                                        else
                                            File.Move(_strimagefile, BaseSettings.FinalLocation + @"\Branding\Image_with_Annotation\" + Path.GetFileName(_strimagefile));
                                    }
                                    else
                                    {
                                        _logger.WriteLine("> Image file no longer available in the location. ");
                                        _workItem.HasError = "Y";
                                        _workItem.Comments = "Image file no longer available in the location while burining.";
                                        _workItem.Status = "Extracted_Hold";
                                        DocumentProcessor.UpdateWorkItemAsHold(_workItem);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _hasError = true;
                                    _errormessage = ex.Message;
                                    _logger.WriteLine("> Error in moving annotation file to archive location. " + ex.Message);
                                }

                            }
                        //Update the status
                        _workItem.HasError = _hasError == true ? "Y" : "N";
                        _workItem.Comments = _hasError == true ? "Error while burning the annotation. Exception: " + _errormessage : "Annotation burned successfully ";
                        _workItem.Status = _hasError == true ? "Error" : "Completed";
                        DocumentProcessor.UpdateWorkItemAfterBurning(_workItem);

                       }
                       else
                       {
                            _logger.WriteLine("> File No longer available in the location. " );
                            _workItem.HasError = "Y";
                            _workItem.Comments = "File No longer available in the location while burining.";
                            _workItem.Status = "Extracted_Hold";
                            DocumentProcessor.UpdateWorkItemAsHold(_workItem);
                        }
                       if (_hasError) continue;
                    }
                    catch(Exception ex)
                    {
                        _hasErrorInReadingAnnotation = true;                        
                        _logger.WriteLine("> Error in reading annotation file from staging location. " + ex.Message);
                        _workItem.HasError = "Y";
                        _workItem.Comments = "Error while reading the annotation" ;
                        _workItem.Status = "Extracted_Hold";
                        DocumentProcessor.UpdateWorkItemAsHold(_workItem);
                    }

                    if (_hasErrorInReadingAnnotation) continue;
                    //------
                }
                _timeEnd = DateTime.Now;
                _timeElapsed = _timeEnd.Subtract(_timeStart).TotalMinutes;
                string strMessage = String.Format("Processed {0} documents. Total time taken to complete the process is {1}, Process Starts @ {2} and Ends @ {3}", _documentCount.ToString(), Math.Round(_timeElapsed, 2).ToString(), _timeStart.ToString(), _timeEnd.ToString());
                //MessageBox.Show(strMessage);
            }
            else
            {
                string strMessage = String.Format("There is no document to process.");
                MessageBox.Show(strMessage);
            }

        }
        private void ConvertImageToPdf(string srcImageFile)
        {            
            string _PdfFileName = Path.GetFileName(srcImageFile).Replace(".TIF", ".pdf");
            
            ImageData imageData = ImageDataFactory.Create(srcImageFile);

            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(BaseSettings.FinalLocation + @"\Pdf_With_Annotation\" + _PdfFileName));
            Document document = new Document(pdfDocument);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData);
            image.SetWidth(pdfDocument.GetDefaultPageSize().GetWidth() - 50);
            image.SetAutoScaleHeight(true);
            document.Add(image);

            pdfDocument.Close();
        }
        private void UpdateStatus(string Message)
        {
            _processSummary.Summary = Message;
            lblStatus.Text = _processSummary.Summary;
            lblStatus.Update();
        }
        private void btnBurning_Click(object sender, EventArgs e)
        {
            //IntiProcess();
            for (int itr=1;itr<=4;itr++)
            {
                if(!disableButtonClick)
                {
                    ProcessDocuments();
                }
                if (itr == 4)
                    disableButtonClick = true;
            }
            MessageBox.Show("Completed");
            
        }
        private void ConvertImageWithMultiplePagesToPdf(string srcImageFile, ItemType workItemType)
        {
            string _PdfFileName = Path.GetFileName(srcImageFile).Replace(".TIF", ".pdf");
            string _pdfFinalLocation = "";
            if (workItemType == ItemType.Document)
                _pdfFinalLocation = BaseSettings.FinalLocation + @"\Document\Pdf_With_Annotation\" + _PdfFileName;
            else
                _pdfFinalLocation = BaseSettings.FinalLocation + @"\Branding\Pdf_With_Annotation\" + _PdfFileName;

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
                    Bitmap _bitMapTemp = new Bitmap(srcImageFile);
                    _bitMapTemp.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Page, i);

                    using (var MemoryStream = new MemoryStream())
                    {
                        _bitMapTemp.Save(MemoryStream, System.Drawing.Imaging.ImageFormat.Tiff);
                        imageData = ImageDataFactory.Create(MemoryStream.ToArray());
                        iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData);
                        document.Add(image);
                    }

                    _bitMapTemp.Dispose();
                    _bitMapTemp = null;
                }

                _bitMap.Dispose();
                if (!pdfDocument.IsCloseWriter())
                   pdfDocument.Close();
            }
        }
    
        private void ConvertImageWithPdfusingDynamicPdf(string srcImageFile, ItemType workItemType)
        {
            string _PdfFileName = Path.GetFileName(srcImageFile).Replace(".TIF", ".pdf");
            string _pdfFinalLocation = "";
            if (workItemType == ItemType.Document)
                _pdfFinalLocation = BaseSettings.FinalLocation + @"\Document\Pdf_With_Annotation\" + _PdfFileName;
            else
                _pdfFinalLocation = BaseSettings.FinalLocation + @"\Branding\Pdf_With_Annotation\" + _PdfFileName;

            ceTe.DynamicPDF.Conversion.ImageConverter imageConverter = new ceTe.DynamicPDF.Conversion.ImageConverter(srcImageFile);
            imageConverter.Convert(_pdfFinalLocation);

        }

        private void ConvertImageWithPdfusingItext(string srcImageFile, ItemType workItemType)
        {
            string _PdfFileName = Path.GetFileName(srcImageFile).Replace(".TIF", ".pdf");
            string _pdfFinalLocation = "";
            if (workItemType == ItemType.Document)
                _pdfFinalLocation = BaseSettings.FinalLocation + @"\Document\Pdf_With_Annotation\" + _PdfFileName;
            else
                _pdfFinalLocation = BaseSettings.FinalLocation + @"\Branding\Pdf_With_Annotation\" + _PdfFileName;

            

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
                    using(Bitmap _bitMapTemp = new Bitmap(srcImageFile))
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

        private void ConvertToPdf(string srcImageFile, ItemType workItemType)
        {
            string _PdfFileName = Path.GetFileName(srcImageFile).Replace(".TIF", ".pdf");
            string _pdfFinalLocation = "";

            var _tiffurl = new System.Uri(srcImageFile);

            if (workItemType == ItemType.Document)
                _pdfFinalLocation = BaseSettings.FinalLocation + @"\Document\Pdf_With_Annotation\" + _PdfFileName;
            else
                _pdfFinalLocation = BaseSettings.FinalLocation + @"\Branding\Pdf_With_Annotation\" + _PdfFileName;

            RandomAccessFileOrArray _randomAccessFileOrArray = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateBestSource(srcImageFile));
            int _totalpage = TiffImageData.GetNumberOfPages(_randomAccessFileOrArray);
            
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(_pdfFinalLocation)))
            {
                for (int i = 0; i < _totalpage; i++)
                {
                    ImageData tiffImage = ImageDataFactory.CreateTiff(_tiffurl,true,i,true);
                    Rectangle tiffPageSize = new Rectangle();
                    PdfPage newPage = pdfDocument.AddNewPage();
                    PdfCanvas canvas = new PdfCanvas(newPage);
                    //canvas.AddImagetiffImage, tiffPageSize, false);
                }
            }
         }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_logger != null)
                _logger.CloseLogger();

            Application.Exit();
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
               
    }
}

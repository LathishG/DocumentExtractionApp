using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocumentExtraction.BLL.Service;
using WinSCP;
using System.IO;
using CsvHelper;
using System.Globalization;
using DocumentExtraction.BLL.Model;

namespace DocusignKofaxUtility
{
    public class DocumentUpload
    {
        private Logger _logger;
        private DateTime _timeStart;
        private DateTime _timeEnd;
        private double _timeElapsed;

        private void IntiProcess()
        {
            _logger = Logger.Instance;
            _logger.CreateLogFile("Docusign_Kofax_Process_Log");
            _timeStart = DateTime.Now;
        }

        private void GetScannedDocuments()
        {
            List<ScannedDocuments> scannedDocuments ;
            List<string> directoryList = Directory.GetDirectories(BaseSettings.SourcePdfLocation).ToList<string>();
            List<string> scannedDocumentList = new List<string>();
            foreach (string subFolder in directoryList)
            {
                scannedDocuments = new List<ScannedDocuments>();
                //Get the list of files from the directory
                scannedDocumentList = Directory.GetFiles(subFolder).ToList<string>();
                string workItemID = "";
                if(scannedDocumentList.Count>1)
                {
                    ScannedDocuments document = new ScannedDocuments();
                    foreach (string fileName in scannedDocumentList)
                    {                        
                        string fileExtension = Path.GetExtension(fileName);
                        if (fileExtension.ToLower() == ".pdf")
                        {
                            document.DocumentName = fileName;
                        }
                        if (fileExtension.ToLower() == ".csv")
                        {
                           Dictionary<string,string> indexFileData=ReadDataFromIndexFile(fileName);
                           DocumentAttributeModel8 metaData= CreateMetadata(indexFileData);
                           workItemID = metaData.WorkItemID;
                           string metaDataFileName = string.Format(BaseSettings.MetadataLocation+ @"\MetadataFile_{0}_{1}.csv", workItemID, DateTime.Now.ToString("ddmmyyyyhhmmss"));
                           string documentName = string.Format("{0}_{1}.pdf", workItemID, DateTime.Now.ToString("ddmmyyyyhhmmss"));
                            metaData.FileName = documentName;
                            document.RemoteDocumentName = documentName;
                            document.RemoteMetaDataFileName = Path.GetFileName(metaDataFileName);
                            List<dynamic> documentMetaData = new List<dynamic>();
                            documentMetaData.Add(metaData);
                            bool isMetaDataFileCreated = GenerateMetaDataFile(documentMetaData, metaDataFileName);
                            if(isMetaDataFileCreated)
                            {
                                document.MetaDataFile = metaDataFileName;
                            }
                        }
                    }
                    if(document.MetaDataFile!="" && document.DocumentName!="")
                       scannedDocuments.Add(document);
                }
                if( scannedDocuments.Count> 0)
                {
                    bool isFileUploaded = UploadFileToSftp(scannedDocuments);

                    string archiveFolderName = string.Format("{0}_{1}", workItemID,DateTime.Now.ToString("ddmmyyyyhhmmss"));

                    MoveScannedFolderToArchive(subFolder, archiveFolderName);                   
                }
                    
            }
        }

        private Dictionary<string, string> ReadDataFromIndexFile(string fileName)
        {
            Dictionary<string, string> metaData = new Dictionary<string, string>();
            using (var reader = new StreamReader(fileName))
            {
                string[] fileData = reader.ReadLine().Split(new char[]{ ','});

                for(int i=0;i< fileData.Length-1; i++)
                {
                    metaData.Add(fileData[i].Replace("\"","").ToString(), fileData[i + 1].Replace("\"", "").ToString());
                    i = i + 1;
                }
            }
            return metaData;
        }
        private DocumentAttributeModel8 CreateMetadata(Dictionary<string,string> scannedData)
        {   
            DocumentAttributeModel8 fileMetaData = new DocumentAttributeModel8
            {
                EntityType = "",
                BussinessName = "",
                FileNumber = "",
                DocumentDate = "",
                DocumentTitle = "",
                TransactionNumber = "",
                BusinessRegistrationDivision = "",
                Public = "",
                Suffix = "",
                WorkItemID = scannedData["WorkItemID"].ToString(),
                TypeofDocument="Approval",
                DocumentClass= scannedData["BREGS-NBR-LK-New"].ToString()
            };

            return fileMetaData;
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
        private bool UploadFileToSftp(List<ScannedDocuments> files)
        {
            bool isUploaded = false;
            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = BaseSettings.SFTPHostName,
                UserName = BaseSettings.SFTPUserName,
                //Password = BaseSettings.SFTPPassword,
                //SshHostKeyPolicy = SshHostKeyPolicy.GiveUpSecurityAndAcceptAny
                SshHostKeyFingerprint = BaseSettings.SshHostKey,
                SshPrivateKeyPath = BaseSettings.SshPrivateKeyPath
            };

            using (Session session = new Session())
            {
                //_logger.WriteLine(string.Format(" > Connecting to the SFTP {0} ", DateTime.Now.ToLongTimeString()));
                session.Open(sessionOptions);

                //Uploading documents to sftp
                foreach (ScannedDocuments file in files)
                {
                    //Uploading pdf documents
                    using (FileStream fileStream = new FileStream(file.DocumentName, FileMode.Open))
                    {
                        session.PutFile(fileStream, BaseSettings.RemoteFolderLocation + file.RemoteDocumentName);
                    }
                    //Uploading metadata file.
                    using (FileStream fileStream = new FileStream(file.MetaDataFile, FileMode.Open))
                    {
                        session.PutFile(fileStream, BaseSettings.RemoteFolderLocation + file.RemoteMetaDataFileName);
                    }
                }

                isUploaded = true;
            }
            return isUploaded;
        }
        private void MoveScannedFolderToArchive(string sourceFolder, string destinationFolderName)
        {            
            string destinationFolder = BaseSettings.ArchiveLocation + @"\" + destinationFolderName;

            if (!Directory.Exists(destinationFolder))
                Directory.CreateDirectory(destinationFolder);

            DirectoryInfo sourceDirectoryInfo = new DirectoryInfo(sourceFolder);

            foreach (FileInfo file in sourceDirectoryInfo.GetFiles())
            {
                if (Directory.Exists(destinationFolder))
                {
                    file.MoveTo($@"{destinationFolder}\{file.Name}");
                }
            }
            Directory.Delete(sourceFolder);
        }
        public void DoProcess()
        {
            this.GetScannedDocuments();
        }
    }


    public class ScannedDocuments
    {
        public string DocumentName { get; set; }
        public string MetaDataFile { get; set; }
        public string RemoteDocumentName { get; set; }
        public string RemoteMetaDataFileName { get; set; }
    }
}

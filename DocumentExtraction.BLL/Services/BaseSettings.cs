using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentExtraction.BLL.Service
{
    public static class BaseSettings
    {
        public static readonly string LogfilePath = ConfigurationManager.AppSettings["LogPath"];
        public static readonly string LogfileName = ConfigurationManager.AppSettings["LogFileName"];
        public static readonly string WorkflowUser = ConfigurationManager.AppSettings["WorkflowUser"];
        public static readonly string WorkflowPassword = ConfigurationManager.AppSettings["WorkflowPassword"];
        public static readonly string WorkflowDomain = ConfigurationManager.AppSettings["WorkflowDomain"];
        public static readonly string StagingLocation = ConfigurationManager.AppSettings["StagingLocation"];
        public static readonly string AnnotationFileLocation = ConfigurationManager.AppSettings["AnnotationFileLocation"];
        public static readonly string ArchiveLocation = ConfigurationManager.AppSettings["ArchiveLocation"];
        public static readonly string FinalLocation = ConfigurationManager.AppSettings["FinalLocation"];
        public static readonly string OracleDbConnection = ConfigurationManager.AppSettings["OracleDbConnection"];
        public static readonly string Batch = ConfigurationManager.AppSettings["BatchID"];
        public static readonly string BatchRecordCount = ConfigurationManager.AppSettings["BatchRecordCount"];
        public static readonly string MaxThreadCount = ConfigurationManager.AppSettings["MaxThreadCount"];
        public static readonly string InstanceName = ConfigurationManager.AppSettings["InstanceName"];
        public static readonly bool EnableDetailedLog =Convert.ToBoolean(ConfigurationManager.AppSettings["EnableDetailedLog"]);
        public static readonly string ProcessedDate =ConfigurationManager.AppSettings["ProcessedDate"];

        public static readonly string MetadataLocation=ConfigurationManager.AppSettings["MetadataLocation"]; 
        public static readonly string SourcePdfLocation =ConfigurationManager.AppSettings["SourcePdfLocation"];
        public static readonly string InitialRemoteFolderLocation = ConfigurationManager.AppSettings["InitialRemoteFolderLocation"];
        public static readonly string RemoteFolderLocation =ConfigurationManager.AppSettings["RemoteFolderLocation"]; 
        public static readonly string SFTPHostName= ConfigurationManager.AppSettings["SFTPHostName"]; 
        public static readonly string SFTPUserName=ConfigurationManager.AppSettings["SFTPUserName"];
        public static readonly string SFTPPassword= ConfigurationManager.AppSettings["SFTPPassword"];
        public static readonly string SshHostKey=ConfigurationManager.AppSettings["SshHostKey"];
        public static readonly string SshPrivateKeyPath=ConfigurationManager.AppSettings["SshPrivateKeyPath"];
        public static readonly string TransType = ConfigurationManager.AppSettings["TransType"];
        public static readonly string DocCount = ConfigurationManager.AppSettings["DocCount"];
        public static readonly string ProcessSummaryLocation = ConfigurationManager.AppSettings["ProcessSummaryLocation"];
    }
}

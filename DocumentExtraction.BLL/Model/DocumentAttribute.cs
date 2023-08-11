using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentExtraction.BLL.Model
{

    public interface IAttribute
    {
        string FileName { get; set; }
    }
    public class DocumentAttributeModel1: IAttribute
    {
        [Name("FileName")]
        [Index(0)]
        public string FileName { get; set; }

        [Name("doc_attrib_grp1.EntityType")]
        [Index(1)]
        public string EntityType { get; set; }

        [Name("doc_attrib_grp1.BusinessName")]
        [Index(2)]
        public string BussinessName { get; set; }
        [Name("doc_attrib_grp1.FileNumber")]
        [Index(3)]
        public string FileNumber { get; set; }
        [Name("doc_attrib_grp1.DocumentDate")]
        [Index(4)]
        public string DocumentDate { get; set; }
        [Name("doc_attrib_grp1.DocumentTitle")]
        [Index(5)]
        public string DocumentTitle { get; set; }

        [Name("doc_attrib_grp1.TransactionNumber")]
        [Index(6)]
        public string TransactionNumber { get; set; }
        [Name("doc_attrib_grp1.BusinessRegistrationDivision")]
        [Index(7)]
        public string BusinessRegistrationDivision { get; set; }
        [Name("doc_attrib_grp1.Public")]
        [Index(8)]
        public string Public { get; set; }
        [Name("doc_attrib_grp1.Suffix")]
        [Index(9)]
        public string Suffix { get; set; }
    }

    public class DocumentAttributeModel2 : IAttribute
    {
        [Name("FileName")]
        [Index(0)]
        public string FileName { get; set; }

        [Name("AphaRange")]
        [Index(1)]
        public string AlphaRange { get; set; }
        [Name("Quarter")]
        [Index(2)]
        public string Quarter { get; set; }
        [Name("Year")]
        [Index(3)]
        public string Year { get; set; }
        [Name("PartnershipName")]
        [Index(4)]
        public string PartnershipName { get; set; }
        [Name("DateReceived")]
        [Index(5)]
        public string DateReceived { get; set; }
        [Name("FileNumber")]
        [Index(6)]
        public string FileNumber { get; set; }
        [Name("doc_attrib_grp1.BusinessRegistrationDivision")]
        [Index(7)]
        public string BusinessRegistrationDivision { get; set; }

        [Name("doc_attrib_grp1.Public")]
        [Index(8)]
        public string Public { get; set; }
        
        [Name("doc_attrib_grp1.TransactionNumber")]
        [Index(9)]
        public string TransactionNumber { get; set; }

        [Name("doc_attrib_grp1.Suffix")]
        [Index(10)]
        public string Suffix { get; set; }

        [Name("doc_attrib_grp1.DocumentType")]
        [Index(11)]
        public string DocumentType { get; set; }
    }

    public class DocumentAttributeModel3:IAttribute
    {
        [Name("FileName")]
        [Index(0)]
        public string FileName { get; set; }

        [Name("Year")]
        [Index(1)]
        public string Year { get; set; }
        [Name("PartnetshipName")]
        [Index(2)]
        public string PartnershipName { get; set; }
        [Name("FileNumber")]
        [Index(3)]
        public string FileNumber { get; set; }

        [Name("doc_attrib_grp1.BusinessRegistrationDivision")]
        [Index(4)]
        public string BusinessRegistrationDivision { get; set; }

        [Name("doc_attrib_grp1.Suffux")]
        [Index(5)]
        public string Suffix { get; set; }

        [Name("AnnualStatementType")]
        [Index(6)]
        public string AnnualStatementType { get; set; }
        [Name("DateReceived")]
        [Index(7)]
        public string DateReceived { get; set; }
        [Name("doc_attrib_grp1.Public")]
        [Index(8)]
        public string Public { get; set; }

        [Name("doc_attrib_grp1.TransactionNumber")]
        [Index(9)]
        public string TransactionNumber { get; set; }
    }

    public class DocumentAttributeModel4 : IAttribute
    {
        [Name("FileName")]
        [Index(0)]
        public string FileName { get; set; }

        [Name("AssignmentDate")]
        [Index(1)]
        public string AssignmentDate { get; set; }
        [Name("TradeName")]
        [Index(2)]
        public string TradeName { get; set; }
        [Name("CertificateNumber")]
        [Index(3)]
        public string CertificateNumber { get; set; }
        [Name("Assignor")]
        [Index(4)]
        public string Assignor { get; set; }
        [Name("Assignee")]
        [Index(5)]
        public string Assignee { get; set; }
        [Name("doc_attrib_grp1.Public")]
        [Index(6)]
        public string Public { get; set; }

        [Name("doc_attrib_grp1.TransactionNumber")]
        [Index(7)]
        public string TransactionNumber { get; set; }
    }

    public class DocumentAttributeModel5 : IAttribute
    {
        [Name("FileName")]
        [Index(0)]
        public string FileName { get; set; }

        [Name("ApplicantName")]
        [Index(1)]
        public string ApplicantName { get; set; }
        [Name("FileNumber")]
        [Index(2)]
        public string FileNumber { get; set; }
        [Name("TradeName/TradeMark")]
        [Index(3)]
        public string TMName { get; set; }
        [Name("CertificateNumber")]
        [Index(4)]
        public string CertificateNumber { get; set; }
        [Name("RegistrationDate")]
        [Index(5)]
        public string RegistrationDate { get; set; }

        [Name("doc_attrib_grp1.BusinessRegistrationDivision")]
        [Index(6)]
        public string BusinessRegistrationDivision { get; set; }

        [Name("doc_attrib_grp1.Suffux")]
        [Index(7)]
        public string Suffix { get; set; }

        [Name("doc_attrib_grp1.Public")]
        [Index(8)]
        public string Public { get; set; }

        [Name("doc_attrib_grp1.TransactionNumber")]
        [Index(9)]
        public string TransactionNumber { get; set; }

        [Name("TM/TN/SM Name First Letter")]
        [Index(10)]
        public string TMFirstName { get; set; }
    }

    public class DocumentAttributeModel6 : IAttribute
    {
        [Name("FileName")]
        [Index(0)]
        public string FileName { get; set; }

        [Name("doc_attrib_grp1.FileNumber")]
        [Index(3)]
        public string FileNumber { get; set; }

        [Name("doc_attrib_grp1.DocumentDate")]
        [Index(4)]
        public string DocumentDate { get; set; }

        [Name("doc_attrib_grp1.DocumentTitle")]
        [Index(5)]
        public string DocumentTitle { get; set; }

        [Name("doc_attrib_grp1.TransactionNumber")]
        [Index(6)]
        public string TransactionNumber { get; set; }

        [Name("doc_attrib_grp1.BusinessRegistrationDivision")]
        [Index(7)]
        public string BusinessRegistrationDivision { get; set; }

        [Name("doc_attrib_grp1.Public")]
        [Index(8)]
        public string Public { get; set; }

        [Name("doc_attrib_grp1.Suffix")]
        [Index(9)]
        public string Suffix { get; set; }
    }

    public class DocumentAttributeModel7 : IAttribute
    {
        [Name("FileName")]
        [Index(0)]
        public string FileName { get; set; }

        [Name("Year")]
        [Index(1)]
        public string Year { get; set; }

        [Name("doc_attrib_grp1.Public")]
        [Index(8)]
        public string Public { get; set; }

        [Name("doc_attrib_grp1.TransactionNumber")]
        [Index(9)]
        public string TransactionNumber { get; set; }
    }

    public class DocumentAttributeModel8 : IAttribute
    {
        [Name("FileName")]
        [Index(0)]
        public string FileName { get; set; }

        [Name("doc_attrib_grp8.EntityType")]
        [Index(1)]
        public string EntityType { get; set; }

        [Name("doc_attrib_grp8.BusinessName")]
        [Index(2)]
        public string BussinessName { get; set; }
        [Name("doc_attrib_grp8.FileNumber")]
        [Index(3)]
        public string FileNumber { get; set; }
        [Name("doc_attrib_grp8.DocumentDate")]
        [Index(4)]
        public string DocumentDate { get; set; }
        [Name("doc_attrib_grp8.DocumentTitle")]
        [Index(5)]
        public string DocumentTitle { get; set; }

        [Name("doc_attrib_grp8.TransactionNumber")]
        [Index(6)]
        public string TransactionNumber { get; set; }
        [Name("doc_attrib_grp8.BusinessRegistrationDivision")]
        [Index(7)]
        public string BusinessRegistrationDivision { get; set; }
        [Name("doc_attrib_grp8.Public")]
        [Index(8)]
        public string Public { get; set; }
        [Name("doc_attrib_grp8.Suffix")]
        [Index(9)]
        public string Suffix { get; set; }

        [Name("doc_attrib_grp8.WorkItemID")]
        [Index(9)]
        public string WorkItemID { get; set; }

        [Name("doc_attrib_grp8.TypeofDocument")]
        [Index(9)]
        public string TypeofDocument { get; set; }

        [Name("doc_attrib_grp8.DocumentClass")]
        [Index(9)]
        public string DocumentClass { get; set; }
    }
    public class WorkItemTransaction
    {
        public string ObjectID { get; set; }
        public ItemType WorkItemType { get; set; }
        public string TransactionType { get; set; }
        public string EntityType { get; set; }
        public string BusinessName { get; set; }
        public string FileNumber { get; set; }
        public string DocumentDate { get; set; }
        public string DocumentTitle { get; set; }
        public string BusinessRegistrationDivision { get; set; }
        public string Suffix { get; set; }
        public string Public { get; set; }
        public string TransactionNumber { get; set; }
        public string AlphaRange { get; set; }
        public string Quarter { get; set; }
        public string Year { get; set; }
        public string PartnershipName { get; set; }
        public string DocumentDateReceived { get; set; }
        public string AnnualStatementType { get; set; }
        public string AssignmentDate { get; set; }
        public string TradeName { get; set; }
        public string CertificateNumber { get; set; }
        public string Assignor { get; set; }
        public string Assignee { get; set; }
        public string ApplicantName { get; set; }
        public string TNName { get; set; }
        public string RegistrationDate { get; set; }
        public string TNNameFirstLetter { get; set; }
    }

    public class DocumentDetail
    {
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
    }
    public class UploadDocuments
    {
        public DocumentDetail documentDetail { get; set; }
        public dynamic MetaData { get; set; }
    }
}

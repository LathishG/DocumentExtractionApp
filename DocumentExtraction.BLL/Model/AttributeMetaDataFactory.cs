using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentExtraction.BLL.Model
{
    public static class AttributeMetaDataFactory
    {
        public static dynamic GetAttributeDataModel(WorkItemTransaction transaction)
        {
            switch (transaction.TransactionType)
            {
                case "CN":
                case "GPSC":
                case "NC":
                case "PDS":
                case "PNC":
                case "PRS":
                    return new DocumentAttributeModel2 { 
                     AlphaRange=transaction.AlphaRange,                     
                     BusinessRegistrationDivision = transaction.BusinessRegistrationDivision,
                     DateReceived = transaction.DocumentDateReceived,
                     DocumentType = transaction.DocumentTitle,
                     FileNumber = transaction.FileNumber,
                     PartnershipName = transaction.PartnershipName,
                     Public = transaction.Public,
                     Quarter = transaction.Quarter,
                     Suffix = transaction.Suffix,
                     TransactionNumber = transaction.TransactionNumber,
                     Year = transaction.Year                     
                    };
                case "ANNBX":
                    return new DocumentAttributeModel3 { 
                         AnnualStatementType = transaction.AnnualStatementType,
                         BusinessRegistrationDivision = transaction.BusinessRegistrationDivision,
                         DateReceived = transaction.DocumentDateReceived,
                         FileNumber = transaction.FileNumber,
                         PartnershipName = transaction.PartnershipName,
                         Public = transaction.Public,
                         Suffix = transaction.Suffix,
                         TransactionNumber = transaction.TransactionNumber,
                         Year = transaction.Year
                    };
                case "TNAS":
                case "TNTR":
                    return new DocumentAttributeModel4 { 
                         Assignee = transaction.Assignee,
                         AssignmentDate = transaction.AssignmentDate,
                         Assignor = transaction.Assignor,
                         CertificateNumber = transaction.CertificateNumber,
                         Public = transaction.Public,
                         TradeName = transaction.TradeName,
                         TransactionNumber = transaction.TransactionNumber
                    };
                case "TNNA":
                    return new DocumentAttributeModel5 { 
                        ApplicantName = transaction.ApplicantName,
                        BusinessRegistrationDivision = transaction.BusinessRegistrationDivision,
                        CertificateNumber = transaction.CertificateNumber,
                        FileNumber = transaction.FileNumber,
                        Public = transaction.Public,
                        RegistrationDate = transaction.RegistrationDate,
                        Suffix = transaction.Suffix,
                        TMFirstName = transaction.TNNameFirstLetter,
                        TMName = transaction.TNName,
                        TransactionNumber = transaction.TransactionNumber
                    };
                default:
                    return new DocumentAttributeModel1 {                               
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

            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentExtraction.BLL.Model
{
    public class WorkItem
    {
        public string ID { get; set; }
        public string WorkItemID { get; set; }
        public string Name { get; set; }
        public int BatchId { get; set; }
        public ItemType WorkItemType { get; set; }
        public string HasPages { get; set; }
        public int DocumentCount { get; set; }
        public int TiffDocumentCount { get; set; }
        public int TiffDocumentWithAnnotationCount { get; set; }
        public int NonTiffDocumentCount { get; set; }
        public string HasAnnotation { get; set; }
        [DefaultValue('N')]
        public string HasError { get; set; }
        [DefaultValue('N')]
        public string IsProcessed { get; set; }
        public string Comments { get; set; }
        public DateTime ProcessedOn { get; set; }
        public string ProcessedBy { get; set; }
        public string Status { get; set; }
    }

    public enum ItemType
    {
        Document,
        BrandingImage
    }
}

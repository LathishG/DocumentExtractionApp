using CsvHelper;
using DocumentExtraction.BLL.Model;
using DocumentExtraction.BLL.Service;
using DocumentProcess.Common;
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
    class Program
    { 
        static async Task Main(string[] args)
        {
            DocumentUpload documentUpload = new DocumentUpload();
            await documentUpload.DoProcess();

            documentUpload.EndProcess();

        }        
    }
}

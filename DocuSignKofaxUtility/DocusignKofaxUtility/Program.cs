using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DocusignKofaxUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            DocumentUpload documentUpload = new DocumentUpload();
            documentUpload.DoProcess();
        }        
    }
}

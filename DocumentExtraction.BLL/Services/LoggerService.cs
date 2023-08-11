using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentExtraction.BLL.Service
{
    public class Logger
    {
        private static Logger loggerinstance=null;
        private static readonly object Instancelock = new object();
        private FileStream fStream = null;
        private StreamWriter writer = null;
        private static ReaderWriterLockSlim readerWriterLockSlim = new ReaderWriterLockSlim();
        private Logger() { }

        public static Logger Instance
        {
            get
            {
                lock(Instancelock)
                { 
                    if (loggerinstance == null)
                        loggerinstance = new Logger();

                    return loggerinstance;
                }
            }
        }

        public void CreateLogFile(string process)
        {
            try
            {
                if (!Directory.Exists(BaseSettings.LogfilePath))
                    Directory.CreateDirectory(BaseSettings.LogfilePath);

                fStream = new FileStream(BaseSettings.LogfilePath + "/" + BaseSettings.LogfileName+"_"+ process + "_" + DateTime.Now.ToString("ddMMyyyy"), FileMode.Append);
                writer = new StreamWriter(fStream);
            }
            catch (Exception ex) { throw ex; }
        }

        public void WriteLine(string message)
        {
            readerWriterLockSlim.EnterWriteLock();
            try
            {
                writer.WriteLine(message);
            }
            catch (Exception) { throw; }
            finally
            {
                readerWriterLockSlim.ExitWriteLock();
            }
        }

        public void CloseLogger()
        {
            if (writer != null) writer.Close();
            if (fStream != null) fStream.Close();
        }
    }
}

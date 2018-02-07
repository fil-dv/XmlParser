using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Semaphore.Infrastructure.WorkWithFiles
{
    public static class FileHandler
    {
        public static string ReadFile(string path)
        {
            string res = "";
            try
            {
                using (var streamReader = File.OpenText(path))
                {
                    res = streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("From Semaphore.Infrastructure.WorkWithFiles.ReadFile()" + ex.Message);
            }
            return res;
        }

        public static void WriteToFile(/*string path,*/ string text)
        {           
            try
            {
                //string createText = "Hello and Welcome" + Environment.NewLine;
                File.AppendAllText("log.txt", text + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.TargetSite + " " + ex.Message);
               // MessageBox.Show("From Semaphore.Infrastructure.WorkWithFiles.WriteToFile()" + ex.Message);
            }           
        }

    }
}

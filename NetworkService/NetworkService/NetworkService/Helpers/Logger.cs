using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Helpers
{
    public static class Logger
    {
        private static readonly object lockLog = new object();

        public static void AppendLog(string fileName, string text)
        {
            try
            {
                lock (lockLog)
                {
                    using (StreamWriter writer = File.AppendText(fileName))
                    {
                        writer.WriteLine($"{DateTime.Now} - {text}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while writing log file '{fileName}': {ex.Message}");
            }
        }
    }
}

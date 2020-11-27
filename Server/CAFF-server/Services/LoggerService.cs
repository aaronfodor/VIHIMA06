
using System;
using System.Collections.Generic;
using System.IO;

namespace CAFF_server
{
    public class LoggerService
    {
        private readonly string FileName = "Logs/log.txt";
        private readonly string ComponentName = "";

        public void Info(string message)
        {
            Log("Info", message);
        }

        public void Debug(string message)
        {
            Log("Debug", message);
        }

        public void Warning(string message)
        {
            Log("Warning", message);
        }

        public void Error(string message)
        {
            Log("Error", message);
        }

        private void Log(string level, string message)
        {
            string LogItem = $"{DateTime.UtcNow} [{ComponentName}][{level}]: {message}";

            Console.WriteLine(LogItem);
            using (StreamWriter sw = File.AppendText(FileName))
            {
                sw.WriteLine(LogItem);
            }
        }

    }
}

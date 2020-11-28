
using CAFF_server.Entities;
using System;
using System.Collections.Generic;
using System.IO;

namespace CAFF_server
{
    public interface ILoggerService
    {
        void Info(string message, string userid);
        void Error(string message, string userid);
        void Warning(string message, string userid);
        void Debug(string message, string userid);
    }
    public class LoggerService : ILoggerService
    {
        private DataContext _context;

        public LoggerService(DataContext context)
        {
            _context = context;
        }

        public void Info(string message, string userid)
        {
            Log("Info", message, userid);
        }

        public void Debug(string message, string userid)
        {
            Log("Debug", message, userid);
        }

        public void Warning(string message, string userid)
        {
            Log("Warning", message, userid);
        }

        public void Error(string message, string userid)
        {
            Log("Error", message, userid);
        }

        private void Log(string level, string message, string userid)
        {
            _context.Add(new Log() { Level = level, TimeStamp = DateTime.Now, Message = message, UserId = userid});
            _context.SaveChanges();
        }

    }
}

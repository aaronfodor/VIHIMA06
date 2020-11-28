using CAFF_server.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace CAFF_server.Services
{
    public interface ICaffService
    {
        int AddCaff(string path, string originalName, string userid);
        IEnumerable<CAFF> GetAllCaff();
        IEnumerable<CAFF> GetCaffSearch(string search);
        CAFF GetCaff(int id);
        void DeleteCaff(int id);
    }

    public class CaffService : ICaffService
    {
        private DataContext _context;
        public CaffService(DataContext context)
        {
            _context = context;
        }

        [DllImport("caff_parser.dll", EntryPoint = "parse", CallingConvention = CallingConvention.Cdecl)]
        static extern int parse(string path);

        public int AddCaff(string path, string originalName, string userid)
        {
            //parse("C:\\directoryname\\caffname.caff");
            try
            {
                var returnCode = parse(path);
                if (returnCode == 0)
                {
                    var filename = Path.GetFileName(path);
                    var user = _context.User.Find(userid);
                    var caff = new CAFF()
                    {
                        StoredFileName = filename.Substring(0, filename.Length - 5),
                        OriginalFileName = originalName,
                        Uploader = user,
                        UploadTimestamp = DateTime.Now,
                        Comments = new List<Comment>()
                    };
                    _context.CAFFs.Add(caff);
                    _context.SaveChanges();
                }
                return returnCode;
            }
            catch
            {
                throw;
            }
        }

        public void DeleteCaff(int id)
        {
            var caff = _context.CAFFs.Find(id);
            caff.Deleted = true;
            _context.CAFFs.Update(caff);
            _context.SaveChanges();
        }

        public IEnumerable<CAFF> GetAllCaff()
        {
            return _context.CAFFs.Where(x => !x.Deleted);
        }

        public CAFF GetCaff(int id)
        {
            var caff = _context.CAFFs.SingleOrDefault(x => x.Id == id);
            if (caff.Deleted) return null;
            else return caff;
        }

        public IEnumerable<CAFF> GetCaffSearch(string search)
        {
            return _context.CAFFs.Where(x => x.OriginalFileName.Contains(search) && !x.Deleted);
        }

    }
}

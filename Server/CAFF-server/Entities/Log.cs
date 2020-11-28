using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CAFF_server.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
    }
}

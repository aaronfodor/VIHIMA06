using System;
using System.Collections.Generic;

namespace CAFF_server.Entities
{
    public class CAFFDTO
    {
        public string Name { get; set; }
        public string UserId { get; set; }
        public string Creator { get; set; }
        public DateTime CreationTimestampUTC { get; set; }
        public string Caption { get; set; }
        public byte[] Preview { get; set; }
        public ICollection<string> Tags { get; set; }
    }
}

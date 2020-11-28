using CAFF_server.DTOs;
using CAFF_server.Entities;
using System;
using System.Collections.Generic;

namespace CAFF_server.DTOs
{
    public class CAFFDTO
    {
        public int Id { get; set; }
        public string Uploader { get; set; }
        public DateTime UploadTimestamp { get; set; }
        public string OriginalFileName { get; set; }
        public string StoredFileName { get; set; }
        public byte[] Preview { get; set; }
    }
}

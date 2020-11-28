using System;
using System.Collections.Generic;

namespace CAFF_server.Entities
{
    public class CAFF
    {
        public int Id { get; set; }
        public User Uploader { get; set; }
        public DateTime UploadTimestamp { get; set; }
        public List<Comment> Comments { get; set; }
        public string OriginalFileName { get; set; }
        public string StoredFileName { get; set; }
        public bool Deleted { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace CAFF_server.Entities
{
    public class CAFF
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UploaderId { get; set; }
        public DateTime UploadTimestampUTC { get; set; }
        public string Creator { get; set; }
        public DateTime CreationTimestampUTC { get; set; }
        public string Caption { get; set; }
        public byte[] Preview { get; set; }
        public byte[] Content { get; set; }
        public ICollection<string> Tags { get; set; }
        public ICollection<string> CommentIds { get; set; }
    }
}

using System;

namespace CAFF_server.DTOs
{
    public class CommentDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

using System;

namespace CAFF_server.Entities
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CAFFId { get; set; }
        public string Text { get; set; }
        public DateTime TimestampUTC { get; set; }
    }
}

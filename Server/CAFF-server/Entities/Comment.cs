using System;

namespace CAFF_server.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public User User { get; set; }
        public CAFF CAFF { get; set; }
        public string Text { get; set; }
        public bool Deleted { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

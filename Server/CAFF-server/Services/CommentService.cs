using CAFF_server.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CAFF_server.Services
{
    public interface ICommentService
    {
        Comment AddComment(Comment comment, int caffid, string userid);
        IEnumerable<Comment> GetAllComment(int id);
        void DeleteComment(int id);
    }

    public class CommentService : ICommentService
    {
        private DataContext _context;
        public CommentService(DataContext context)
        {
            _context = context;
        }

        public Comment AddComment(Comment comment, int caffid, string userid)
        {
            comment.CAFF = _context.CAFFs.Find(caffid);
            comment.User = _context.User.Find(userid);
            comment.Timestamp = DateTime.Now;
            _context.Comments.Add(comment);
            _context.SaveChanges();
            return comment;
        }

        public void DeleteComment(int id)
        {
            var comment = _context.Comments.Find(id);
            _context.Comments.Remove(comment);
            _context.SaveChanges();
        }

        public IEnumerable<Comment> GetAllComment(int id)
        {
            return _context.Comments.Include(x => x.CAFF).Include(x => x.User).Where(x => !x.Deleted && x.CAFF.Id == id);
        }

    }
}

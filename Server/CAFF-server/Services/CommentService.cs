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
        Comment AddComment(string text, int caffid, string userid);
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

        public Comment AddComment(string text, int caffid, string userid)
        {
            if (caffid != 0 && userid != null && text != null)
            {
                var comment = new Comment();
                comment.CAFF = _context.CAFFs.Find(caffid);
                comment.User = _context.User.Find(userid);
                comment.Timestamp = DateTime.Now;
                comment.Text = text;
                _context.Comments.Add(comment);
                _context.SaveChanges();
                return comment;
            }
            else
            return null;
        }

        public void DeleteComment(int id)
        {
            var comment = _context.Comments.Find(id);
            _context.Comments.Remove(comment);
            _context.SaveChanges();
        }

        public IEnumerable<Comment> GetAllComment(int id)
        {
            return _context.Comments.Include(x => x.CAFF).Include(x => x.User).Where(x => !x.Deleted && x.CAFF.Id == id).OrderByDescending(x => x.Id);
        }

    }
}

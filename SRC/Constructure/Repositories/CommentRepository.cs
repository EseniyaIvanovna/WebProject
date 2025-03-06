using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly List<Comment> _comments = new List<Comment>();
        public Task Create(Comment comment)
        {
            if (comment == null) throw new ArgumentNullException(nameof(comment));
            
            _comments.Add(comment);
            return Task.CompletedTask;
        }

        public Task<bool> Delete(int id)
        {
            var comment = _comments.FirstOrDefault(c => c.Id == id);

            if (comment == null) return Task.FromResult(false);
            
            _comments.Remove(comment);
            return Task.FromResult(true);
        }

        public Task<Comment> GetById(int id)
        {
            var comment = _comments.FirstOrDefault(c => c.Id == id);
            return Task.FromResult(comment);
        }

        public Task<IEnumerable<Comment>> GetByUserId(int userId)
        {
            var comments = _comments.Where(c => c.User.Id == userId).AsEnumerable();
            return Task.FromResult(comments);
        }

        public Task<bool> Update(Comment comment)
        {
            if (comment == null) throw new ArgumentNullException(nameof(comment));
           
            var existingComment = _comments.FirstOrDefault(c => c.Id == comment.Id);

            if (existingComment == null) return Task.FromResult(false);
            
            
            existingComment.Content = comment.Content;
            existingComment.User = comment.User;
            existingComment.Post = comment.Post;
            existingComment.CreatedAt = comment.CreatedAt;

            return Task.FromResult(true);
        }
    }
}

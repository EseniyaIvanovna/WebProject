using Domain;
using Microsoft.VisualBasic;
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
        public CommentRepository()
        {
            // тестовые данные 
            _comments.Add(new Comment { Id = 1, Content = "Great post!", UserId = 1, PostId = 1 });
            _comments.Add(new Comment { Id = 2, Content = "Nice work!", UserId = 2, PostId = 1 });
            _comments.Add(new Comment { Id = 3, Content = "Interesting read.", UserId = 1, PostId = 2 });
        }
        public Task<int> Create(Comment comment)
        {
            if (comment == null) 
                throw new ArgumentNullException(nameof(comment));
            var existingComment = _comments.FirstOrDefault(c => c.Id == comment.Id);
            if (existingComment != null)
            {
                throw new InvalidOperationException("A comment with the same ID already exists.");
            }
            _comments.Add(comment);
            return Task.FromResult(comment.Id); 
        }

        public Task<bool> Delete(int id)
        {
            var comment = _comments.FirstOrDefault(c => c.Id == id);

            if (comment == null) 
                return Task.FromResult(false);
            
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
            var comments = _comments.Where(c => c.UserId == userId);
            return Task.FromResult(comments);
        }

        public Task<bool> Update(Comment comment)
        {
            if (comment == null) 
                throw new ArgumentNullException(nameof(comment));
           
            var existingComment = _comments.FirstOrDefault(c => c.Id == comment.Id);

            if (existingComment == null) 
                return Task.FromResult(false);
            
            
            existingComment.Content = comment.Content;
            existingComment.UserId = comment.UserId;
            existingComment.PostId = comment.PostId;
            existingComment.CreatedAt = comment.CreatedAt;

            return Task.FromResult(true);
        }

        public Task<IEnumerable<Comment>> GetAll()
        {
            return Task.FromResult<IEnumerable<Comment>>(_comments.AsEnumerable());
        }
    }
}

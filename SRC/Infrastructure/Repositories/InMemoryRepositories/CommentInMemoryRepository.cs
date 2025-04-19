using Domain;
using Infrastructure.Repositories.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Repositories.InMemoryRepositories
{
    [ExcludeFromCodeCoverage]
    public class CommentInMemoryRepository : ICommentRepository
    {
        private readonly List<Comment> _comments = new List<Comment>();

        public CommentInMemoryRepository()
        {
            // тестовые данные 
            _comments.Add(new Comment { Id = 1, Content = "Great post!", UserId = 1, PostId = 1 });
            _comments.Add(new Comment { Id = 2, Content = "Nice work!", UserId = 2, PostId = 1 });
            _comments.Add(new Comment { Id = 3, Content = "Interesting read.", UserId = 1, PostId = 2 });
        }

        public Task<int> Create(Comment comment)
        {
            var existingComment = _comments.FirstOrDefault(m => m.Id == comment.Id);
            if (existingComment != null)
            {
                throw new InvalidOperationException("A comment with the same ID already exists.");
            }
            _comments.Add(comment);
            return Task.FromResult(comment.Id);
        }

        public Task<bool> Delete(int id)
        {
            var comment = _comments.FirstOrDefault(m => m.Id == id);
            if (comment == null)
            {
                return Task.FromResult(false);
            }

            _comments.Remove(comment);
            return Task.FromResult(true);
        }

        public Task<Comment?> GetById(int id)
        {
            var comment = _comments.FirstOrDefault(m => m.Id == id);
            return Task.FromResult(comment);
        }

        public Task<IEnumerable<Comment>> GetByUserId(int userId)
        {
            var comments = _comments.Where(m => m.UserId == userId);
            return Task.FromResult(comments);
        }

        public Task<bool> Update(Comment comment)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));

            var existingComment = _comments.FirstOrDefault(m => m.Id == comment.Id);
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
            return Task.FromResult(_comments.AsEnumerable());
        }

        public Task DeleteByPostId(int postId)
        {
            var commentsToDelete = _comments.Where(c => c.PostId == postId).ToList();
            foreach (var comment in commentsToDelete)
            {
                _comments.Remove(comment);
            }
            return Task.CompletedTask;
        }

        public Task DeleteByUserId(int userId)
        {
            var commentsToDelete = _comments.Where(c => c.UserId == userId).ToList();
            foreach (var comment in commentsToDelete)
            {
                _comments.Remove(comment);
            }
            return Task.CompletedTask;
        }

        Task ICommentRepository.DeleteByPostOwnerId(int userId)
        {
            throw new NotImplementedException();
        }
    }
}

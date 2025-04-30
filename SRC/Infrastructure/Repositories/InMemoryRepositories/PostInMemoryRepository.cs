using Domain;
using Infrastructure.Repositories.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Repositories.InMemoryRepositories
{
    [ExcludeFromCodeCoverage]
    public class PostInMemoryRepository : IPostRepository
    {
        private readonly List<Post> _posts;

        public PostInMemoryRepository()
        {
            _posts = new List<Post>
            {
                new Post { Id = 1, UserId = 1, Text = "This is the first post.", CreatedAt = DateTime.UtcNow },
                new Post { Id = 2, UserId = 2, Text = "This is the second post.", CreatedAt = DateTime.UtcNow },
                new Post { Id = 3, UserId = 1, Text = "This is the third post.", CreatedAt = DateTime.UtcNow }
            };
        }

        public Task<int> Create(Post post)
        {
            if (post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }

            var existingPost = _posts.FirstOrDefault(p => p.Id == post.Id);
            if (existingPost != null)
            {
                throw new InvalidOperationException("A post with the same ID already exists.");
            }

            _posts.Add(post);
            return Task.FromResult(post.Id);
        }

        public Task<bool> Delete(int id)
        {
            var post = _posts.FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return Task.FromResult(false);
            }

            _posts.Remove(post);
            return Task.FromResult(true);
        }

        public Task DeleteByUserId(int userId)
        {
            var postsToDelete = _posts.Where(p => p.UserId == userId).ToList();
            foreach (var post in postsToDelete)
            {
                _posts.Remove(post);
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Post>> GetAll()
        {
            return Task.FromResult(_posts.AsEnumerable());
        }

        public Task<Post?> GetById(int Id)
        {
            var post = _posts.First(p => p.Id == Id);
#pragma warning disable CS8619 // Допустимость значения NULL для ссылочных типов в значении не соответствует целевому типу.
            return Task.FromResult(post);
#pragma warning restore CS8619 // Допустимость значения NULL для ссылочных типов в значении не соответствует целевому типу.
        }

        public Task<bool> Update(Post post)
        {
            if (post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }

            var existingPost = _posts.FirstOrDefault(p => p.Id == post.Id);
            if (existingPost == null)
            {
                return Task.FromResult(false);
            }

            existingPost.Text = post.Text;
            existingPost.UserId = post.UserId;
            existingPost.CreatedAt = post.CreatedAt;

            return Task.FromResult(true);
        }

        Task IPostRepository.DeleteByUserId(int userId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Post>> IPostRepository.GetByUserId(int userId)
        {
            throw new NotImplementedException();
        }
    }
}

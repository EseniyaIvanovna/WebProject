using Domain;
using Infrastructure.Repositories.Interfaces;
using Npgsql;

namespace Infrastructure.Repositories.InMemoryRepositories
{
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

        public Task Delete(int id)
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
            var post = _posts.FirstOrDefault(p => p.Id == Id);
            return Task.FromResult(post);
        }

        public Task Update(Post post)
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

        Task IPostRepository.DeleteByUserId(int userId, NpgsqlTransaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}

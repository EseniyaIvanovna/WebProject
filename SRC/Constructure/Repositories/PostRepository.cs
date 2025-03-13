using Domain;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        public PostRepository()
        {
            // тестовые данные
            _posts.Add(new Post { Id = 1, Text = "This is the content of the first post.", UserId = 1 });
            _posts.Add(new Post { Id = 2, Text = "This is the content of the second post.", UserId = 2 });
            _posts.Add(new Post { Id = 3, Text = "This is the content of the third post.", UserId = 1 });
        }
        private readonly List<Post> _posts = new List<Post>();
        public Task<int> Create(Post post)
        {
            if(post == null) 
                throw new ArgumentNullException(nameof(post));
            var existingPost = _posts.FirstOrDefault(m => m.Id == post.Id);
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
                return Task.FromResult(false);            

            _posts.Remove(post);
            return Task.FromResult(true);
        }

        public Task<IEnumerable<Post>> GetAll()
        {
            return Task.FromResult(_posts.AsEnumerable());
        }

        public Task<Post> GetById(int Id)
        {
            var post = _posts.FirstOrDefault(p => p.Id == Id);
            return Task.FromResult(post);
        }

        public Task<bool> Update(Post post)
        {
            if(post == null) 
                throw new ArgumentNullException(nameof(post));
            
            var existingPost = _posts.FirstOrDefault(p => p.Id == post.Id);

            if (existingPost == null) 
                return Task.FromResult(false);
            
            
            existingPost.Text = post.Text;
            existingPost.UserId = post.UserId;
            existingPost.CreatedAt = post.CreatedAt;

            return Task.FromResult(true);
        }
    }
}

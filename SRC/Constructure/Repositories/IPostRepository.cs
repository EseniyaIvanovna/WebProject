using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IPostRepository
    {
        public Task<Post> GetById(int Id);
        public Task<IEnumerable<Post>> GetAll();
        public Task Create(Post post);
        public Task<bool> Update(Post post);
        public Task<bool> Delete(int id);
    }
}

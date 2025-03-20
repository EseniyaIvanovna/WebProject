using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        public Task<IEnumerable<Comment>> GetByUserId(int id);
        public Task<Comment> GetById(int Id);
        public Task<int> Create(Comment comment);
        public Task<bool> Update(Comment comment);
        public Task<bool> Delete(int id);
        public Task<IEnumerable<Comment>> GetAll();
        public Task DeleteByPostId(int postId);
        public Task DeleteByUserId(int userId);
    }
}

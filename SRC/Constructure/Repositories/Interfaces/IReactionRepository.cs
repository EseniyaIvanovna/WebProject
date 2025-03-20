using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IReactionRepository
    {
        public Task<Reaction> GetById(int Id);
        public Task<IEnumerable<Reaction>> GetByUserId(int Id);
        public Task<IEnumerable<Reaction>> GetByPostId(int Id);
        public Task<IEnumerable<Reaction>> GetAll();
        public Task<int> Create(Reaction reaction);
        public Task<bool> Update(Reaction reaction);
        public Task<bool> Delete(int id);
        public Task DeleteByPostId(int postId);
        public Task DeleteByUserId(int userId);
    }
}

using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IReactionRepository
    {
        public Task<Reaction> GetById(int Id);
        public Task<IEnumerable<Reaction>> GetByUserId(int Id);
        public Task<IEnumerable<Reaction>> GetByPosId(int Id);
        public Task Create(Reaction reaction);
        public Task<bool> Update(Reaction reaction);
        public Task<bool> Delete(int id);
    }
}

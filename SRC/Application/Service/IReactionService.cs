using Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public interface IReactionService
    {
        public Task<ReactionDto> GetById(int Id);
        public Task<IEnumerable<ReactionDto>> GetByUserId(int Id);
        public Task<IEnumerable<ReactionDto>> GetByPosId(int Id);
        public Task Create(ReactionDto reaction);
        public Task<bool> Update(ReactionDto reaction);
        public Task<bool> Delete(int id);
    }
}

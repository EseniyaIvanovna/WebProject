using Application.Dto;
using AutoMapper;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class ReactionService : IReactionService
    {
        private IReactionRepository reactRepository;
        private IMapper mapper;

        public ReactionService(IReactionRepository reactRepository, IMapper mapper)
        {
            this.reactRepository = reactRepository;
            this.mapper = mapper;
        }
        public Task Create(ReactionDto reaction)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ReactionDto> GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ReactionDto>> GetByPosId(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ReactionDto>> GetByUserId(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(ReactionDto reaction)
        {
            throw new NotImplementedException();
        }
    }
}

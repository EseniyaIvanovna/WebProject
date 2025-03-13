using Application.Dto;
using AutoMapper;
using Domain;
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

        public ReactionService(IReactionRepository ReactRepository, IMapper Mapper)
        {
            reactRepository = ReactRepository;
            mapper = Mapper;
        }

        public async Task<int> Create(ReactionDto reaction)
        {
            var mappedReaction = mapper.Map<Reaction>(reaction);
            int reactionId = await reactRepository.Create(mappedReaction);
            return reactionId;
        }

        public async Task<bool> Delete(int id)
        {
            var result = await reactRepository.Delete(id);
            return result;
        }

        public async Task<ReactionDto> GetById(int id)
        {
            var reaction = await reactRepository.GetById(id);
            return mapper.Map<ReactionDto>(reaction);
        }

        public async Task<IEnumerable<ReactionDto>> GetByPostId(int postId)
        {
            var reactions = await reactRepository.GetByPostId(postId);
            return mapper.Map<IEnumerable<ReactionDto>>(reactions);
        }

        public async Task<IEnumerable<ReactionDto>> GetByUserId(int userId)
        {
            var reactions = await reactRepository.GetByUserId(userId);
            return mapper.Map<IEnumerable<ReactionDto>>(reactions);
        }

        public async Task<bool> Update(ReactionDto reaction)
        {
            var existingReaction = await reactRepository.GetById(reaction.Id);
            if (existingReaction == null)
            {
                return false;
            }

            mapper.Map(reaction, existingReaction);
            await reactRepository.Update(existingReaction);
            return true;
        }

        public async Task<IEnumerable<ReactionDto>> GetAll()
        {
            var reactions = await reactRepository.GetAll();
            return mapper.Map<IEnumerable<ReactionDto>>(reactions);
        }
    }
}

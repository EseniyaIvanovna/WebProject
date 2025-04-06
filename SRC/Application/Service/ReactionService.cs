using Application.Exceptions;
using Application.Requests;
using Application.Responses;
using AutoMapper;
using Domain;
using Infrastructure.Repositories.Interfaces;

namespace Application.Service
{
    public class ReactionService : IReactionService
    {
        private readonly IReactionRepository _reactionRepository;
        private readonly IMapper _mapper;

        public ReactionService(IReactionRepository reactRepository, IMapper mapper)
        {
            _reactionRepository = reactRepository;
            _mapper = mapper;
        }

        public async Task<int> Create(CreateReactionRequest request)
        {
            if (await _reactionRepository.Exists(request.UserId, request.PostId))
                throw new ConflictApplicationException("User can have only one reaction per post");

            var reaction = _mapper.Map<Reaction>(request);
            return await _reactionRepository.Create(reaction);
        }

        public async Task Delete(int id)
        {
            var reaction = await _reactionRepository.GetById(id);
            if (reaction == null)
                throw new NotFoundApplicationException($"Reaction {id} not found");

            var result = await _reactionRepository.Delete(id);
            if(result == false)
            {
                throw new EntityDeleteException("Reaction", id.ToString());
            }
        }

        public async Task<ReactionResponse> GetById(int id)
        {
            var reaction = await _reactionRepository.GetById(id);
            if (reaction == null)
                throw new NotFoundApplicationException($"Reaction {id} not found");

            return _mapper.Map<ReactionResponse>(reaction);
        }

        public async Task<IEnumerable<ReactionResponse>> GetByPostId(int postId)
        {
            var reactions = await _reactionRepository.GetByPostId(postId);
            return _mapper.Map<IEnumerable<ReactionResponse>>(reactions);
        }

        public async Task<IEnumerable<ReactionResponse>> GetByUserId(int userId)
        {
            var reactions = await _reactionRepository.GetByUserId(userId);
            return _mapper.Map<IEnumerable<ReactionResponse>>(reactions);
        }

        public async Task Update(UpdateReactionRequest request)
        {
            var existingReaction = await _reactionRepository.GetById(request.Id);
            if (existingReaction == null)
                throw new NotFoundApplicationException($"Reaction {request.Id} not found");

            existingReaction.Type = request.Type;
            var result = await _reactionRepository.Update(existingReaction);

            if(result == false)
            {
                throw new EntityUpdateException("Reaction", request.Id.ToString());
            }
        }

        public async Task<IEnumerable<ReactionResponse>> GetAll()
        {
            var reactions = await _reactionRepository.GetAll();
            return _mapper.Map<IEnumerable<ReactionResponse>>(reactions);
        }
    }
}

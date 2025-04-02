using Application.Exceptions;
using Application.Exceptions.Application.Exceptions;
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
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;

        public ReactionService(IReactionRepository reactRepository, IUserRepository userRepository, IPostRepository postRepository, IMapper mapper)
        {
            _reactionRepository = reactRepository;
            _userRepository = userRepository;
            _postRepository = postRepository;
            _mapper = mapper;
        }

        public async Task<int> Create(CreateReactionRequest request)
        {
            if (await _reactionRepository.Exists(request.UserId, request.PostId))
                throw new ConflictApplicationException("User can have only one reaction per post");

            var reaction = _mapper.Map<Reaction>(request);
            return await _reactionRepository.Create(reaction);
        }

        public async Task<bool> Delete(int id)
        {
            var reaction = await _reactionRepository.GetById(id);
            if (reaction == null)
                throw new NotFoundApplicationException($"Reaction {id} not found");

            return await _reactionRepository.Delete(id);
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

        public async Task<bool> Update(UpdateReactionRequest request)
        {
            var existingReaction = await _reactionRepository.GetById(request.Id);
            if (existingReaction == null)
                throw new NotFoundApplicationException($"Reaction {request.Id} not found");

            existingReaction.Type = request.Type;
            return await _reactionRepository.Update(existingReaction);
        }

        public async Task<IEnumerable<ReactionResponse>> GetAll()
        {
            var reactions = await _reactionRepository.GetAll();
            return _mapper.Map<IEnumerable<ReactionResponse>>(reactions);
        }
    }
}

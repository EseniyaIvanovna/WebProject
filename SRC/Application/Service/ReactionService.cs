using Application.Dto;
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

        public async Task<int> Create(ReactionDto reaction)
        {
            if (reaction == null)
            {
                throw new ArgumentNullException(nameof(reaction), "Reaction cannot be null.");
            }

            var user = await _userRepository.GetById(reaction.UserId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var post = await _postRepository.GetById(reaction.PostId);
            if (post == null)
            {
                throw new InvalidOperationException("Post not found.");
            }

            var mappedReaction = _mapper.Map<Reaction>(reaction);
            return await _reactionRepository.Create(mappedReaction);
        }

        public async Task<bool> Delete(int id)
        {
            var reaction = await _reactionRepository.GetById(id);
            if (reaction == null)
            {
                throw new InvalidOperationException("Reaction not found.");
            }

            return await _reactionRepository.Delete(id);
        }

        public async Task<ReactionDto> GetById(int id)
        {
            var reaction = await _reactionRepository.GetById(id);
            return _mapper.Map<ReactionDto>(reaction);
        }

        public async Task<IEnumerable<ReactionDto>> GetByPostId(int postId)
        {
            var reactions = await _reactionRepository.GetByPostId(postId);
            return _mapper.Map<IEnumerable<ReactionDto>>(reactions);
        }

        public async Task<IEnumerable<ReactionDto>> GetByUserId(int userId)
        {
            var reactions = await _reactionRepository.GetByUserId(userId);
            return _mapper.Map<IEnumerable<ReactionDto>>(reactions);
        }

        public async Task<bool> Update(ReactionDto reaction)
        {
            if (reaction == null)
            {
                throw new ArgumentNullException(nameof(reaction), "Reaction cannot be null.");
            }

            var existingReaction = await _reactionRepository.GetById(reaction.Id);
            if (existingReaction == null)
            {
                throw new InvalidOperationException("Reaction not found.");
            }

            _mapper.Map(reaction, existingReaction);
            return await _reactionRepository.Update(existingReaction);
        }

        public async Task<IEnumerable<ReactionDto>> GetAll()
        {
            var reactions = await _reactionRepository.GetAll();
            return _mapper.Map<IEnumerable<ReactionDto>>(reactions);
        }
    }
}

using Application.Exceptions;
using Application.Requests;
using Application.Responses;
using Application.Service.Interfaces;
using AutoMapper;
using Domain;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class ReactionService : IReactionService
    {
        private readonly IReactionRepository _reactionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ReactionService> _logger;

        public ReactionService(IReactionRepository reactRepository, IMapper mapper, ILogger<ReactionService> logger)
        {
            _reactionRepository = reactRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Create(CreateReactionRequest request)
        {
            if (await _reactionRepository.Exists(request.UserId, request.PostId))
                throw new ConflictApplicationException("User can have only one reaction per post");

            var reaction = _mapper.Map<Reaction>(request);
            var reactionId = await _reactionRepository.Create(reaction);

            _logger.LogInformation(
                "Reaction created with id {Id} of type {Type} by user {UserId} to post {PostId}",
                reactionId,
                request.Type,
                request.UserId,
                request.PostId);

            return reactionId;
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

            _logger.LogInformation(
                "Reaction successfully deleted with id {Id}",
                id);
        }

        public async Task<ReactionResponse> GetById(int id)
        {
            var reaction = await _reactionRepository.GetById(id);
            if (reaction == null)
                throw new NotFoundApplicationException($"Reaction {id} not found");

            var response = _mapper.Map<ReactionResponse>(reaction);

            _logger.LogInformation(
                "Reaction retrieved with id {Id}",
                id);

            return response;
        }

        public async Task<IEnumerable<ReactionResponse>> GetByPostId(int postId)
        {
            var reactions = await _reactionRepository.GetByPostId(postId);
            var responses = _mapper.Map<IEnumerable<ReactionResponse>>(reactions);

            _logger.LogInformation(
                "Retrieved {Count} reactions for post {PostId}",
                responses.Count(),
                postId);

            return responses;
        }

        public async Task<IEnumerable<ReactionResponse>> GetByUserId(int userId)
        {
            var reactions = await _reactionRepository.GetByUserId(userId);
            var responses = _mapper.Map<IEnumerable<ReactionResponse>>(reactions);

            _logger.LogInformation(
                "Retrieved {Count} reactions for user {UserId}",
                responses.Count(),
                userId);

            return responses;
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

            _logger.LogInformation(
                "Reaction updated with id {Id} to type {Type}",
                request.Id,
                request.Type);
        }

        public async Task<IEnumerable<ReactionResponse>> GetAll()
        {
            var reactions = await _reactionRepository.GetAll();
            var responses = _mapper.Map<IEnumerable<ReactionResponse>>(reactions);

            _logger.LogInformation(
                "Retrieved {Count} reactions in total",
                responses.Count());

            return responses;
        }
    }
}

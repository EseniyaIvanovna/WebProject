using Application.Exceptions;
using Application.Requests;
using Application.Responses;
using Application.Service.Interfaces;
using AutoMapper;
using Domain;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class InteractionService : IInteractionService
    {
        private readonly IInteractionRepository _interactionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<InteractionService> _logger;

        public InteractionService(IInteractionRepository interactionRepository, IMapper mapper, ILogger<InteractionService> logger)
        {
            _interactionRepository = interactionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Create(CreateInteractionRequest request)
        {
            if (await _interactionRepository.ExistsBetweenUsers(request.User1Id, request.User2Id))
                throw new ConflictApplicationException("Interaction between these users already exists");

            var interaction = _mapper.Map<Interaction>(request);
            var interactionId = await _interactionRepository.Create(interaction);

            _logger.LogInformation(
                "Interaction created with id {Id} between users {User1Id} and {User2Id}",
                interactionId,
                request.User1Id,
                request.User2Id);

            return interactionId;
        }

        public async Task Delete(int id)
        {
            var interaction = await _interactionRepository.GetById(id);
            if (interaction == null)
                throw new NotFoundApplicationException($"Interaction {id} not found");

            var result = await _interactionRepository.Delete(id);
            if(result == false)
            {
                throw new EntityDeleteException("Interaction", id.ToString());
            }

            _logger.LogInformation(
                "Interaction successfully deleted with id {Id}",
                id);
        }

        public async Task<InteractionResponse> GetById(int id)
        {
            var interaction = await _interactionRepository.GetById(id);
            if (interaction == null)
                throw new NotFoundApplicationException($"Interaction {id} not found");

            var response = _mapper.Map<InteractionResponse>(interaction);

            _logger.LogInformation(
                "Interaction retrieved with id {Id}",
                id);

            return response;
        }
        
        public async Task<IEnumerable<InteractionResponse>> GetAll()
        {
            var interactions = await _interactionRepository.GetAll();
            var responses = _mapper.Map<IEnumerable<InteractionResponse>>(interactions);

            _logger.LogInformation(
                "Retrieved {Count} interactions in total",
                responses.Count());

            return responses;
        }
       
        public async Task<IEnumerable<InteractionResponse>> GetByStatus(Status status)
        {
            var interactions = await _interactionRepository.GetByStatus(status);
            var responses = _mapper.Map<IEnumerable<InteractionResponse>>(interactions);

            _logger.LogInformation(
                "Retrieved {Count} interactions with status {Status}",
                responses.Count(),
                status);

            return responses;
        }
       
        public async Task Update(UpdateInteractionRequest request)
        {
            var existingInteraction = await _interactionRepository.GetById(request.Id);
            if (existingInteraction == null)
                throw new NotFoundApplicationException($"Interaction {request.Id} not found");

            existingInteraction.Status = request.Status;
            var result = await _interactionRepository.Update(existingInteraction);

            if(result == false)
            {
                throw new EntityUpdateException("Interaction", request.Id.ToString());
            }

            _logger.LogInformation(
                "Interaction updated with id {Id} to status {Status}",
                request.Id,
                request.Status);
        }
    }
}

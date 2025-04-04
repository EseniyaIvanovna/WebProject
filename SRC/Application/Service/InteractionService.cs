using Application.Exceptions;
using Application.Requests;
using Application.Responses;
using AutoMapper;
using Domain;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace Application.Service
{
    public class InteractionService : IInteractionService
    {
        private readonly IInteractionRepository _interactionRepository;
        private readonly IMapper _mapper;

        public InteractionService(IInteractionRepository interactionRepository, IMapper mapper)
        {
            _interactionRepository = interactionRepository;
            _mapper = mapper;
        }

        public async Task<int> Create(CreateInteractionRequest request)
        {
            if (await _interactionRepository.ExistsBetweenUsers(request.User1Id, request.User2Id))
                throw new ConflictApplicationException("Interaction between these users already exists");

            var interaction = _mapper.Map<Interaction>(request);
            return await _interactionRepository.Create(interaction);
        }

        public async Task Delete(int id)
        {
            var interaction = await _interactionRepository.GetById(id);
            if (interaction == null)
                throw new NotFoundApplicationException($"Interaction {id} not found");

            await _interactionRepository.Delete(id);
        }

        public async Task<InteractionResponse> GetById(int id)
        {
            var interaction = await _interactionRepository.GetById(id);
            if (interaction == null)
                throw new NotFoundApplicationException($"Interaction {id} not found");
            return _mapper.Map<InteractionResponse>(interaction);
        }
        
        public async Task<IEnumerable<InteractionResponse>> GetAll()
        {
            var interactions = await _interactionRepository.GetAll();
            return _mapper.Map<IEnumerable<InteractionResponse>>(interactions);
        }
       
        public async Task<IEnumerable<InteractionResponse>> GetByStatus(Status status)
        {
            var interactions = await _interactionRepository.GetByStatus(status);
            return _mapper.Map<IEnumerable<InteractionResponse>>(interactions);
        }
       
        public async Task Update(UpdateInteractionRequest request)
        {
            var existingInteraction = await _interactionRepository.GetById(request.Id);
            if (existingInteraction == null)
                throw new NotFoundApplicationException($"Interaction {request.Id} not found");

            existingInteraction.Status = request.Status;
            await _interactionRepository.Update(existingInteraction);
        }
    }
}

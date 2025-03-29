using Application.Exceptions;
using Application.Exceptions.Application.Exceptions;
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
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public InteractionService(IInteractionRepository interactionRepository, IUserRepository userRepository, IMapper mapper)
        {
            _interactionRepository = interactionRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<int> Create(CreateInteractionRequest request)
        {
            var user1 = await _userRepository.GetById(request.User1Id);
            if (user1 == null)
                throw new NotFoundApplicationException($"User {request.User1Id} not found");

            var user2 = await _userRepository.GetById(request.User2Id);
            if (user2 == null)
                throw new NotFoundApplicationException($"User {request.User2Id} not found");

            if (await _interactionRepository.ExistsBetweenUsers(request.User1Id, request.User2Id))
                throw new ConflictApplicationException("Interaction between these users already exists");

            var inreraction = new Interaction()
            {
                User1Id = request.User1Id,
                User2Id=request.User2Id,
                Status=request.Status
            }; 
            return await _interactionRepository.Create(inreraction);
        }

        public async Task<bool> Delete(int id)
        {
            var interaction = await _interactionRepository.GetById(id);
            if (interaction == null)
                throw new NotFoundApplicationException($"Interaction {id} not found");

            return await _interactionRepository.Delete(id);
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
       
        public async Task<bool> Update(UpdateInteractionRequest request)
        {
            var existingInteraction = await _interactionRepository.GetById(request.Id);
            if (existingInteraction == null)
                throw new NotFoundApplicationException($"Interaction {request.Id} not found");

            existingInteraction.Status = request.Status;
            return await _interactionRepository.Update(existingInteraction);
        }
    }
}

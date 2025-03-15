using Application.Dto;
using AutoMapper;
using Domain;
using Domain.Enums;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<int> Create(InteractionDto interaction)
        {
            if (interaction == null)
            {
                throw new ArgumentNullException(nameof(interaction), "Interaction cannot be null.");
            }

            var user1 = await _userRepository.GetById(interaction.User1Id);
            if (user1 == null)
            {
                throw new InvalidOperationException("User1 not found.");
            }

            var user2 = await _userRepository.GetById(interaction.User2Id);
            if (user2 == null)
            {
                throw new InvalidOperationException("User2 not found.");
            }

            var mappedInteraction = _mapper.Map<Interaction>(interaction);
            return await _interactionRepository.Create(mappedInteraction);
        }

        public async Task<bool> Delete(int id)
        {
            var interaction = await _interactionRepository.GetById(id);
            if (interaction == null)
            {
                throw new InvalidOperationException("Interaction not found.");
            }

            return await _interactionRepository.Delete(id);
        }

        public async Task<InteractionDto> GetById(int id)
        {
            var interaction = await _interactionRepository.GetById(id);
            return _mapper.Map<InteractionDto>(interaction);
        }
        
        public async Task<IEnumerable<InteractionDto>> GetAll()
        {
            var interactions = await _interactionRepository.GetAll();
            return _mapper.Map<IEnumerable<InteractionDto>>(interactions);
        }
       
        public async Task<IEnumerable<InteractionDto>> GetByStatus(Status status)
        {
            var interactions = await _interactionRepository.GetByStatus(status);
            return _mapper.Map<IEnumerable<InteractionDto>>(interactions);
        }
       
        public async Task<bool> Update(InteractionDto interaction)
        {
            if (interaction == null)
            {
                throw new ArgumentNullException(nameof(interaction), "Interaction cannot be null.");
            }

            var existingInteraction = await _interactionRepository.GetById(interaction.Id);
            if (existingInteraction == null)
            {
                throw new InvalidOperationException("Interaction not found.");
            }

            _mapper.Map(interaction, existingInteraction);
            return await _interactionRepository.Update(existingInteraction);
        }
    }
}

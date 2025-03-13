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
        private IInteractionRepository interactionRepository;
        private IMapper mapper;

        public InteractionService(IInteractionRepository InteractionRepository, IMapper Mapper)
        {
            interactionRepository = InteractionRepository;
            mapper = Mapper;
        }
        public async Task<int> Create(InteractionDto interaction)
        {
            var mappedInteraction = mapper.Map<Interaction>(interaction);
            int interactionId = await interactionRepository.Create(mappedInteraction);
            return interactionId;
        }

        public async Task<bool> Delete(int id)
        {
            var result = await interactionRepository.Delete(id);
            return result;
        }

        public async Task<InteractionDto> GetById(int id)
        {
            var interaction = await interactionRepository.GetById(id);
            return mapper.Map<InteractionDto>(interaction);
        }
        public async Task<IEnumerable<InteractionDto>> GetAll()
        {
            var interactions = await interactionRepository.GetAll();
            return mapper.Map<IEnumerable<InteractionDto>>(interactions);
        }
        public async Task<IEnumerable<InteractionDto>> GetByStatus(Status status)
        {
            var interactions = await interactionRepository.GetByStatus(status);
            return mapper.Map<IEnumerable<InteractionDto>>(interactions);
        }
        public async Task<bool> Update(InteractionDto interaction)
        {
            var existingInteraction = await interactionRepository.GetById(interaction.Id);
            if (existingInteraction == null)
            {
                return false;
            }

            mapper.Map(interaction, existingInteraction);
            await interactionRepository.Update(existingInteraction);
            return true;
        }
    }
}

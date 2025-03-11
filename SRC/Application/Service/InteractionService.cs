using Application.Dto;
using AutoMapper;
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

        public InteractionService(IInteractionRepository interactionRepository, IMapper mapper)
        {
            this.interactionRepository = interactionRepository;
            this.mapper = mapper;
        }
        public Task Create(InteractionDto interaction)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<InteractionDto> GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InteractionDto>> GetByStatus(Status status)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(InteractionDto interaction)
        {
            throw new NotImplementedException();
        }
    }
}

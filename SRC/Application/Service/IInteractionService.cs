using Application.Dto;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public interface IInteractionService
    {
        public Task<InteractionDto> GetById(int Id);
        public Task<IEnumerable<InteractionDto>> GetByStatus(Status status);
        public Task Create(InteractionDto interaction);
        public Task<bool> Update(InteractionDto interaction);
        public Task<bool> Delete(int id);
    }
}

using Application.Dto;
using Domain.Enums;

namespace Application.Service
{
    public interface IInteractionService
    {
        public Task<InteractionDto> GetById(int Id);
        public Task<IEnumerable<InteractionDto>> GetByStatus(Status status);
        public Task<IEnumerable<InteractionDto>> GetAll();
        public Task<int> Create(InteractionDto interaction);
        public Task<bool> Update(InteractionDto interaction);
        public Task<bool> Delete(int id);
    }
}

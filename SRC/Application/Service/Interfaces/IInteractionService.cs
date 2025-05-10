using Application.Requests;
using Application.Responses;
using Domain.Enums;

namespace Application.Service.Interfaces
{
    public interface IInteractionService
    {
        public Task<InteractionResponse> GetById(int Id);
        public Task<IEnumerable<InteractionResponse>> GetByStatus(Status status);
        public Task<IEnumerable<InteractionResponse>> GetAll();
        public Task<int> Create(CreateInteractionRequest request);
        public Task Update(UpdateInteractionRequest request);
        public Task Delete(int id);
    }
}

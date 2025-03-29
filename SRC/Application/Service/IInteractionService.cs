using Application.Requests;
using Application.Responses;
using Domain.Enums;

namespace Application.Service
{
    public interface IInteractionService
    {
        public Task<InteractionResponse> GetById(int Id);
        public Task<IEnumerable<InteractionResponse>> GetByStatus(Status status);
        public Task<IEnumerable<InteractionResponse>> GetAll();
        public Task<int> Create(CreateInteractionRequest request);
        public Task<bool> Update(UpdateInteractionRequest request);
        public Task<bool> Delete(int id);
    }
}

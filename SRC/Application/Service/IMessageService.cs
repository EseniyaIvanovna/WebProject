using Application.Requests;
using Application.Responses;

namespace Application.Service
{
    public interface IMessageService
    {
        public Task<MessageResponse> GetById(int Id);
        public Task<IEnumerable<MessageResponse>> GetByUserId(int Id);
        public Task<int> Create(CreateMessageRequest request);
        public Task Update(UpdateMessageRequest request);
        public Task Delete(int id);
        public Task<IEnumerable<MessageResponse>> GetAll();
    }
}

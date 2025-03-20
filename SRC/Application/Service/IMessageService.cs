using Application.Dto;

namespace Application.Service
{
    public interface IMessageService
    {
        public Task<MessageDto> GetById(int Id);
        public Task<IEnumerable<MessageDto>> GetByUserId(int Id);
        public Task<int> Create(MessageDto message);
        public Task<bool> Update(MessageDto message);
        public Task<bool> Delete(int id);
        public Task<IEnumerable<MessageDto>> GetAll();
    }
}

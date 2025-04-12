using Domain;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IMessageRepository
    {
        public Task<Message> GetById(int Id);
        public Task<IEnumerable<Message>> GetByUserId(int Id);
        public Task<int> Create(Message message);
        public Task<bool> Update(Message message);
        public Task<bool> Delete(int id);
        public Task DeleteMessagesByUser(int userId);
        public Task<IEnumerable<Message>> GetAll();
    }
}

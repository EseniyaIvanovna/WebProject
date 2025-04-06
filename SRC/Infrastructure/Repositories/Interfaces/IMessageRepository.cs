using Domain;
using Npgsql;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IMessageRepository
    {
        public Task<Message?> GetById(int Id);
        public Task<IEnumerable<Message>> GetByUserId(int Id);
        public Task<int> Create(Message message);
        public Task Update(Message message);
        public Task Delete(int id);
        public Task DeleteMessagesByUser(int userId, NpgsqlTransaction transaction);
        public Task<IEnumerable<Message>> GetAll();
    }
}

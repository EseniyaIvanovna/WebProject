using Domain;
using Domain.Enums;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IInteractionRepository
    {
        public Task<Interaction?> GetById(int Id);
        public Task<IEnumerable<Interaction>> GetByStatus(Status status);
        public Task<int> Create(Interaction interaction);
        public Task Update(Interaction interaction);
        public Task Delete(int id);
        public Task<IEnumerable<Interaction>> GetAll();
        public Task<bool> ExistsBetweenUsers(int user1Id, int user2Id);
        public Task DeleteByUserId(int userId);
    }
}

using Domain;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IReactionRepository
    {
        public Task<Reaction?> GetById(int Id);
        public Task<IEnumerable<Reaction>> GetByUserId(int Id);
        public Task<IEnumerable<Reaction>> GetByPostId(int Id);
        public Task<IEnumerable<Reaction>> GetAll();
        public Task<int> Create(Reaction reaction);
        public Task<bool> Update(Reaction reaction);
        public Task<bool> Delete(int id);
        public Task DeleteByPostId(int postId);
        public Task DeleteByUserId(int userId);
        public Task<bool> Exists(int userId, int postId);
        public Task DeleteByPostOwnerId(int userId);

    }
}

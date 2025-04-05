using Domain;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IPostRepository
    {
        public Task<Post?> GetById(int Id);
        public Task<IEnumerable<Post>> GetAll();
        public Task<int> Create(Post post);
        public Task Update(Post post);
        public Task Delete(int id);
        public Task DeleteByUserId(int userId);
    }
}

using Application.Requests;
using Application.Responses;

namespace Application.Service.Interfaces
{
    public interface IPostService
    {
        public Task<PostResponse?> GetById(int Id);
        public Task<IEnumerable<PostResponse>> GetAll();
        public Task<IEnumerable<PostResponse>> GetByUserId(int id);
        public Task<int> Create(CreatePostRequest request);
        public Task Update(UpdatePostRequest request);
        public Task Delete(int id);
    }
}

using Application.Requests;
using Application.Responses;

namespace Application.Service
{
    public interface IPostService
    {
        public Task<PostResponse> GetById(int Id);
        public Task<IEnumerable<PostResponse>> GetAll();
        public Task<int> Create(CreatePostRequest request);
        public Task<bool> Update(UpdatePostRequest request);
        public Task<bool> Delete(int id);
    }
}

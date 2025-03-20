using Application.Dto;

namespace Application.Service
{
    public interface IPostService
    {
        public Task<PostDto> GetById(int Id);
        public Task<IEnumerable<PostDto>> GetAll();
        public Task<int> Create(PostDto post);
        public Task<bool> Update(PostDto post);
        public Task<bool> Delete(int id);
    }
}

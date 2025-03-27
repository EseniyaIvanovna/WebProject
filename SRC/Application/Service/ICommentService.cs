using Application.Dto;

namespace Application.Service
{
    public interface ICommentService
    {
        public Task<IEnumerable<CommentDto>> GetByUserId(int id);
        public Task<CommentDto> GetById(int id);
        public Task<int> Create(CommentDto comment);
        public Task<bool> Update(CommentDto comment);
        public Task<bool> Delete(int id);
        public Task<IEnumerable<CommentDto>> GetAll();
    }
}

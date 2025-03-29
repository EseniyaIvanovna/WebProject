using Application.Requests;
using Application.Responses;

namespace Application.Service
{
    public interface ICommentService
    {
        public Task<IEnumerable<CommentResponse>> GetByUserId(int id);
        public Task<CommentResponse> GetById(int id);
        public Task<int> Create(CreateCommentRequest request);
        public Task<bool> Update(UpdateCommentRequest request);
        public Task<bool> Delete(int id);
        public Task<IEnumerable<CommentResponse>> GetAll();
    }
}

using Application.Requests;
using Application.Responses;

namespace Application.Service
{
    public interface IReactionService
    {
        public Task<ReactionResponse> GetById(int Id);
        public Task<IEnumerable<ReactionResponse>> GetByUserId(int Id);
        public Task<IEnumerable<ReactionResponse>> GetByPostId(int Id);
        public Task<int> Create(CreateReactionRequest request);
        public Task<bool> Update(UpdateReactionRequest request);
        public Task<bool> Delete(int id);
        public Task<IEnumerable<ReactionResponse>> GetAll();
    }
}

using Application.Dto;

namespace Application.Service
{
    public interface IReactionService
    {
        public Task<ReactionDto> GetById(int Id);
        public Task<IEnumerable<ReactionDto>> GetByUserId(int Id);
        public Task<IEnumerable<ReactionDto>> GetByPostId(int Id);
        public Task<int> Create(ReactionDto reaction);
        public Task<bool> Update(ReactionDto reaction);
        public Task<bool> Delete(int id);
        public Task<IEnumerable<ReactionDto>> GetAll();
    }
}

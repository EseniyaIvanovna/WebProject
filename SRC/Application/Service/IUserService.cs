using Application.Dto;

namespace Application.Service
{
    public interface IUserService
    {
        public Task<int> Add(UserDto user);
        public Task<bool> Delete(int id);
        public Task<UserDto> GetById(int Id);
        public Task<IEnumerable<UserDto>> GetAll();
        public Task<bool> Update(UserDto user);
    }
}

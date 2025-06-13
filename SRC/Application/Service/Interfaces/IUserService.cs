using Application.Requests;
using Application.Responses;

namespace Application.Service.Interfaces
{
    public interface IUserService
    {
        public Task<int> Add(CreateUserRequest user);
        public Task Delete(int id);
        public Task<UserResponse> GetById(int Id);
        public Task<IEnumerable<UserResponse>> GetAll();
        public Task Update(UpdateUserRequest user);
    }
}

using Application.Requests;
using Application.Responses;

namespace Application.Service
{
    public interface IUserService
    {
        public Task<int> Add(CreateUserRequest user);
        public Task<bool> Delete(int id);
        public Task<UserResponse> GetById(int Id);
        public Task<IEnumerable<UserResponse>> GetAll();
        public Task<bool> Update(UpdateUserRequest user);
    }
}

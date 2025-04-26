using Application.Requests;
using Application.Responses;

namespace Application.Service
{
    public interface IAuthService
    {
        Task<int> Register(RegistrationRequest request);
        Task<LoginResponse> Login(LoginRequest request);
    }
}

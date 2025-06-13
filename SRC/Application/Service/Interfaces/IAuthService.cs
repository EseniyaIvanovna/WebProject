using Application.Requests;
using System.Security.Claims;

namespace Application.Service.Interfaces
{
    public interface IAuthService
    {
        Task<ClaimsPrincipal> Register(RegistrationRequest request);
        Task<ClaimsPrincipal> Login(LoginRequest request);
    }
}

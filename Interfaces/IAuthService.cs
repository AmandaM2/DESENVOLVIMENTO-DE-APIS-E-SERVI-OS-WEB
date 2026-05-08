using Api.DTOs;

namespace Api.Interfaces
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginDTO dto);
    }
}

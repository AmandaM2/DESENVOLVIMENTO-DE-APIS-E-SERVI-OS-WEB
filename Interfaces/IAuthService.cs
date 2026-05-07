using api.DTOs;

namespace api.Interfaces
{
    public interface IAuthService
    {

        string GerarToken(LoginDTO loginDto);

    }
}

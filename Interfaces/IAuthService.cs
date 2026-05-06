using api.DTOs;

namespace api.Interfaces
{
    public interface IAuthService
    {

        string GerarToken(LoginDTO loginDto);

        // Valida se as credenciais estão corretas
        bool ValidarUsuario(LoginDTO loginDto);
    }
}
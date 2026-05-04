using api.DTOs;

namespace api.Interfaces
{
    public interface IAuthService
    {
        [cite_start]// Gera o token JWT para o login [cite: 11, 60]
        string GerarToken(LoginDTO loginDto);

        // Valida se as credenciais estão corretas
        bool ValidarUsuario(LoginDTO loginDto);
    }
}
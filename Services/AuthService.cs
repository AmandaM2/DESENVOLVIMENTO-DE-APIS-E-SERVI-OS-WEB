using Api.DTOs;
using Api.Interfaces;

namespace Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IContaRepository _contaRepository;

        public AuthService(IContaRepository contaRepository)
        {
            _contaRepository = contaRepository;
        }

        public string GerarToken(LoginDTO loginDto)
        {
            return "token-fake";
        }

        public bool ValidarUsuario(LoginDTO loginDto)
        {
            return true;
        }

        public async Task<string?> LoginAsync(LoginDTO dto)
        {
            var conta = await _contaRepository.GetByTitularAsync(dto.Usuario);

            if (conta == null || conta.Senha != dto.Senha)
            {
                return null;
            }

            return GerarToken(dto);
        }
    }
}

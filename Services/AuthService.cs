using Api.DTOs;
using Api.Interfaces;

namespace Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IContaRepository _contaRepository;
        private readonly ITokenService _tokenService; // Adicionamos o serviço de token

        // Injetamos ambos no construtor
        public AuthService(IContaRepository contaRepository, ITokenService tokenService)
        {
            _contaRepository = contaRepository;
            _tokenService = tokenService;
        }

        public async Task<string?> LoginAsync(LoginDTO dto)
        {
            // 1. Busca a conta pelo titular (ajustado para dto.Usuario)
            var conta = await _contaRepository.GetByTitularAsync(dto.Usuario);

            // 2. Valida se a conta existe e se a senha bate
            if (conta == null || conta.Senha != dto.Senha)
            {
                return null; // Retorna nulo se as credenciais forem inválidas
            }

            // 3. AGORA SIM: Usa o TokenService real para gerar o JWT
            return _tokenService.GerarToken(conta);
        }

        // Você pode remover ou manter esses métodos abaixo como privados,
        // mas o LoginAsync acima é o que realmente faz o trabalho agora.
        public string GerarToken(LoginDTO loginDto) => throw new NotImplementedException("Use o ITokenService");
        public bool ValidarUsuario(LoginDTO loginDto) => throw new NotImplementedException();
    }
}

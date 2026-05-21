using Api.DTOs;
using Api.Interfaces;
using System;
using System.Threading.Tasks;

namespace Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IContaRepository _contaRepository;
        private readonly ITokenService _tokenService;

        // Construtor injetando os repositórios e serviços necessários
        public AuthService(IContaRepository contaRepository, ITokenService tokenService)
        {
            _contaRepository = contaRepository;
            _tokenService = tokenService;
        }

        public async Task<string?> LoginAsync(LoginDTO dto)
        {
            // 1. Busca a conta pelo nome do usuário (o repositório já faz o Include do Usuário)
            var conta = await _contaRepository.GetByTitularAsync(dto.Usuario);

            // 2. CORREÇÃO (Linha 24): A propriedade 'Senha' mudou da 'Conta' para o 'Usuario'
            if (conta == null || conta.Usuario == null || conta.Usuario.Senha != dto.Senha)
            {
                return null; // Credenciais inválidas
            }

            // 3. CORREÇÃO (Linha 30): O TokenService foi atualizado para receber a entidade 'Usuario'
            return _tokenService.GerarToken(conta.Usuario);
        }

        // Métodos da interface mantidos para evitar erros de contrato
        public string GerarToken(LoginDTO loginDto) => throw new NotImplementedException("Use o ITokenService");
        public bool ValidarUsuario(LoginDTO loginDto) => throw new NotImplementedException();
    }
}

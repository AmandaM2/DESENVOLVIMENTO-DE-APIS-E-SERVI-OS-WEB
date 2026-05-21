using Microsoft.AspNetCore.Mvc;
using Api.DTOs;
using Api.Interfaces;
using System.Threading.Tasks;
using System;

namespace Api.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IContaRepository _contaRepository;
        private readonly ITokenService _tokenService; // Injetando o serviço de token que corrigimos!

        // Atualizado o construtor para receber o ITokenService no lugar do IConfiguration
        public AuthController(IContaRepository contaRepository, ITokenService tokenService)
        {
            _contaRepository = contaRepository;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            // 1. Busca a conta usando o método otimizado que já traz o Usuário junto (via Include)
            var conta = await _contaRepository.GetByTitularAsync(dto.Usuario);

            // 2. Valida se a conta existe, se o usuário está vinculado e se a senha bate
            if (conta == null || conta.Usuario == null || conta.Usuario.Senha != dto.Senha)
            {
                return Unauthorized(new { Erro = "Usuário ou senha inválidos." });
            }

            // 3. Delega a fabricação do Token JWT para o TokenService, passando o Usuario correto
            var tokenString = _tokenService.GerarToken(conta.Usuario);

            // Retorna o token gerado com sucesso
            return Ok(new { Token = tokenString });
        }
    }
}

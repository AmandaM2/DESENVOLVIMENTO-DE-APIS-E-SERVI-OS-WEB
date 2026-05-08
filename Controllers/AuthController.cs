using Microsoft.AspNetCore.Mvc;
using Api.DTOs;
using Api.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IContaRepository _contaRepository;
        private readonly IConfiguration _configuration;

        // Injetando o Repositório e a Configuração (para ler o appsettings.json)
        public AuthController(IContaRepository contaRepository, IConfiguration configuration)
        {
            _contaRepository = contaRepository;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            // 1. Busca a conta no banco usando o 'Usuario' (que estamos assumindo ser o Titular)
            var contas = await _contaRepository.GetAllAsync();
            var conta = contas.FirstOrDefault(c => c.Titular == dto.Usuario);

            // 2. Valida a senha (Em texto puro)
            if (conta == null || conta.Senha != dto.Senha)
                return Unauthorized(new { Erro = "Conta ou senha inválidos." });

            // 3. Fabrica o Token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    // GUARDAMOS O ID DA CONTA DENTRO DO TOKEN! Isso é a chave da segurança.
                    new Claim(ClaimTypes.NameIdentifier, conta.Id.ToString()),
                    new Claim(ClaimTypes.Name, conta.Titular)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { Token = tokenHandler.WriteToken(token) });
        }
    }
}

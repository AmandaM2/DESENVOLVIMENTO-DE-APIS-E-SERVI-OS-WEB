using Microsoft.AspNetCore.Mvc;
using Api.DTOs;
using Api.Interfaces;
using Api.Models;
using System.Threading.Tasks;
using System;

namespace Api.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IContaRepository _contaRepository;
        private readonly ITokenService _tokenService;

        public AuthController(IContaRepository contaRepository, ITokenService tokenService)
        {
            _contaRepository = contaRepository;
            _tokenService = tokenService;
        }

        // 1. ENDPOINT DE LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            // Busca a conta usando o método otimizado que já traz o Usuário junto
            var conta = await _contaRepository.GetByTitularAsync(dto.Usuario);

            // Valida se a conta existe, se o usuário está vinculado e se a senha bate
            if (conta == null || conta.Usuario == null || conta.Usuario.Senha != dto.Senha)
            {
                return Unauthorized(new { Erro = "Usuário ou senha inválidos." });
            }

            // Delega a fabricação do Token JWT para o TokenService
            var tokenString = _tokenService.GerarToken(conta.Usuario);

            return Ok(new { Token = tokenString });
        }

        // 2. ENDPOINT DE CADASTRO (Único e Corrigido)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginDTO dto)
        {
            try
            {
                bool temMaiuscula = dto.Senha.Any(char.IsUpper);
                bool temMinuscula = dto.Senha.Any(char.IsLower);
                bool temNumero = dto.Senha.Any(char.IsDigit);
                bool temEspecial = dto.Senha.Any(ch => !char.IsLetterOrDigit(ch));

                if (!temMaiuscula || !temMinuscula || !temNumero || !temEspecial)
                {
                    return BadRequest(new { Erro = "A senha deve conter pelo menos 1 letra maiúscula, 1 minúscula, 1 número e 1 caractere especial." });
                }
                //  Verifica se já existe uma conta com esse mesmo usuário para evitar duplicidade
                var contaExistente = await _contaRepository.GetByTitularAsync(dto.Usuario);
                if (contaExistente != null)
                {
                    return BadRequest(new { Erro = "Este usuário já está sendo utilizado." });
                }

                // Cria o objeto do Usuário e da Conta Bancária usando a propriedade 'Nome' que você ajustou
                var novaConta = new Conta
                {
                    Tipo = dto.TipoConta,
                    Saldo = 0.00m,
                    LimiteCartao = 500.00m,
                    Cofrinho = 0.00m,
                    Usuario = new Usuario
                    {
                        Nome = dto.Usuario,
                        Email = dto.Email,
                        Cpf = dto.Cpf,
                        Senha = dto.Senha
                    }
                };

                // Salva a nova conta no banco de dados usando o método correto da sua interface
                await _contaRepository.CreateAsync(novaConta);

                return Ok(new { Mensagem = "Conta criada com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Erro = "Erro ao salvar no MySQL: " + ex.Message });
            }
        }
    }
}

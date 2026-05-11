using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Api.DTOs;
using Api.Interfaces;
using System.Threading.Tasks;
using System;

namespace Api.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    [Authorize] // Garante que só usuários logados acessem a Controller...
    public class ContasController : ControllerBase
    {
        // Alterado para IContaRepository para usarmos o que já está pronto e funcionando!
        private readonly IContaRepository _repository;

        public ContasController(IContaRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var conta = await _repository.GetByIdAsync(id);
            if (conta == null) return NotFound(new { erro = "Conta não encontrada." });
            return Ok(conta);
        }

        [AllowAnonymous] // A MÁGICA AQUI: Permite que pessoas sem login acessem apenas este método!
        [HttpPost("criar")]
        public async Task<IActionResult> CriarConta([FromBody] ContaCreateDTO novaConta)
        {
            try
            {
                // Verifica se já existe alguém com esse nome para não dar erro no Login
                var contaExistente = await _repository.GetByTitularAsync(novaConta.Titular);
                if (contaExistente != null)
                {
                    return BadRequest(new { erro = "Já existe uma conta com este titular. Escolha outro nome." });
                }

                // Cria a conta de fato
                var contaCriada = await _repository.CreateAsync(
                    new Conta
                    {
                        Titular = novaConta.Titular,
                        Senha = novaConta.Senha,
                        Tipo = novaConta.Tipo,
                        Saldo = novaConta.Saldo
                    }
                );

                return Ok(new
                {
                    mensagem = "Conta criada com sucesso! Agora você já pode fazer login.",
                    id = contaCriada.Id
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }
    }
}

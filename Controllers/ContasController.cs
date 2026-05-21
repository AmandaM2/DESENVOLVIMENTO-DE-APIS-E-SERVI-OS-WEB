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

        [AllowAnonymous] // Permite que pessoas sem login acessem para se cadastrar
        [HttpPost("criar")]
        public async Task<IActionResult> CriarConta([FromBody] ContaCreateDTO novaConta)
        {
            try
            {
                // Verifica se já existe uma conta com este titular
                var contaExistente = await _repository.GetByTitularAsync(novaConta.Titular);
                if (contaExistente != null)
                {
                    return BadRequest(new { erro = "Já existe uma conta com este titular. Escolha outro nome." });
                }

                // CORREÇÃO: Instanciando a Conta e o Usuário de forma interligada
                var contaParaCriar = new Conta
                {
                    Tipo = novaConta.Tipo,
                    Saldo = novaConta.Saldo,
                    Usuario = new Usuario
                    {
                        Nome = novaConta.Titular,
                        Email = novaConta.Titular.Contains("@") ? novaConta.Titular : $"{novaConta.Titular.ToLower()}@banco.com",
                        Senha = novaConta.Senha // Caso use hash futuramente (ex: BCrypt), aplique-o aqui
                    }
                };

                // Envia a estrutura completa para o repositório salvar
                var contaCriada = await _repository.CreateAsync(contaParaCriar);

                return Ok(new
                {
                    mensagem = "Conta e usuário criados com sucesso! Agora você já pode fazer login.",
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

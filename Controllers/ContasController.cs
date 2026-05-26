using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Api.DTOs;
using Api.Interfaces;
using System.Threading.Tasks;
using System;
using System.Collections.Generic; // Necessário para a lista padrão de transações

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

        // ==========================================================================
        // NOVO: ENDPOINT DO DASHBOARD (Resolve o Erro 400 do script.js)
        // ==========================================================================
        [HttpGet("detalhes")]
        public async Task<IActionResult> ObterDetalhes()
        {
            try
            {
                // Extrai o nome do usuário logado diretamente de dentro do Token JWT
                var usuarioLogado = User.Identity?.Name;
                Console.WriteLine(usuarioLogado);

                if (string.IsNullOrEmpty(usuarioLogado))
                {
                    return BadRequest(new { erro = "Não foi possível identificar o usuário no token." });
                }

                // Busca a estrutura da conta vinculada a esse usuário
                var conta = await _repository.GetByTitularAsync(usuarioLogado);

                if (conta == null)
                {
                    return NotFound(new { erro = "Conta bancária não encontrada." });
                }

                // Retorna os dados com as chaves minúsculas combinando com o script.js
                return Ok(new
                {
                    saldo = conta.Saldo,
                    limiteCartao = conta.LimiteCartao,
                    cofrinho = conta.Cofrinho,
                    tipo = conta.Tipo ?? "Corrente",

                    transacoes = (conta.Transacoes ?? new List<Transacao>())
        .OrderByDescending(t => t.DataHora)
        .Select(t => new
        {
            dataHora = t.DataHora,

            // 🛠️ CORRIGIDO: Agora usando 't.Tipo' que existe no seu Model
            tipoOperacao = (int)t.Tipo switch
            {
                1 => "Saque",
                2 => "Depósito",
                _ => "Outro"
            },

            valor = t.Valor
        }).ToList()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = "Erro ao processar dados do painel: " + ex.Message });
            }
        }

        // ==========================================================================
        // SEUS ENDPOINTS ORIGINAIS PRESERVADOS
        // ==========================================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var conta = await _repository.GetByIdAsync(id);
            if (conta == null) return NotFound(new { erro = "Conta não encontrada." });
            return Ok(conta);
        }

        [AllowAnonymous]
        [HttpPost("criar")]
        public async Task<IActionResult> CriarConta([FromBody] ContaCreateDTO novaConta)
        {
            try
            {
                var contaExistente = await _repository.GetByTitularAsync(novaConta.Titular);
                if (contaExistente != null)
                {
                    return BadRequest(new { erro = "Já existe uma conta com este titular." });
                }

                var contaParaCriar = new Conta
                {
                    Tipo = novaConta.Tipo,
                    Saldo = novaConta.Saldo,

                    // 🛠️ ADICIONE ESSAS DUAS LINHAS AQUI:
                    Cofrinho = 0.0m, // Ou novaConta.Cofrinho se o seu DTO já tiver esse campo
                    LimiteCartao = 1000.0m, // Define um limite padrão inicial para o cliente

                    Usuario = new Usuario
                    {
                        Nome = novaConta.Titular,
                        Email = novaConta.Titular.Contains("@") ? novaConta.Titular : $"{novaConta.Titular.ToLower()}@banco.com",
                        Senha = novaConta.Senha
                    }
                };

                var contaCriada = await _repository.CreateAsync(contaParaCriar);
                return Ok(new { mensagem = "Conta criada com sucesso!", id = contaCriada.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }
    }
}
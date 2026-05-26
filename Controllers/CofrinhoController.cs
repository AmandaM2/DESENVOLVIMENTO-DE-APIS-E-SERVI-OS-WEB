using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Interfaces;
using Api.DTOs;
using Api.Models; // Necessário para reconhecer 'Transacao' e 'TipoTransacao'
using System.Threading.Tasks;
using System;

namespace Api.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    [Authorize]
    public class CofrinhoController : ControllerBase
    {
        private readonly IContaRepository _contaRepository;

        public CofrinhoController(IContaRepository contaRepository)
        {
            _contaRepository = contaRepository;
        }

        // 1. ROTA: /Api/Cofrinho/guardar
        [HttpPost("guardar")]
        public async Task<IActionResult> Guardar([FromBody] CofrinhoOperacaoDTO dto)
        {
            try
            {
                var usuarioLogado = User.Identity?.Name;
                var conta = await _contaRepository.GetByTitularAsync(usuarioLogado ?? "Usuário não identificado");

                if (conta == null)
                    return NotFound(new { erro = "Conta não encontrada." });

                if (conta.Saldo < dto.Valor)
                    return BadRequest(new { erro = "Saldo insuficiente para guardar no cofrinho." });

                // 1. Atualiza os saldos da conta
                conta.Saldo -= dto.Valor;
                conta.Cofrinho += dto.Valor;
                await _contaRepository.UpdateAsync(conta.Id, conta);

                // 🔥 CORRIGIDO: Agora usa a propriedade 'Tipo' e o enum 'TipoTransacao' (1 = Saque)
                var novaTransacao = new Transacao
                {
                    ContaId = conta.Id,
                    Valor = dto.Valor,
                    DataHora = DateTime.Now,
                    Tipo = (TipoTransacao)1
                };
                await _contaRepository.AdicionarTransacaoAsync(novaTransacao);

                // Retorna os valores atualizados para o script.js
                return Ok(new
                {
                    mensagem = $"R$ {dto.Valor:F2} guardados no seu cofrinho com sucesso!",
                    saldo = conta.Saldo,
                    cofrinho = conta.Cofrinho
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = "Erro ao guardar dinheiro: " + ex.Message });
            }
        }

        // 2. ROTA: /Api/Cofrinho/resgatar
        [HttpPost("resgatar")]
        public async Task<IActionResult> Resgatar([FromBody] CofrinhoOperacaoDTO dto)
        {
            try
            {
                var usuarioLogado = User.Identity?.Name;
                var conta = await _contaRepository.GetByTitularAsync(usuarioLogado ?? "Usuário não identificado");

                if (conta == null)
                    return NotFound(new { erro = "Conta não encontrada." });

                if (conta.Cofrinho < dto.Valor)
                    return BadRequest(new { erro = "Você não tem esse valor no cofrinho para resgatar." });

                // 1. Atualiza os saldos da conta
                conta.Cofrinho -= dto.Valor;
                conta.Saldo += dto.Valor;
                await _contaRepository.UpdateAsync(conta.Id, conta);

                // 🔥 CORRIGIDO: Agora usa a propriedade 'Tipo' e o enum 'TipoTransacao' (2 = Depósito)
                var novaTransacao = new Transacao
                {
                    ContaId = conta.Id,
                    Valor = dto.Valor,
                    DataHora = DateTime.Now,
                    Tipo = (TipoTransacao)2
                };
                await _contaRepository.AdicionarTransacaoAsync(novaTransacao);

                // Retorna os valores atualizados para o script.js
                return Ok(new
                {
                    mensagem = $"R$ {dto.Valor:F2} resgatados para o seu saldo disponível!",
                    saldo = conta.Saldo,
                    cofrinho = conta.Cofrinho
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = "Erro ao resgatar dinheiro: " + ex.Message });
            }
        }
    }
}
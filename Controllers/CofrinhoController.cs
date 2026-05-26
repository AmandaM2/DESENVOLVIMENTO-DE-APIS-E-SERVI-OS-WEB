using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Interfaces;
using Api.DTOs;
using System.Threading.Tasks;
using System;

namespace Api.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    [Authorize] // Proteção: Só usuários logados podem mexer no cofrinho
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
                // Pega o nome do usuário direto do Token JWT
                var usuarioLogado = User.Identity?.Name;
                var conta = await _contaRepository.GetByTitularAsync(usuarioLogado ??= "Usuário não identificado");

                if (conta == null)
                    return NotFound(new { erro = "Conta não encontrada." });

                if (conta.Saldo < dto.Valor)
                    return BadRequest(new { erro = "Saldo insuficiente para guardar no cofrinho." });

                // Regra de negócio: Tira do saldo disponível e coloca na poupança
                conta.Saldo -= dto.Valor;
                conta.Cofrinho += dto.Valor;

                // Salva a alteração no MySQL usando o repositório corrigido
                await _contaRepository.UpdateAsync(conta.Id, conta);

                return Ok(new { mensagem = $"R$ {dto.Valor:F2} guardados no seu cofrinho com sucesso!" });
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
                var conta = await _contaRepository.GetByTitularAsync(usuarioLogado ??= "Usuário não identificado");

                if (conta == null)
                    return NotFound(new { erro = "Conta não encontrada." });

                if (conta.Cofrinho < dto.Valor)
                    return BadRequest(new { erro = "Você não tem esse valor no cofrinho para resgatar." });

                // Regra de negócio: Tira do cofrinho e devolve para o saldo disponível
                conta.Cofrinho -= dto.Valor;
                conta.Saldo += dto.Valor;

                // Salva a alteração no MySQL
                await _contaRepository.UpdateAsync(conta.Id, conta);

                return Ok(new { mensagem = $"R$ {dto.Valor:F2} resgatados para o seu saldo disponível!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = "Erro ao resgatar dinheiro: " + ex.Message });
            }
        }
    }
}

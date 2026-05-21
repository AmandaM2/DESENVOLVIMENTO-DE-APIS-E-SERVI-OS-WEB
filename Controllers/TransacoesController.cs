using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.DTOs;
using Api.Interfaces;
using System.Threading.Tasks;
using System;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    [Authorize] // Segurança JWT ativada: Só entra quem tem o token!
    public class TransacoesController : ControllerBase
    {
        private readonly ITransacaoService _service;

        public TransacoesController(ITransacaoService service)
        {
            _service = service;
        }

        [HttpPost("sacar")]
        public async Task<IActionResult> Sacar([FromBody] TransacaoDTO dto)
        {
            try
            {
                // Recupera o ID criptografado no Token JWT
                var idToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                dto.ContaId = int.Parse(idToken!);

                await _service.RealizarSaque(dto);

                return Ok(new
                {
                    mensagem = "Saque realizado com sucesso!",
                    novoSaldo = await _service.ObterSaldoAsync(dto.ContaId)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        [HttpPost("depositar")]
        public async Task<IActionResult> Depositar([FromBody] TransacaoDTO dto)
        {
            try
            {
                var idToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                dto.ContaId = int.Parse(idToken!);

                await _service.RealizarDeposito(dto);

                return Ok(new
                {
                    mensagem = "Depósito realizado com sucesso!",
                    novoSaldo = await _service.ObterSaldoAsync(dto.ContaId)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        // 📜 ADICIONADO: Endpoint de Extrato
        [HttpGet("extrato")]
        public async Task<IActionResult> Extrato()
        {
            try
            {
                var idToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int contaId = int.Parse(idToken!);

                // Busca o histórico de transações mapeado pelo seu serviço
                var historico = await _service.ObterExtratoAsync(contaId);
                var saldoAtual = await _service.ObterSaldoAsync(contaId);

                return Ok(new
                {
                    saldo = saldoAtual,
                    transacoes = historico
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.DTOs;
using Api.Interfaces;
using System.Threading.Tasks;
using System;
using System.Security.Claims; // ADICIONADO: Para ler os dados do Token JWT

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
                // REGRA DE SEGURANÇA: Pegando o ID da conta que está dentro do Token JWT
                var idToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (idToken != dto.ContaId.ToString())
                {
                    // Forbid = Erro 403 (Proibido)
                    return StatusCode(403, new { erro = "Acesso negado. Você só pode movimentar a sua própria conta!" });
                }

                await _service.RealizarSaque(dto);
                return Ok(new { mensagem = "Saque realizado!", novoSaldo = await _service.ObterSaldoAsync(dto.ContaId) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message }); // Erro 400
            }
        }

        [HttpPost("depositar")]
        public async Task<IActionResult> Depositar([FromBody] TransacaoDTO dto)
        {
            try
            {
                // REGRA DE SEGURANÇA (O mesmo bloqueio do saque)
                var idToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (idToken != dto.ContaId.ToString())
                {
                    return StatusCode(403, new { erro = "Acesso negado. Você só pode depositar na sua própria conta usando este endpoint logado!" });
                }

                await _service.RealizarDeposito(dto);
                return Ok(new { mensagem = "Depósito realizado!" });
            }
            catch (Exception ex) // ADICIONADO: Tratamento de erro que faltava
            {
                return BadRequest(new { erro = ex.Message });
            }
        }
    }
}

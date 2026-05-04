using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Banco.Services;
using Banco.DTOs;

namespace Banco.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [cite_start]
    [Authorize] // Segurança JWT 
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
                [cite_start]// A lógica de verificar saldo e aplicar taxas está dentro do Service [cite: 58, 59]
                await _service.RealizarSaque(dto);
                return Ok(new { mensagem = "Saque realizado!", novoSaldo = await _service.ObterSaldo(dto.ContaId) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        [HttpPost("depositar")]
        public async Task<IActionResult> Depositar([FromBody] TransacaoDTO dto)
        {
            await _service.RealizarDeposito(dto);
            return Ok(new { mensagem = "Depósito realizado!" });
        }
    }
}
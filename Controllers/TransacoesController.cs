using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.DTOs;
using api.Interfaces;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
                await _service.RealizarSaque(dto);
                return Ok(new { mensagem = "Saque realizado!", novoSaldo = _service.ObterSaldo(dto.ContaId) });
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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.DTOs;
using Api.Interfaces;

namespace Api.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    [Authorize] // Garante que só usuários logados acessem
    public class ContasController : ControllerBase
    {
        private readonly IContaService _service;

        public ContasController(IContaService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var conta = await _service.BuscarPorId(id);
            if (conta == null) return NotFound();
            return Ok(conta);
        }

        [HttpPost]
        public async Task<IActionResult> CriarConta(ContaDTO contaDto)
        {
            await _service.CriarConta(contaDto);
            return Ok("Conta criada com sucesso");
        }
    }
}

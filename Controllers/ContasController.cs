using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Banco.Services;
using Banco.DTOs;

namespace Banco.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [cite_start]
    [Authorize] // Garante que só usuários logados acessem 
    public class ContasController : ControllerBase
    {
        private readonly IContaService _service;

        public ContasController(IContaService service)
        {
            _service = service;
        }

        [cite_start]// Busca dados da conta para exibir na View [cite: 61]
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
            await _service.Criar(contaDto);
            return Ok("Conta criada com sucesso");
        }
    }
}
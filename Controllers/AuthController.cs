using Microsoft.AspNetCore.Mvc;
using api.DTOs;
using api.Interfaces;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO login)
        {
            var token = _authService.GerarToken(login);
            if (token == null) return Unauthorized("Usuário ou senha inválidos");
            return Ok(new { token });
        }
    }
}

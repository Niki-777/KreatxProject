using KreatxProject.Models;
using KreatxProject.Server.Services; // Kjo siguron që Controller-i sheh folderin e Services
using Microsoft.AspNetCore.Mvc;

namespace KreatxProject.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        //(Dependency Injection)
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // Validimi automatik i Annotations ([Required], [EmailAddress])
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.LoginAsync(model);

            // Nëse diçka shkoi gabim (IsSuccess është false), kthejmë Unauthorized me mesazhin përkatës
            if (!result.IsSuccess)
            {
                return Unauthorized(new { message = result.ErrorMessage });
            }

            // Nëse çdo gjë shkoi mirë, kthejmë të dhënat që pret React-i
            return Ok(new
            {
                token = result.Token,
                expiration = result.Expiration,
                role = result.Role
            });
        }
    }
}
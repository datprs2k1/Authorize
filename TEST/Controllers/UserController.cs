using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TEST.Models.Token;
using TEST.Models.User;
using TEST.Repositories;

namespace TEST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repo;

        public UserController(IUserRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("")]
        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            return Ok("OK");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _repo.LoginAsync(dto);

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _repo.RegisterAsync(dto);

            if (result)
            {
                return Ok(new
                {
                    message = "User is created."
                });
            }

            return BadRequest("Something is error.");
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenDto dto)
        {
            var result = await _repo.RefreshToken(dto);

            return Ok(result);
        }
    }
}

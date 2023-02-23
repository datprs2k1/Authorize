using AutoMapper;
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
        private readonly IMapper _mapper;

        public UserController(IUserRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet("")]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            var data = await _repo.GetUsersAsync();

            return Ok(data);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetUserById(int id)
        {
            var data = await _repo.GetUserById(id);

            if (data == null)
            {
                return BadRequest("User is not exist.");
            }

            return Ok(data);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, UserDto user)
        {
            var result = await _repo.UpdateUserAsync(id, user);

            if (!result)
            {
                return BadRequest("Errors.");
            }

            return Ok("Updated.");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _repo.DeleteUserAsync(id);

            if (!result)
            {
                return BadRequest("Errors.");
            }

            return Ok("Deleted.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _repo.LoginAsync(dto);

            var user = await _repo.GetUserById(result.UserID);

            return Ok(new
            {
                token = new
                {
                    accessToken = result.AccessToken,
                    refreshToken = result.RefreshToken,
                },
                user = _mapper.Map<UserDto>(user)
            });
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

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(TokenDto dto)
        {
            var result = await _repo.LogoutAsync(dto);

            if (!result)
            {
                return BadRequest("Errors.");
            }

            return Ok("Success.");
        }
    }
}

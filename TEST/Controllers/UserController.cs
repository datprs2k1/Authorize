using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TEST.Core;
using TEST.Models.Token;
using TEST.Models.User;

namespace TEST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetUsers()
        {
            var data = await _unitOfWork.Users.All();

            return Ok(data);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetUserById(int id)
        {
            var data = await _unitOfWork.Users.GetUserById(id);

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
            var result = await _unitOfWork.Users.UpdateUserAsync(id, user);

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
            var result = await _unitOfWork.Users.DeleteUserAsync(id);

            if (!result)
            {
                return BadRequest("Errors.");
            }

            return Ok("Deleted.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _unitOfWork.Users.LoginAsync(dto);

            var user = await _unitOfWork.Users.GetUserById(result.UserID);

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
            var result = await _unitOfWork.Users.RegisterAsync(dto);

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
            var result = await _unitOfWork.Users.RefreshToken(dto);

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(TokenDto dto)
        {
            var result = await _unitOfWork.Users.LogoutAsync(dto);

            if (!result)
            {
                return BadRequest("Errors.");
            }

            return Ok("Success.");
        }
    }
}

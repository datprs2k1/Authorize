using TEST.Models.Token;
using TEST.Models.User;

namespace TEST.Repositories
{
    public interface IUserRepository
    {
        public Task<TokenDto> LoginAsync(LoginDto dto);
        public Task<Boolean> RegisterAsync(RegisterDto dto);
        public Task<Boolean> LogoutAsync(TokenDto dto);
        public Task<TokenDto> RefreshToken(TokenDto dto);
        public Task<List<UserDto>> GetUsersAsync();
        public Task<UserDto> GetUserById(int id);
        public Task<Boolean> UpdateUserAsync(int id, UserDto user);
        public Task<Boolean> DeleteUserAsync(int id);

    }
}

using TEST.Models.Token;
using TEST.Models.User;

namespace TEST.Repositories
{
    public interface IUserRepository
    {
        public Task<TokenDto> LoginAsync(LoginDto dto);
        public Task<Boolean> RegisterAsync(RegisterDto dto);
        public Task<TokenDto> RefreshToken(TokenDto dto);

    }
}

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TEST.Data;
using TEST.Models.Token;
using TEST.Models.User;

namespace TEST.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly APIEntities _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserRepository(APIEntities context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<TokenDto> LoginAsync(LoginDto dto)
        {
            try
            {
                var userLogin = _mapper.Map<User>(dto);

                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userLogin.Email);

                if (user == null && user.Password != userLogin.Password)
                {
                    throw new ApplicationException("Email or Password is not correct.");
                }

                var token = await GenerateToken(user);

                return token;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }

        private async Task<TokenDto> GenerateToken(User user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: authClaims,
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials: signingCredentials
                );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = GenerateRefreshToken();

            var newRefreshToken = new Token()
            {
                token = refreshToken,
                Jti = token.Id,
                isUsed = false,
                isRevoked = false,
                UserID = user.Id,
                expiredAt = DateTime.UtcNow.AddMinutes(1),
                createdAt = DateTime.UtcNow
            };

            _context.Tokens.Add(newRefreshToken);

            await _context.SaveChangesAsync();

            return new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new Byte[64];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            try
            {
                var exist = await _context.Users.AnyAsync(x => x.Email == dto.Email);

                if (exist)
                {
                    throw new ApplicationException("Email is exist.");
                }

                var user = _mapper.Map<User>(dto);

                _context.Users.Add(user);

                await _context.SaveChangesAsync();

                return true;

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public async Task<TokenDto> RefreshToken(TokenDto dto)

        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var jwtValidateParams = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,

                ValidAudience = _configuration["JWT:Audience"],
                ValidIssuer = _configuration["JWT:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),

                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var tokenInVerification = jwtTokenHandler.ValidateToken(dto.AccessToken, jwtValidateParams, out var tokenValidated);

                if (tokenValidated is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if (!result)
                    {
                        throw new ApplicationException("Token's Algorithms is error.");
                    }
                }

                var tokenStored = await _context.Tokens.FirstOrDefaultAsync(x => x.token == dto.RefreshToken);

                if (tokenStored == null)
                {
                    throw new ApplicationException("Token is not exist.");
                }

                if (tokenStored.isUsed)
                {
                    throw new ApplicationException("Token is used.");
                }

                if (tokenStored.isRevoked)
                {
                    throw new ApplicationException("Token is revoked");
                }

                var Jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

                if (tokenStored.Jti != Jti)
                {
                    throw new ApplicationException("Jti is not corrected");
                }

                var tokenExpiredDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)?.Value);

                var expiredDate = ConvertUnixToDateTime(tokenExpiredDate);

                if (expiredDate > DateTime.UtcNow)
                {
                    throw new ApplicationException("Token is not expired.");
                }

                tokenStored.isUsed = true;
                tokenStored.isRevoked = true;

                _context.Tokens.Update(tokenStored);
                await _context.SaveChangesAsync();

                var user = await _context.Users.FindAsync(tokenStored.UserID);

                var token = await GenerateToken(user);

                return token;

            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);

            }
        }
        private DateTime ConvertUnixToDateTime(long tokenExpiredDate)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(tokenExpiredDate);

            return dateTimeOffset.UtcDateTime;
        }
    }
}

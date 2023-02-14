using System.ComponentModel.DataAnnotations;

namespace TEST.Models.Token
{
    public class TokenDto
    {
        [Required]
        public string? AccessToken { get; set; }
        [Required]
        public string? RefreshToken { get; set; }
    }
}

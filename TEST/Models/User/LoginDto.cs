using System.ComponentModel.DataAnnotations;

namespace TEST.Models.User
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}


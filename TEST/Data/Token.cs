using System.ComponentModel.DataAnnotations;

namespace TEST.Data
{
    public class Token
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? token { get; set; }
        [Required]
        public string? Jti { get; set; }
        [Required]
        public Boolean isUsed { get; set; }
        [Required]
        public Boolean isRevoked { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public DateTime expiredAt { get; set; }
        [Required]
        public DateTime createdAt { get; set; }

        public User? User { get; set; }
    }
}

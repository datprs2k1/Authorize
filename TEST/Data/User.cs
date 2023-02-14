using System.ComponentModel.DataAnnotations;

namespace TEST.Data
{
    public class User
    {
        public User()
        {
            Tokens = new HashSet<Token>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }

        public ICollection<Token> Tokens { get; set; }
    }
}

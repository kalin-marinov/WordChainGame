using System.ComponentModel.DataAnnotations;

namespace WordChainGame.Models
{
    public class RegisterUserModel
    {
        [Required]
        public string UserName { get; set; }

        public string FullName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required EmailAddress]
        public string Email { get; set; }

    }
}

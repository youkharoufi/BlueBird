using System.ComponentModel.DataAnnotations;

namespace BlueBirds.Models
{
    public class ResetPassword
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}

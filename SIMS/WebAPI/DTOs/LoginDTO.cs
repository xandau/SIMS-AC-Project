using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class LoginDTO
    {
        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }
    }
}

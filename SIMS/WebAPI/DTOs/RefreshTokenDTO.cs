using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class RefreshTokenDTO
    {
        [Required]
        public string Token { get; set; }
    }
}

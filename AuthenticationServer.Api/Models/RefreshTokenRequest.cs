using System.ComponentModel.DataAnnotations;

namespace AuthenticationServer.Api.Models
{
    public class RefreshTokenRequest
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}

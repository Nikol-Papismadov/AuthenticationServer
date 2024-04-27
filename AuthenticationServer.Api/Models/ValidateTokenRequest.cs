using System.ComponentModel.DataAnnotations;

namespace AuthenticationServer.Api.Models
{
    public class ValidateTokenRequest
    {
        [Required]
        public string Token { get; set; } = null!;
    }
}

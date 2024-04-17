using System.ComponentModel.DataAnnotations;

namespace AuthenticationServer.Api.Models
{
    public class RegisterRequest
    {
        [Required]
        [RegularExpression(@"^[\w]{5,20}$")]
        public string UserName { get; set; } = null!;
        [Required]
        [RegularExpression(@"^[\w]{5,20}$")]
        public string Password { get; set; } = null!;
        [Required]
        public string ConfirmPassword { get; set; } = null!;
    }
}

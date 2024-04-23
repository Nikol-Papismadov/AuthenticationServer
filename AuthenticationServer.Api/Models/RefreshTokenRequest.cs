namespace AuthenticationServer.Api.Models
{
    public class RefreshTokenRequest
    {
        public string Username { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}

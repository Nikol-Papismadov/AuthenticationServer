using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationServer.Services;

public interface IAuthenticationService
{
    Task<(string accessToken, string refreshToken)> Login(string username, string password);
    Task Register(string username, string password);
    Task<string> RefreshToken(string username, string refreshToken);
    Task Logout(string username);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationServer.Services;

public interface IAuthenticationService
{
    Task<string> Login(string username, string password);
    Task Register(string username, string password);
}

using AuthenticationServer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationServer.Services
{
    public interface ITokenGenerator
    {
        string GenerateAccessToken(AppUser user);
        string GenerateRefreshToken(AppUser user);
        string GenerateToken(AppUser user, string secret, double expiration);
    }
}

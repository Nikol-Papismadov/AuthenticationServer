using AuthenticationServer.Models;
using AuthenticationServer.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationServer.Services
{
    public class JwtTokenGenerator(AuthenticationConfiguration configuration) : ITokenGenerator
    {
        public string GenerateToken(AppUser user)
        {
            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.AccessTokenSecret));
            var credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
            var claims = new Claim[] { new Claim(ClaimTypes.Name, user.UserName) };
            var utcExperation = DateTime.UtcNow.AddMinutes(configuration.AccessTokenExpirationMinutes);
            var token = new JwtSecurityToken(
                issuer: configuration.Issuer,
                audience: configuration.Audience,
                claims: claims, expires: utcExperation,
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

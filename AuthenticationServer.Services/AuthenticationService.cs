using AuthenticationServer.Data.Exceptions;
using AuthenticationServer.Data.Repositories;
using AuthenticationServer.Models;
using AuthenticationServer.Models.Entities;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace AuthenticationServer.Services;

public class AuthenticationService(IAppUserRepository repository, IPasswordHasher hasher, ITokenGenerator tokenGenerator, AuthenticationConfiguration configuration) : IAuthenticationService
{
    static private Dictionary<string, string> userRefreshTokenPairs = new Dictionary<string, string>();
    public async Task<(string accessToken, string refreshToken)> Login(string username, string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(username);
        ArgumentException.ThrowIfNullOrEmpty(password);

        AppUser user = await GetUser(repository, username);

        if (!hasher.Verify(password, user.PasswordHash))
        {
            throw new InvalidOperationException("Wrong password");
        }

        var refreshToken = tokenGenerator.GenerateRefreshToken(user);
        userRefreshTokenPairs[username] = refreshToken;

        return (tokenGenerator.GenerateAccessToken(user), refreshToken);
    }

    private static async Task<AppUser> GetUser(IAppUserRepository repository, string username)
    {
        if ((await repository.UserExists(username)) == false)
        {
            throw new InvalidOperationException("User not found");
        }
        return await repository.GetByUserName(username);
    }

    
    public async Task Register(string username, string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(username);
        ArgumentException.ThrowIfNullOrEmpty(password);
        if (await repository.UserExists(username))
        {
            throw new InvalidOperationException("Username Unavilable");
        }
        var user = new AppUser
        {
            UserName = username,
            PasswordHash = hasher.Hash(password)
        };

        try
        {
            await repository.Add(user);
        }
        catch (DatabaseException ex)
        {
            throw new InvalidOperationException("database error", ex);
        }

    }
    public async Task<bool> ValidateToken(string accessToken)
    {
        var validationParameters = new TokenValidationParameters()
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.AccessTokenSecret)),
            ValidIssuer = configuration.Issuer,
            ValidAudience = configuration.Audience,
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ClockSkew = TimeSpan.Zero
        };
        try
        {
            var principles = await new JwtSecurityTokenHandler().ValidateTokenAsync(accessToken, validationParameters);
            return principles.IsValid;
        }
        catch
        {
            return false;
        }
    }
    public async Task<string> RefreshToken(string username, string refreshToken)
    {
        
        ArgumentException.ThrowIfNullOrEmpty(username);
        ArgumentException.ThrowIfNullOrEmpty(refreshToken);

        AppUser user = await GetUser(repository, username);

        if (!userRefreshTokenPairs.ContainsKey(username))
            throw new Exception("user not found");

        if (userRefreshTokenPairs[username] != refreshToken)
            throw new Exception("Wrong refresh token");

        return tokenGenerator.GenerateAccessToken(user);
    }

    public async Task Logout(string username)
    {
        ArgumentException.ThrowIfNullOrEmpty(username);
        if (userRefreshTokenPairs.ContainsKey(username))
        {
            userRefreshTokenPairs.Remove(username);
        }
        else
        {
            throw new KeyNotFoundException("Username not found");
        }
    }
}


using AuthenticationServer.Data.Exceptions;
using AuthenticationServer.Data.Repositories;
using AuthenticationServer.Models.Entities;

namespace AuthenticationServer.Services;

public class AuthenticationService(IAppUserRepository repository, IPasswordHasher hasher, ITokenGenerator tokenGenerator) : IAuthenticationService
{
    private KeyValuePair<string?, string?> userToken { get; set; } = new();
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
        storeRefreshToken(username, refreshToken);

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

    private void storeRefreshToken(string? username = null, string? refreshToken = null)
    {
        userToken = new KeyValuePair<string?, string?>(username, refreshToken);
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

    public async Task<string> RefreshToken(string username, string refreshToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(username);
        ArgumentException.ThrowIfNullOrEmpty(refreshToken);

        AppUser user = await GetUser(repository, username);

        if (refreshToken != userToken.Value)
        {
            throw new InvalidOperationException("Invalid refresh token");
        }

        return tokenGenerator.GenerateAccessToken(user);
    }

    public async Task Logout(string username)
    {
        ArgumentException.ThrowIfNullOrEmpty(username);

        AppUser user = await GetUser(repository, username);

        if (user.UserName != userToken.Key)
        {
            throw new InvalidOperationException("Logout failed");
        }
        storeRefreshToken();
    }
}


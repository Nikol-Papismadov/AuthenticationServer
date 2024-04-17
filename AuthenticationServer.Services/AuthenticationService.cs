using AuthenticationServer.Data.Exceptions;
using AuthenticationServer.Data.Repositories;
using AuthenticationServer.Models.Entities;

namespace AuthenticationServer.Services;

public class AuthenticationService(IAppUserRepository repository, IPasswordHasher hasher, ITokenGenerator tokenGenerator) : IAuthenticationService
{
    public async Task<string> Login(string username, string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(username);
        ArgumentException.ThrowIfNullOrEmpty(password);
        if ((await repository.UserExists(username)) == false)
        {
            throw new InvalidOperationException("User not found");
        }
        var user = await repository.GetByUserName(username);
        if (!hasher.Verify(password, user.PasswordHash))
        {
            throw new InvalidOperationException("Wrong password");
        }
        return tokenGenerator.GenerateToken(user);
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
}


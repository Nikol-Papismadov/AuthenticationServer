using AuthenticationServer.Data.Exceptions;
using AuthenticationServer.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AuthenticationServer.Data.Exceptions.DatabaseException;

namespace AuthenticationServer.Data.Repositories
{
    public class AppUserRepository(AuthenticationDbContext context) : IAppUserRepository
    {
        public async Task Add(AppUser user)
        {
            if (await UserExists(user.UserName))
            {
                throw new DuplicateEntityException();
            }
            try
            {
                await context.AppUsers.AddAsync(user);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("database error", ex);
            }
        }

        public async Task<AppUser> GetByUserName(string userName)
        {
            var user = await context.AppUsers.SingleOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                throw new EntityNotFoundException();
            }
            return user;
        }

        public async Task<bool> UserExists(string userName)
        {
            return await context.AppUsers.AnyAsync(u => u.UserName == userName);
        }
    }
}


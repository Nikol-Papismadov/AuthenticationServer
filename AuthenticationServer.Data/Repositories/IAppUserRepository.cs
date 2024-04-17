using AuthenticationServer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationServer.Data.Repositories
{
    public interface IAppUserRepository
    {
        Task<AppUser> GetByUserName(string userName);
        Task Add(AppUser User);
        Task<bool> UserExists(string userName);
    }
}

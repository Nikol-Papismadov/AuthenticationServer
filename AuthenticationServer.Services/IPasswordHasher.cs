using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationServer.Services
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string text, string hash);
    }
}

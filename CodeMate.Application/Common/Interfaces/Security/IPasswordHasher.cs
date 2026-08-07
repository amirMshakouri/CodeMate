using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Application.Common.Interfaces.Security
{
    public interface IPasswordHasher
    {
        string Hash(string password);

        bool Verify(string password, string passwordHash);
    }
}

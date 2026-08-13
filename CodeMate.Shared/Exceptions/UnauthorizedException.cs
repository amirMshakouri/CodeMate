using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Shared.Exceptions
{
    public sealed class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message) : base(message, 401) { }
    }

    
}

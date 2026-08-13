using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Shared.Exceptions
{
    public sealed class ForbiddenException : AppException
    {
        public ForbiddenException(string message) : base(message, 403) { }
    }
}

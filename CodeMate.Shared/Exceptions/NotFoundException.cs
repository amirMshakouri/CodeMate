using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Shared.Exceptions
{
    public sealed class NotFoundException : AppException
    {
        public NotFoundException(string message) : base(message, 404) { }
    }
}

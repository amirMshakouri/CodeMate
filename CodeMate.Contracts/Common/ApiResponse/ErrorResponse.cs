using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Contracts.Common.ApiResponse
{
    public sealed class ErrorResponse
    {
        public required string Message { get; set; }
        public IDictionary<string, string[]>? Errors { get; set; }
    }
}

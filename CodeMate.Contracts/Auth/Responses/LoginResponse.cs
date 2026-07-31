using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Contracts.Auth.Responses
{
    public sealed class LoginResponse
    {
        public required string Token { get; set; } 

        public  DateTimeOffset Expiration { get; set; }

        public required string UserName { get; set; } 
    }
}

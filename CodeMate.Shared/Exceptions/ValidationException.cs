using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Shared.Exceptions
{
   
   
        public sealed class ValidationException : AppException
        {
            public IDictionary<string, string[]> Errors { get; }

            public ValidationException(IDictionary<string, string[]> errors)
                : base("One or more validation errors occurred.", 400)
            {
                Errors = errors;
            }
        }
    }


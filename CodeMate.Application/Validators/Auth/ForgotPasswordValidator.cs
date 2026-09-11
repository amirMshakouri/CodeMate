using System;
using System.Collections.Generic;
using System.Text;
using CodeMate.Contracts.Auth.Requests;
using FluentValidation;

namespace CodeMate.Application.Validators.Auth;

public sealed class ForgotPasswordValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");
    }
}
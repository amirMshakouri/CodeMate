using CodeMate.Contracts.Users.Requests;
using FluentValidation;

namespace CodeMate.Application.Validators.Users;

public sealed class UpdateProfileValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.FullName)
            .MaximumLength(100)
            .WithMessage("Full name must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.FullName));

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[0-9]{10,15}$")
            .WithMessage("Invalid phone number.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.Bio)
            .MaximumLength(500)
            .WithMessage("Bio must not exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Bio));
    }
}
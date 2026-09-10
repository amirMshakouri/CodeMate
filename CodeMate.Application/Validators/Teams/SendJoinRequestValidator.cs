using CodeMate.Contracts.Teams.Requests;
using FluentValidation;

namespace CodeMate.Application.Validators.Teams;

public sealed class SendJoinRequestValidator
    : AbstractValidator<SendJoinRequest>
{
    public SendJoinRequestValidator()
    {
        RuleFor(x => x.Message)
            .MaximumLength(1000)
            .WithMessage("Join request message must not exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Message));
    }
}
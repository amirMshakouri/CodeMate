using CodeMate.Contracts.Teams.Requests;
using FluentValidation;

namespace CodeMate.Application.Validators.Teams;

public sealed class CreateTeamValidator
    : AbstractValidator<CreateTeamRequest>
{
    public CreateTeamValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Team name is required.");
    }
}
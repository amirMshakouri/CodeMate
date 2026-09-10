using CodeMate.Contracts.Teams.Requests;
using FluentValidation;

namespace CodeMate.Application.Validators.Teams;

public sealed class UpdateTeamValidator
    : AbstractValidator<UpdateTeamRequest>
{
    public UpdateTeamValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Team name is required.");
    }
}
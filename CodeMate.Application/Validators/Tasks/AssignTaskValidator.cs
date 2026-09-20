
using CodeMate.Contracts.Tasks.Requests;
using FluentValidation;

namespace CodeMate.Application.Validators.Tasks;

public sealed class AssignTaskValidator : AbstractValidator<AssignTaskRequest>
{
    public AssignTaskValidator()
    {
        RuleFor(x => x.AssignedUserId)
            .NotEmpty()
            .WithMessage("Assigned user ID is required.");
    }
}


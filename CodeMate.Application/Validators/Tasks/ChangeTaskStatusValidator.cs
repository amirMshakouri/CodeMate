using CodeMate.Contracts.Tasks.Requests;
using FluentValidation;

namespace CodeMate.Application.Validators.Tasks;

public sealed class ChangeTaskStatusValidator : AbstractValidator<ChangeTaskStatusRequest>
{
    public ChangeTaskStatusValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid task status.");
    }
}
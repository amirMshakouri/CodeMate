
using CodeMate.Contracts.Tasks.Requests;
using FluentValidation;

namespace CodeMate.Application.Validators.Tasks;

public sealed class UpdateTaskValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(150)
            .WithMessage("Title must not exceed 150 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Invalid task priority.");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTimeOffset.UtcNow)
            .When(x => x.DueDate.HasValue)
            .WithMessage("Due date must be in the future.");
    }
}


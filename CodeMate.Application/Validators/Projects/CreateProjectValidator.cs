using CodeMate.Contracts.Projects.Requests;
using FluentValidation;

namespace CodeMate.Application.Validators.Projects;

public sealed class CreateProjectValidator
    : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Project title is required.")
            .MinimumLength(3)
            .WithMessage("Project title must be at least 3 characters.")
            .MaximumLength(150)
            .WithMessage("Project title must not exceed 150 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Project description must not exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}
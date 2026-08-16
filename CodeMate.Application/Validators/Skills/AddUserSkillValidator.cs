using CodeMate.Contracts.Skills.Requests;
using CodeMate.Domain.Enums;
using FluentValidation;

namespace CodeMate.Application.Validators.Skills
{
    public sealed class AddUserSkillValidator : AbstractValidator<AddUserSkillRequest>
    {
        public AddUserSkillValidator()
        {
            RuleFor(x => x.SkillName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Level)
                .Must(level => Enum.IsDefined(typeof(SkillLevel), level))
                .WithMessage("Invalid skill level.");
        }
    }
}
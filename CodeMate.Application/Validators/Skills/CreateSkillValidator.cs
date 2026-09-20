using System;
using System.Collections.Generic;
using System.Text;

using CodeMate.Contracts.Skills.Requests;
using FluentValidation;

namespace CodeMate.Application.Validators.Skills;

public sealed class CreateSkillValidator : AbstractValidator<CreateSkillRequest>
{
    public CreateSkillValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Skill name is required.")
            .MaximumLength(100).WithMessage("Skill name must not exceed 100 characters.");
    }
}
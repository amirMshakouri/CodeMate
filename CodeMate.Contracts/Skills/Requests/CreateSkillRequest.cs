using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Contracts.Skills.Requests;

public sealed class CreateSkillRequest
{
    public required string Name { get; set; }
}
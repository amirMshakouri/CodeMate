using System;
using System.Collections.Generic;
using System.Text;

using AutoMapper;
using CodeMate.Contracts.Skills.Requests;
using CodeMate.Domain.Entities;

namespace CodeMate.Application.Mappings;

public sealed class AdminSkillMappingProfile : Profile
{
    public AdminSkillMappingProfile()
    {
        CreateMap<CreateSkillRequest, Skill>();
    }
}
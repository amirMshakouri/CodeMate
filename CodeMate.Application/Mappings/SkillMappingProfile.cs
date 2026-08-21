using AutoMapper;
using CodeMate.Contracts.Skills.Responses;
using CodeMate.Domain.Entities;

namespace CodeMate.Application.Mappings;

public sealed class SkillMappingProfile : Profile
{
    public SkillMappingProfile()
    {
        CreateMap<Skill, SkillResponse>();

        CreateMap<UserSkill, SkillResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.Skill.Id))
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => src.Skill.Name));
    }
}
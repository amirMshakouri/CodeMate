using AutoMapper;
using CodeMate.Contracts.Teams.Enums;
using CodeMate.Contracts.Teams.Responses;
using CodeMate.Domain.Entities;

namespace CodeMate.Application.Mappings;

public sealed class TeamQueryMappingProfile : Profile
{
    public TeamQueryMappingProfile()
    {
        CreateMap<Team, TeamCardResponse>();

        CreateMap<TeamMember, ProjectMemberResponse>()
            .ForMember(
                dest => dest.Role,
                opt => opt.MapFrom(src => (TeamMemberRoleResponse)src.Role));
    }
}
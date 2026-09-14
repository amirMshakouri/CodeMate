using AutoMapper;
using CodeMate.Contracts.Teams.Requests;
using CodeMate.Contracts.Teams.Responses;
using CodeMate.Domain.Entities;

namespace CodeMate.Application.Mappings;

public sealed class TeamCommandMappingProfile : Profile
{
    public TeamCommandMappingProfile()
    {
        CreateMap<Team, TeamResponse>();

        CreateMap<CreateTeamRequest, Team>();

        CreateMap<UpdateTeamRequest, Team>()
            .ForAllMembers(opt =>
                opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
using AutoMapper;
using CodeMate.Contracts.Teams.Enums;
using CodeMate.Contracts.Teams.Responses;
using CodeMate.Domain.Entities;

namespace CodeMate.Application.Mappings;

public sealed class JoinRequestQueryMappingProfile : Profile
{
    public JoinRequestQueryMappingProfile()
    {
        CreateMap<JoinRequest, JoinRequestResponse>()
            .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => (JoinRequestStatusResponse)src.Status));
    }
}
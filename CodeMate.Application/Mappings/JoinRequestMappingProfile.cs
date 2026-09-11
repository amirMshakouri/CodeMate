using AutoMapper;
using CodeMate.Contracts.Teams.Requests;
using CodeMate.Domain.Entities;

namespace CodeMate.Application.Mappings;

public sealed class JoinRequestMappingProfile : Profile
{
    public JoinRequestMappingProfile()
    {
        CreateMap<SendJoinRequest, JoinRequest>();
    }
}
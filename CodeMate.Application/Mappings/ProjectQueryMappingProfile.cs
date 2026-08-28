using AutoMapper;
using CodeMate.Contracts.Projects.Enums;
using CodeMate.Contracts.Projects.Responses;
using CodeMate.Domain.Entities;

namespace CodeMate.Application.Mappings;

public sealed class ProjectQueryMappingProfile : Profile
{
    public ProjectQueryMappingProfile()
    {
        CreateMap<Project, ProjectCardResponse>()
            .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => (ProjectStatusResponse)src.Status));

        CreateMap<Project, ProjectDetailsResponse>()
            .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => (ProjectStatusResponse)src.Status));
    }
}
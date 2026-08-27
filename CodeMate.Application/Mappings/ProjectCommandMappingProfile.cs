using AutoMapper;
using CodeMate.Contracts.Projects.Requests;
using CodeMate.Contracts.Projects.Responses;
using CodeMate.Domain.Entities;

namespace CodeMate.Application.Mappings;

public sealed class ProjectCommandMappingProfile : Profile
{
    public ProjectCommandMappingProfile()
    {
        CreateMap<Project, ProjectResponse>();

        CreateMap<CreateProjectRequest, Project>();

        CreateMap<UpdateProjectRequest, Project>()
            .ForAllMembers(opt =>
                opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
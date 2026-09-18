
using AutoMapper;
using CodeMate.Contracts.Tasks.Requests;
using CodeMate.Contracts.Tasks.Responses;
using CodeMate.Domain.Entities;

namespace CodeMate.Application.Mappings;

public sealed class TaskCommandMappingProfile : Profile
{
    public TaskCommandMappingProfile()
    {
        CreateMap<TaskItem, TaskResponse>();

        CreateMap<CreateTaskRequest, TaskItem>();

        CreateMap<UpdateTaskRequest, TaskItem>()
            .ForAllMembers(opt =>
                opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}


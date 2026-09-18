using AutoMapper;
using CodeMate.Contracts.Tasks.Enums;
using CodeMate.Contracts.Tasks.Responses;
using CodeMate.Domain.Entities;

namespace CodeMate.Application.Mappings;

public sealed class TaskQueryMappingProfile : Profile
{
    public TaskQueryMappingProfile()
    {
        CreateMap<TaskItem, TaskCardResponse>()
            .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => (TaskStatusResponse)src.Status))
            .ForMember(
                dest => dest.Priority,
                opt => opt.MapFrom(src => (TaskPriorityResponse)src.Priority));

        CreateMap<TaskItem, TaskDetailsResponse>()
            .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => (TaskStatusResponse)src.Status))
            .ForMember(
                dest => dest.Priority,
                opt => opt.MapFrom(src => (TaskPriorityResponse)src.Priority));
    }
}
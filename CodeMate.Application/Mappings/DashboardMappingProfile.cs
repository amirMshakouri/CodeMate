using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using CodeMate.Contracts.Dashboard.Responses;
using CodeMate.Domain.Entities;

namespace CodeMate.Application.Mappings;

public sealed class DashboardMappingProfile : Profile
{
    public DashboardMappingProfile()
    {
        CreateMap<TaskItem, MemberPerformanceResponse>()
            .ForMember(d => d.UserId, o => o.MapFrom(s => s.AssignedUserId));
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using CodeMate.Contracts.Admin.Users.Responses;
using CodeMate.Contracts.Users.Enums;
using CodeMate.Domain.Entities;

namespace CodeMate.Application.Mappings;

public sealed class AdminUserMappingProfile : Profile
{
    public AdminUserMappingProfile()
    {
        CreateMap<User, AdminUserListItemResponse>()
            .ForMember(
                dest => dest.Role,
                opt => opt.MapFrom(src => (UserRoleResponse)src.Role));

        CreateMap<User, AdminUserDetailsResponse>()
            .ForMember(
                dest => dest.Role,
                opt => opt.MapFrom(src => (UserRoleResponse)src.Role));
    }
}
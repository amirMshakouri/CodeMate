using AutoMapper;
using CodeMate.Contracts.Users.Requests;
using CodeMate.Contracts.Users.Responses;
using CodeMate.Domain.Entities;
using CodeMate.Contracts.Users.Enums;

namespace CodeMate.Application.Mappings;

public sealed class UserProfileMappingProfile : Profile
{
    public UserProfileMappingProfile()
    {        
        CreateMap<UpdateProfileRequest, User>()
                 .ForAllMembers(opt =>
                     opt.Condition((src, dest, srcMember) => srcMember != null));
       
        CreateMap<User, UserProfileResponse>()
            .ForMember(
                dest => dest.Role,
                opt => opt.MapFrom(src => (UserRoleResponse)src.Role));
        
        CreateMap<User, UserSummaryResponse>();
        
    }
}
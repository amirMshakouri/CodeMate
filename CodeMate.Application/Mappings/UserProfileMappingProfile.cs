using AutoMapper;
using CodeMate.Contracts.Users.Requests;
using CodeMate.Contracts.Users.Responses;
using CodeMate.Domain.Entities;

namespace CodeMate.Application.Mappings;

public sealed class UserProfileMappingProfile : Profile
{
    public UserProfileMappingProfile()
    {
        CreateMap<User, UserProfileResponse>();

        CreateMap<User, UserSummaryResponse>();

        CreateMap<UpdateProfileRequest, User>()
            .ForAllMembers(opt =>
                opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
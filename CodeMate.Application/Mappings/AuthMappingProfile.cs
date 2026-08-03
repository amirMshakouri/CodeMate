using AutoMapper;
using CodeMate.Contracts.Auth.Requests;
using CodeMate.Contracts.Auth.Responses;
using CodeMate.Domain.Entities;

namespace CodeMate.Application.Mappings;

public sealed class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<RegisterRequest, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.SecondaryPassword, opt => opt.Ignore());

        CreateMap<User, RegisterResponse>();

        CreateMap<User, LoginResponse>()
            .ForMember(dest => dest.Token, opt => opt.Ignore())
            .ForMember(dest => dest.Expiration, opt => opt.Ignore());
    }
}
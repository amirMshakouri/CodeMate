using AutoMapper;
using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Users.Requests;
using CodeMate.Contracts.Users.Responses;
using CodeMate.Domain.Entities;
using CodeMate.Shared.Exceptions;

namespace CodeMate.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserProfileResponse> GetProfileAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        return _mapper.Map<UserProfileResponse>(user);
    }

    public async Task UpdateProfileAsync(
        Guid userId,
        UpdateProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        _mapper.Map(request, user);

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();
    }
}
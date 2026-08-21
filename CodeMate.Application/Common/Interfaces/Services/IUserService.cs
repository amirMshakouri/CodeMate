using CodeMate.Contracts.Users.Requests;
using CodeMate.Contracts.Users.Responses;

namespace CodeMate.Application.Common.Interfaces.Services;

public interface IUserService
{
    Task<UserProfileResponse> GetProfileAsync(Guid userId);

    Task UpdateProfileAsync(
        Guid userId,
        UpdateProfileRequest request);
}
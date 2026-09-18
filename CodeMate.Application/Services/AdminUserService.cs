using AutoMapper;
using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Admin.Users.Requests;
using CodeMate.Contracts.Admin.Users.Responses;
using CodeMate.Contracts.Common.Pagination;
using CodeMate.Domain.Enums;
using CodeMate.Shared.Exceptions;

namespace CodeMate.Application.Services;

public sealed class AdminUserService : IAdminUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public AdminUserService(
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<PaginationResponse<AdminUserListItemResponse>> SearchUsersAsync(
        SearchUsersRequest request)
    {
        var role = request.Role.HasValue
            ? (UserRole?)request.Role.Value
            : null;

        var users = await _userRepository.SearchAsync(
            request.SearchTerm,
            role,
            request.IsActive,
            request.PageNumber,
            request.PageSize);

        return new PaginationResponse<AdminUserListItemResponse>
        {
            Items = _mapper.Map<IEnumerable<AdminUserListItemResponse>>(users),
            PageNumber = users.PageNumber,
            PageSize = users.PageSize,
            TotalCount = users.TotalCount,
            TotalPages = users.TotalPages
        };
    }

    public async Task<AdminUserDetailsResponse> GetUserDetailsAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        return _mapper.Map<AdminUserDetailsResponse>(user);
    }

    public async Task DeactivateUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        if (user.Id == _currentUserService.UserId)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["UserId"] = new[] { "You cannot deactivate your own account." }
            });
        }

        user.IsActive = false;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task ActivateUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        user.IsActive = true;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task PromoteToAdminAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        user.Role = UserRole.Admin;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using CodeMate.Contracts.Admin.Users.Requests;
using CodeMate.Contracts.Admin.Users.Responses;
using CodeMate.Contracts.Common.Pagination;

namespace CodeMate.Application.Common.Interfaces.Services;

public interface IAdminUserService
{
    Task<PaginationResponse<AdminUserListItemResponse>> SearchUsersAsync(
        SearchUsersRequest request);

    Task<AdminUserDetailsResponse> GetUserDetailsAsync(Guid userId);

    Task DeactivateUserAsync(Guid userId);

    Task ActivateUserAsync(Guid userId);

    Task PromoteToAdminAsync(Guid userId);
}
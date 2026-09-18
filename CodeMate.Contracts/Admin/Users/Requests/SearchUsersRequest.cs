using System;
using System.Collections.Generic;
using System.Text;

using CodeMate.Contracts.Common.Pagination;
using CodeMate.Contracts.Users.Enums;

namespace CodeMate.Contracts.Admin.Users.Requests;

public class SearchUsersRequest : PaginationRequest
{
    public string? SearchTerm { get; set; }
    public UserRoleResponse? Role { get; set; }
    public bool? IsActive { get; set; }
}
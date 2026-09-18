using System;
using System.Collections.Generic;
using System.Text;

using CodeMate.Contracts.Users.Enums;

namespace CodeMate.Contracts.Admin.Users.Responses;

public class AdminUserDetailsResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public UserRoleResponse Role { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
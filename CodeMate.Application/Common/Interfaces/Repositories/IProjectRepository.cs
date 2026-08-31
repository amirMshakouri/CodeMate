using CodeMate.Domain.Entities;
using CodeMate.Domain.Enums;
using CodeMate.Shared.Pagination;

namespace CodeMate.Application.Common.Interfaces.Repositories;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id);
    
    Task<PaginatedList<Project>> SearchAsync(
        string? title,
        ProjectStatus? status,
        int pageNumber,
        int pageSize);
    
}
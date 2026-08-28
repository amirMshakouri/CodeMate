using CodeMate.Contracts.Common.Pagination;
using CodeMate.Contracts.Projects.Enums;

namespace CodeMate.Contracts.Projects.Requests;

public class SearchProjectRequest : PaginationRequest
{
    public string? Title { get; set; }
    public ProjectStatusResponse? Status { get; set; }
}
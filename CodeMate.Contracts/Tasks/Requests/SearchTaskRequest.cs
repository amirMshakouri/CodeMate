using CodeMate.Contracts.Common.Pagination;
using CodeMate.Contracts.Tasks.Enums;

namespace CodeMate.Contracts.Tasks.Requests;

public class SearchTaskRequest : PaginationRequest
{
    public Guid? ProjectId { get; set; }

    public TaskStatusResponse? Status { get; set; }

    public Guid? AssignedUserId { get; set; }
}
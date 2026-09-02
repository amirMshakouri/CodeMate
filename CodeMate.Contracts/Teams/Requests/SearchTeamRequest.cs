using CodeMate.Contracts.Common.Pagination;

namespace CodeMate.Contracts.Teams.Requests;

public sealed class SearchTeamRequest : PaginationRequest
{
    public string? Name { get; set; }
}
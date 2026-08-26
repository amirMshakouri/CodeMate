namespace CodeMate.Contracts.Common.Pagination;

public class PaginationResponse<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}
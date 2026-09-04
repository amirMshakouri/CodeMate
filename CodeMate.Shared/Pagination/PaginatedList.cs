namespace CodeMate.Shared.Pagination;

public class PaginatedList<T> : List<T>
{
    public int PageNumber { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public int TotalPages { get; }

    public PaginatedList(
        IEnumerable<T> items,
        int totalCount,
        int pageNumber,
        int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        AddRange(items);
    }
}
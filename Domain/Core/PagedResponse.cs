namespace Domain.Core;

public abstract class PagedResponse<T>
{
    protected PagedResponse(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = [.. items];
        TotalCount = totalCount;
        PageSize = pageSize;
        PageNumber = pageNumber;
    }

    public IReadOnlyCollection<T> Items { get; } = [];
    public int TotalCount { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public bool HasNextPage => PageNumber * PageSize < TotalCount;
    public bool HasPreviousPage => PageNumber > 1;

}

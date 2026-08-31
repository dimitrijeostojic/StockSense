namespace Domain.Core;

public abstract class PagedRequest
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public bool IsAscending { get; set; }
}

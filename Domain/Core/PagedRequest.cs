namespace Domain.Core;

public abstract record PagedRequest(int PageNumber, int PageSize);

namespace Plates.Shared;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasNextPage => PageNumber < TotalPages;

    public bool HasPreviousPage => PageNumber > 1;

    public string SortColumn { get; set; }
    public string SortDirection { get; set; }
    public string Filter { get; set; }  
}

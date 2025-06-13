namespace WebMVC.ViewModels;

public class PagedViewModel<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;

    public string SortColumn { get; set; }
    public string SortDirection { get; set; }
    public string Filter { get; set; }
}

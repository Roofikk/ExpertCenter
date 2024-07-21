namespace ExpertCenter.MvcApp.Models;

public class PaginationBarModel
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string ActionName { get; set; } = "Index";
    public string ControllerName { get; set; } = "Home";
    public Dictionary<string, string> RouteValues { get; set; } = [];
}

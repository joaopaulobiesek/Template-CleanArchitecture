namespace Template.Application.Common.Models;

public abstract class BasePaginatedQuery
{
    public Guid ExpoId { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int AscDesc { get; set; }
    public string? ColumnName { get; set; }
    public string? SearchText { get; set; }
    public Dictionary<string, string>? CustomFilter { get; set; }
}
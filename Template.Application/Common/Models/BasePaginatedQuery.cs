namespace Template.Application.Common.Models;


public abstract class BasePaginatedQuery
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int AscDesc { get; set; }
    public string? ColumnName { get; set; }
    public string? Src { get; set; }
    public string? CustomFilter { get; set; }
}
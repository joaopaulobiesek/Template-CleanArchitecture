using Newtonsoft.Json;

namespace Template.Application.Common.Models;


public abstract class BasePaginatedQuery
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int AscDesc { get; set; }
    public string? ColumnName { get; set; }
    public string? Src { get; set; }
    public string? CustomFilter { get; set; }

    /// <summary>
    /// Deserializa CustomFilter para Dictionary ou retorna null se inválido
    /// </summary>
    public Dictionary<string, string>? GetCustomFilterDictionary()
    {
        if (string.IsNullOrWhiteSpace(CustomFilter))
            return null;

        try
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(CustomFilter);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
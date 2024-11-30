namespace Template.Application.Common.Models;

public class PaginatedList<T> : ApiResponse<IEnumerable<T>>
{
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalItens { get; }
    public bool HasPagePrevious => PageNumber > 1;
    public bool TemNextPage => PageNumber < TotalPages;

    public PaginatedList(IReadOnlyCollection<T>? items, int count, int pageNumber, int pageSize, string mensagem = "") : base(true, mensagem, items)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalItens = count;
    }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}
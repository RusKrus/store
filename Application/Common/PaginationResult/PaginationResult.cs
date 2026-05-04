namespace Application.Common.PaginationResult;

public class PaginationResult<T>
{
    public int Total { get; init; }
    public List<T> Items { get; init; } = new();
}
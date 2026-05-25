namespace Store.API.Contracts.Response;

public class PaginationResponse<T>(int total, List<T> items)
{
    public int Total { get; init; } = total;
    public List<T> Items { get; init; } = items;
}
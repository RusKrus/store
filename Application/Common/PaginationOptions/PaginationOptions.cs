namespace Application.Common.PaginationOptions;

public sealed class PaginationOptions
{
    public int Offset { get; }
    public int Limit { get; }

    private PaginationOptions(int offset, int limit)
    {
        Offset = offset;
        Limit = limit;
    }

    public static PaginationOptions Create(int page, int perPage, bool showAll)
    {
        if (showAll) return new PaginationOptions(0, int.MaxValue);

        var normalizedSize = Math.Clamp(perPage, 1, 100);
        var normalizedPage = (Math.Max(page, 1) - 1) * normalizedSize;
        return new PaginationOptions(normalizedPage, normalizedSize);
    }
}
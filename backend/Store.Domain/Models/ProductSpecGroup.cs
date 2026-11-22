namespace Store.Domain.Models;

public class ProductSpecGroup
{
    public string BlockName { get; init; } = "";
    public List<ProductSpecItem> Specs { get; init; } = new();
}

public class ProductSpecItem
{
    public string SpecName { get; init; } = "";
    public string SpecDescription { get; init; } = "";
}
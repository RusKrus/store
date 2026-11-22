using System.ComponentModel.DataAnnotations;

namespace Store.Infrastructure.ObjectStorage;

public sealed class ObjectStorageOptions
{
    public const string NameSpace = "ObjectStorage";

    [Required] public string Endpoint { get; init; } = "";
    [Required] public string Region { get; init; } = "";
    [Required] public string Bucket { get; init; } = "";
    [Required] public string AccessKey { get; init; } = "";
    [Required] public string SecretKey { get; init; } = "";
}
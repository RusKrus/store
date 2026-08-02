using System.ComponentModel.DataAnnotations;

namespace Store.Infrastructure.RabbitMq.Topology;

public sealed record OutboxBackgroundServiceOptions
{
    public const string SectionName = "OutboxPublisher";

    [Required] [Range(5000, double.MaxValue)]
    public double BaseRetryInterval { get; init; }

    [Required] [Range(1, int.MaxValue)]
    public int BatchSize { get; init; }

    [Required] [Range(1, int.MaxValue)]
    public int MaxAttempts { get; init; }  

    [Required] [Range(1000, int.MaxValue)]
    public int PublishDelay { get; init; } 
};
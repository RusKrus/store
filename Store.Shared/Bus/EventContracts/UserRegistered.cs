namespace Store.Infrastructure.RabbitMq.Publishers.EventContracts;

public sealed record UserRegistered(int Id, string UserName, string Email, DateTime RegisteredAt);
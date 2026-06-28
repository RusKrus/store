namespace Store.Shared.Bus.EventContracts;

public sealed record UserRegistered(int Id, string UserName, string Email, DateTime RegisteredAt);
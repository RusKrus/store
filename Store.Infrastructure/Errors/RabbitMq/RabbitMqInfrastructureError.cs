namespace Store.Infrastructure.Errors.RabbitMq;

public class RabbitMqInfrastructureError(string message) : Exception(message){}
namespace Store.Shared.Bus;

public static class RabbitMqConstants
{
    public static class Exchange
    {
        public static readonly string StoreEvents = "store-events";
    }

    public static class Queue
    {
        public static readonly string MailServiceStoreEvents = "mail-service.store-events";
    }

    public static class RoutingKey
    {
        public static readonly string StoreUserCreated = "store-user-created";
    }
}
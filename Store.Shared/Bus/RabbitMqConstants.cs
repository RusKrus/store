namespace Store.Shared.Bus;

public static class RabbitMqConstants
{
    public static class Exchange
    {
        public static readonly string StoreEvents = "store-events";

        public static readonly string StoreNotificationsRetry = "store-notifications-retry";
        public static readonly string StoreNotificationsRedelivery = "store-notifications-redelivery";
        public static readonly string StoreNotificationsDead = "store-notifications-dead";

        public static readonly string StoreMailRetry = "store-mail-retry";
        public static readonly string StoreMailRedelivery = "store-mail-redelivery";
        public static readonly string StoreMailDead = "store-mail-dead";
    }

    public static class Queue
    {
        public static readonly string MailServiceStoreEvents = "mail-service.store-events";
        public static readonly string MailServiceRetry = "mail-service.retry";
        public static readonly string MailServiceDead = "mail-service.dead";

        public static readonly string NotificationsServiceStoreEvents = "notifications-service.store-events";
        public static readonly string NotificationsServiceRetry = "notifications-service.retry";
        public static readonly string NotificationsDead = "notifications.dead";
    }

    public static class RoutingKey
    {
        public static readonly string StoreUserCreated = "store-user-created";

        public static readonly string NotificationsServiceRetry = "notifications-service-retry";
        public static readonly string NotificationsDead = "notifications-dead";

        public static readonly string MailServiceRetry = "mail-service-retry";
        public static readonly string MailServiceDead = "mail-service-dead";
    }
}
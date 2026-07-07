# RabbitMQ structure notes

Эта заметка описывает, какой структуры придерживаться для RabbitMQ в текущем решении.

## Общая идея

RabbitMQ лучше держать не в одном общем месте, а разделять по зонам ответственности:

- `Store.Shared` - общие имена exchange/routing key и контракты событий.
- `Application` - абстракции, которые нужны бизнес-слою.
- `Store.Infrastructure` - реализация публикации событий из основного API.
- `Store.MailService.Service` - consumer, его queue, binding и обработка сообщений.

Publisher должен знать exchange и routing key. Consumer должен владеть своей queue и binding.

## Текущая карта файлов

```text
Store.Shared/
  Bus/
    RabbitMqConstants.cs
    EventContracts/
      UserRegistered.cs

Application/
  Interfaces/
    IRabbitMqPublisher.cs

Store.Infrastructure/
  RabbitMq/
    Publishers/
      Publisher.cs
    Topology/
      Options.cs
      ExchangeDeclaration.cs
  DependencyInjection.cs

Store.API/
  Program.cs
  appsettings.json
  appsettings.Development.json

Store.MailService.Service/
  RabbitMq/
    Consumers/
      UserRegisteredConsumer.cs
    Topology/
      Options.cs
      Declaration.cs
  Program.cs
  appsettings.json
  appsettings.Development.json
```

## Что где хранить

### `Store.Shared/Bus`

Здесь лежит то, что должны одинаково знать publisher и consumers:

- имена exchange;
- routing keys;
- контракты событий.

Пример:

```text
Store.Shared/Bus/RabbitMqConstants.cs
Store.Shared/Bus/EventContracts/UserRegistered.cs
```

Сюда не стоит класть настройки подключения, `IConnection`, `IChannel`, declarations очередей или consumer-логику.

### `Application/Interfaces`

Здесь лежит порт для бизнес-слоя:

```text
Application/Interfaces/IRabbitMqPublisher.cs
```

Application не должен знать про `RabbitMQ.Client`, `ConnectionFactory`, channels, exchange declaration и конкретную сериализацию.

### `Store.Infrastructure/RabbitMq`

Здесь живет RabbitMQ-инфраструктура основного API:

```text
Store.Infrastructure/RabbitMq/Publishers/Publisher.cs
Store.Infrastructure/RabbitMq/Topology/Options.cs
Store.Infrastructure/RabbitMq/Topology/ExchangeDeclaration.cs
```

Ответственность:

- создать singleton `IConnection`;
- объявить общий exchange, в который API публикует события;
- реализовать `IRabbitMqPublisher`;
- создавать `IChannel` локально на операцию публикации.

Очереди consumer-сервисов здесь объявлять не нужно. API не должен владеть queue mail-сервиса.

### `Store.MailService.Service/RabbitMq`

Здесь живет все, что относится именно к mail consumer:

```text
Store.MailService.Service/RabbitMq/Consumers/UserRegisteredConsumer.cs
Store.MailService.Service/RabbitMq/Topology/Options.cs
Store.MailService.Service/RabbitMq/Topology/Declaration.cs
```

Ответственность:

- создать singleton `IConnection` для MailService;
- создать long-lived `IChannel` внутри hosted consumer;
- объявить свою queue;
- привязать queue к exchange через нужный routing key;
- читать сообщения через `BasicConsumeAsync`;
- создавать scope на каждое сообщение для scoped-сервисов вроде `IMailService`.

Файл `Declaration.cs` по содержанию сейчас является `MailServiceTopology`. Для читаемости его позже лучше переименовать в `MailServiceTopology.cs`.

## Рекомендуемый шаблон для нового события

Если появляется новое событие, порядок такой:

1. Добавить контракт события в `Store.Shared/Bus/EventContracts`.
2. Добавить routing key в `Store.Shared/Bus/RabbitMqConstants.cs`.
3. Добавить mapping события к routing key в `Store.Infrastructure/RabbitMq/Publishers/Publisher.cs`.
4. В нужном consumer-сервисе добавить queue/binding в его `RabbitMq/Topology`.
5. В consumer-сервисе добавить обработчик или расширить существующий `BackgroundService`.

## Рекомендуемый шаблон для нового consumer-сервиса

```text
Some.Consumer.Service/
  RabbitMq/
    Consumers/
      SomeEventConsumer.cs
    Topology/
      SomeServiceTopology.cs
      Options.cs
  Program.cs
  appsettings.json
  appsettings.Development.json
```

В `Program.cs` такого сервиса обычно регистрируются:

- RabbitMQ options из секции `Rabbit`;
- singleton `IConnection`;
- singleton topology helper;
- scoped обработчики бизнес-логики;
- hosted consumer через `AddHostedService`.

## Правила, чтобы не путаться

- `IConnection` - singleton на сервис.
- `IChannel` - не singleton в DI; создавать локально под workflow.
- Exchange - общий контракт публикации.
- Queue - собственность consumer-сервиса.
- Binding - собственность consumer-сервиса.
- Event contract - общий контракт между publisher и consumer.
- Routing key - общий ключ маршрутизации, хранится рядом с RabbitMQ constants.
- Consumer не должен использовать scoped-сервисы напрямую в конструкторе `BackgroundService`; нужен scope на сообщение.
- В каждом сервисе, который подключается к RabbitMQ, в `appsettings*.json` должна быть полная секция `Rabbit`: `Host`, `Username`, `Password`, `VirtualHost`.


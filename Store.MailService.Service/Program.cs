using Store.MailService.Service.RabbitMq.Consumer;
using Store.MailService.Service.Configuration;
using Store.MailService.Service.Services.Implementation;
using Store.MailService.Service.Services.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Store.MailService.Service.RabbitMq.Topology;
using Options = Store.MailService.Service.RabbitMq.Topology.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<Options>(builder.Configuration.GetSection("Rabbit"));


builder.Services.AddSingleton<IConnection>(opt =>
{
    var options = opt.GetRequiredService<IOptions<Options>>().Value;
    var connection = new ConnectionFactory
    {
        HostName = options.Host,
        UserName = options.Username,
        Password = options.Password,
        VirtualHost = options.VirtualHost
    };

    return connection.CreateConnectionAsync().GetAwaiter().GetResult();
});

builder.Services.AddSingleton<MailServiceTopology>();
builder.Services.AddScoped<IMailService, MailServiceImpl>();
builder.Services.AddHostedService<RabbitMqConsumer>();

var host = builder.Build();
host.Run();
using System.Reflection;
using FluentMigrator.Runner;
using Store.MailService.Service.RabbitMq.Consumer;
using Store.MailService.Service.Configuration;
using Store.MailService.Service.Services.Implementation;
using Store.MailService.Service.Services.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Store.MailService.Service.Persistence;
using Store.MailService.Service.RabbitMq.Topology;
using Options = Store.MailService.Service.RabbitMq.Topology.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

#region RabbitMq
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

builder.Services.AddSingleton<DeadTopologyDeclaration>();
builder.Services.AddSingleton<RetryTopologyDeclaration>();
builder.Services.AddSingleton<MailServiceTopology>();
builder.Services.AddHostedService<RabbitMqConsumer>();
#endregion


#region Sqlite
builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddSQLite()
        .WithGlobalConnectionString(builder.Configuration.GetValue<string>("Dapper:SQLiteConnString"))
        .ScanIn(Assembly.GetExecutingAssembly())
        .For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());

builder.Services.AddSingleton<SqliteConnectionFactory>();
builder.Services.AddScoped<DatabaseMigrator>();
builder.Services.AddScoped<SqliteInitializer>();
#endregion

builder.Services.AddScoped<IMailService, MailServiceImpl>();

var host = builder.Build();

using (var scope = host.Services.CreateScope()) 
{
    var initializer = scope.ServiceProvider.GetRequiredService<SqliteInitializer>();
    initializer.Initialize();

    var migrator = scope.ServiceProvider.GetRequiredService<DatabaseMigrator>();
    migrator.Migrate();
}


host.Run();
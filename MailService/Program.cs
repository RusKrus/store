using MailService.Comsumers;
using MailService.Configuration;
using MailService.Services.Implementation;
using MailService.Services.Interfaces;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddScoped<IMailService, MailServiceImpl>();

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<UserRegisteredConsumer>();
    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["Rabbit:Host"],
            rhconf =>
            {
                rhconf.Username(builder.Configuration["Rabbit:Username"]);
                rhconf.Password(builder.Configuration["Rabbit:Password"]);
            });
        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();
host.Run();
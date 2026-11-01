using Store.Notifications.Infrastructure;
using Store.Notifications.Infrastructure.Persistance;
using Store.NotificationsAPI.Features;
using Store.NotificationsAPI.Features.Notifications;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddPostgres(builder.Configuration);
builder.Services.AddRabbitMq(builder.Configuration);
builder.Services.MapFeatureFunctions();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.MapFeatureEndpoints();
await app.Services.MigrateAsync();
app.UseHttpsRedirection();

app.Run();
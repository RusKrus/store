using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Handlers;
using Application.Handlers.Auth;
using Application.Handlers.Users;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Store.Api.Middlewares;
using Store.Application.Interfaces;
using Store.AutoMapperTypeConverters;
using Store.Infrastructure.Auth;
using Store.Infrastructure.Persistence;
using Store.Infrastructure.Services.BackgroundServices;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy =
            JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddHostedService<CartCleaningBackgroundService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(_ => { }, typeof(AutoMapperProfileConfiguration));

builder.Services.AddDbContext<StoreContext>();
builder.Services.AddPostgresRepositories();
builder.Services.AddPostgres(builder.Configuration);

builder.Services.AddHasherService();
builder.Services.AddJwtService();
builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
builder.Services.AddCartCookiesService();
builder.Services.AddEventBus(builder.Configuration);

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Store API",
        Version = "v1",
        Description = "API store description"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document),
            []
        }
    });

    options.UseInlineDefinitionsForEnums();

    options.DescribeAllParametersInCamelCase();
});

#region handlers
var handlers = typeof(CreateProductHandler)
    .Assembly
    .GetTypes()
    .Where(t =>
        t.Namespace is not null &&
        t.Namespace.Contains("Handlers"));

foreach (var handler in handlers) {
    builder.Services.AddScoped(handler);
}
#endregion

#region auth
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetSection("JwtIssuer").Value,
        ValidAudience = builder.Configuration.GetSection("JwtAudience").Value,
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtKey").Value))
    };
});
builder.Services.AddAuthorization();
#endregion

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
//     using var scope = app.Services.CreateScope();
//     var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
//     mapper.ConfigurationProvider.AssertConfigurationIsValid();
// }
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
await app.Services.MigrateDbAsync();

app.Run();

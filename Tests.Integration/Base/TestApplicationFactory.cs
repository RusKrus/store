using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace Tests.Integration.Base;

public class TestApplicationFactory(string psqlConnectionString, string rabbitMqConnectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTesting");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Postgres"] = psqlConnectionString,
                ["connectionStrings:Rabbit"] = rabbitMqConnectionString,
            });
        });

        builder.ConfigureTestServices(services =>
        {

        });

        base.ConfigureWebHost(builder);
    }
}
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using DistribuidoraFlores.Api.Infrastructure.Persistence;
using DistribuidoraFlores.Api.Modules.Identidade.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Identidade.Domain;

namespace DistribuidoraFlores.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
{
    private readonly SqliteConnection _connection;

    public CustomWebApplicationFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });
    }

    // Gera um HttpClient já com um token JWT válido no header Authorization,
    // sem precisar passar pelo fluxo real de registro/login.
    public HttpClient CreateAuthenticatedClient(Role role, Guid? clienteId = null)
    {
        var client = CreateClient();

        using var scope = Services.CreateScope();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

        var usuario = role == Role.Admin
            ? Usuario.CriarAdmin("admin-teste@teste.com", "hash-nao-usado")
            : Usuario.CriarComerciante("comerciante-teste@teste.com", "hash-nao-usado", clienteId ?? Guid.NewGuid());

        var token = tokenService.GerarToken(usuario);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection.Close();
    }
}
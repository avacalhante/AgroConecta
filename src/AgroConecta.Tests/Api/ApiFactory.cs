using AgroConecta.Infrastructure.Persistence;
using AgroConecta.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace AgroConecta.Tests.Api;

/// <summary>Sobe a API em memória, com banco InMemory e seed de desenvolvimento.</summary>
public class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _nomeBanco = $"agroconecta-api-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove o provider SQL Server registrado por AddInfrastructure para
            // que apenas o provider InMemory permaneça no contêiner de testes.
            services.RemoveAll<IDbContextOptionsConfiguration<AgroConectaDbContext>>();
            services.RemoveAll<DbContextOptions<AgroConectaDbContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<AgroConectaDbContext>();

            services.AddDbContext<AgroConectaDbContext>(options =>
                options.UseInMemoryDatabase(_nomeBanco));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var escopo = host.Services.CreateScope();
        var contexto = escopo.ServiceProvider.GetRequiredService<AgroConectaDbContext>();
        SeedDados.AplicarAsync(contexto).GetAwaiter().GetResult();

        return host;
    }
}

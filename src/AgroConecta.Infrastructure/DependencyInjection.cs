using AgroConecta.Application.Auth;
using AgroConecta.Application.Consultas;
using AgroConecta.Application.Indicadores;
using AgroConecta.Application.Relatorios;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Infrastructure.Persistence;
using AgroConecta.Infrastructure.Repositorios;
using AgroConecta.Infrastructure.Seguranca;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AgroConecta.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        JwtConfiguracao jwtConfiguracao)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        ArgumentNullException.ThrowIfNull(jwtConfiguracao);

        services.AddDbContext<AgroConectaDbContext>(options =>
            options.UseSqlServer(connectionString, sqlServer =>
            {
                sqlServer.MigrationsAssembly(typeof(AgroConectaDbContext).Assembly.FullName);
                sqlServer.EnableRetryOnFailure(3);
            }));

        services.AddScoped<IProdutoRepositorio, ProdutoRepositorio>();
        services.AddScoped<IPedidoRepositorio, PedidoRepositorio>();
        services.AddScoped<ILoteRepositorio, LoteRepositorio>();
        services.AddScoped<IProdutorRepositorio, ProdutorRepositorio>();
        services.AddScoped<ICooperativaRepositorio, CooperativaRepositorio>();
        services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
        services.AddScoped<IEntregaRepositorio, EntregaRepositorio>();
        services.AddScoped<IIndicadoresRepositorio, IndicadoresRepositorio>();
        services.AddScoped<IRelatoriosRepositorio, RelatoriosRepositorio>();
        services.AddScoped<IConsultasWebRepositorio, ConsultasWebRepositorio>();
        services.AddScoped<IRotaRepositorio, RotaRepositorio>();
        services.AddScoped<IFaturaRepositorio, FaturaRepositorio>();
        services.AddScoped<IUnidadeDeTrabalho, UnidadeDeTrabalho>();

        services.AddSingleton<ISenhaHasher, SenhaHasherAdapter>();
        services.AddSingleton<IGeradorToken>(_ => new GeradorTokenJwt(jwtConfiguracao));

        return services;
    }
}

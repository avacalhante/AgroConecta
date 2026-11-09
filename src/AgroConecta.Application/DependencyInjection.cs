using AgroConecta.Application.Auth;
using AgroConecta.Application.Common;
using AgroConecta.Application.Pedidos;
using AgroConecta.Application.Produtos;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AgroConecta.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CriarProdutoComandoValidator>(
            ServiceLifetime.Scoped);

        services.AddScoped<ICasoDeUso<ListarProdutosConsulta, Domain.Common.ResultadoPaginado<ProdutoDto>>, ListarProdutosCasoDeUso>();
        services.AddScoped<ICasoDeUso<CriarProdutoComando, ProdutoDto>, CriarProdutoCasoDeUso>();
        services.AddScoped<ICasoDeUso<int, ProdutoDto>, ObterProdutoCasoDeUso>();

        services.AddScoped<ICasoDeUso<ListarPedidosConsulta, Domain.Common.ResultadoPaginado<PedidoDto>>, ListarPedidosCasoDeUso>();
        services.AddScoped<ICasoDeUso<CriarPedidoComando, PedidoDto>, CriarPedidoCasoDeUso>();
        services.AddScoped<ICasoDeUso<AlterarStatusPedidoComando, PedidoDto>, AlterarStatusPedidoCasoDeUso>();
        services.AddScoped<ICasoDeUso<int, PedidoDto>, ObterPedidoCasoDeUso>();

        services.AddScoped<ICasoDeUso<LoginComando, LoginResultado>, AutenticarCasoDeUso>();
        services.AddScoped<ICasoDeUso<RenovarTokenComando, LoginResultado>, RenovarTokenCasoDeUso>();
        services.AddScoped<ICasoDeUso<int, UsuarioDto>, ObterUsuarioAtualCasoDeUso>();

        return services;
    }
}

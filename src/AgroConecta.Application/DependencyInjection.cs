using AgroConecta.Application.Auth;
using AgroConecta.Application.Common;
using AgroConecta.Application.Cooperativas;
using AgroConecta.Application.Entregas;
using AgroConecta.Application.Indicadores;
using AgroConecta.Application.Lotes;
using AgroConecta.Application.Pedidos;
using AgroConecta.Application.Produtores;
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

        services.AddScoped<ICasoDeUso<ListarLotesConsulta, Domain.Common.ResultadoPaginado<LoteDto>>, ListarLotesCasoDeUso>();
        services.AddScoped<ICasoDeUso<ListarMeusLotesConsulta, Domain.Common.ResultadoPaginado<LoteDto>>, ListarMeusLotesCasoDeUso>();
        services.AddScoped<ICasoDeUso<int, LoteDto>, ObterLoteCasoDeUso>();
        services.AddScoped<ICasoDeUso<CriarLoteComando, LoteDto>, CriarLoteCasoDeUso>();

        services.AddScoped<ICasoDeUso<ListarMinhasEntregasConsulta, IReadOnlyCollection<EntregaDto>>, ListarMinhasEntregasCasoDeUso>();
        services.AddScoped<ICasoDeUso<AlterarStatusEntregaComando, EntregaDto>, AlterarStatusEntregaCasoDeUso>();
        services.AddScoped<ICasoDeUso<RegistrarEvidenciaComando, EntregaDto>, RegistrarEvidenciaCasoDeUso>();

        services.AddScoped<ICasoDeUso<bool, IndicadoresDto>, ObterIndicadoresCasoDeUso>();

        services.AddScoped<ICasoDeUso<AprovarProdutorComando, bool>, AprovarProdutorCasoDeUso>();
        services.AddScoped<ICasoDeUso<CriarCooperativaComando, int>, CriarCooperativaCasoDeUso>();

        return services;
    }
}

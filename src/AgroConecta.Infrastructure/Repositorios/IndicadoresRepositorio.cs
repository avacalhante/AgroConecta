using AgroConecta.Application.Indicadores;
using AgroConecta.Domain.Enums;
using AgroConecta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgroConecta.Infrastructure.Repositorios;

public class IndicadoresRepositorio(AgroConectaDbContext contexto) : IIndicadoresRepositorio
{
    /// <summary>
    /// Percentual de telas acessíveis verificado no checklist WCAG (A10).
    /// Enquanto o checklist não é automatizado, o valor é fixo em 100.
    /// </summary>
    private const double TelasAcessiveisPercentual = 100;

    public async Task<IndicadoresDto> ObterAsync(
        DateTime referencia,
        CancellationToken cancellationToken = default)
    {
        var inicioMes = new DateTime(referencia.Year, referencia.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var pedidosMes = await contexto.Pedidos
            .CountAsync(pedido => pedido.DataPedido >= inicioMes, cancellationToken);

        var entregas = await contexto.Entregas
            .AsNoTracking()
            .Select(entrega => new
            {
                entrega.Status,
                entrega.DataPrevista,
                entrega.DataRealizada,
                TemEvidencia = entrega.FotoUrl != null,
            })
            .ToListAsync(cancellationToken);

        var entregues = entregas
            .Where(entrega => entrega.Status == StatusEntrega.Entregue)
            .ToArray();

        var noPrazo = entregues.Count(entrega =>
            entrega.DataRealizada is not null
            && DateOnly.FromDateTime(entrega.DataRealizada.Value) <= entrega.DataPrevista);

        var totalEntregas = entregas.Count;
        var comEvidencia = entregas.Count(entrega => entrega.TemEvidencia);

        var movimentos = await contexto.MovimentosEstoque
            .AsNoTracking()
            .Select(movimento => new { movimento.Tipo, movimento.Quantidade })
            .ToListAsync(cancellationToken);

        // Perdas são registradas como ajustes de estoque negativos.
        var perdas = movimentos.Count(movimento =>
            movimento.Tipo == TipoMovimentoEstoque.Ajuste && movimento.Quantidade < 0);

        var produtoresAtivos = await contexto.Produtores
            .CountAsync(produtor => produtor.StatusCadastro == StatusCadastro.Aprovado, cancellationToken);

        var cooperativasAtivas = await contexto.Cooperativas
            .CountAsync(cooperativa => cooperativa.Ativa, cancellationToken);

        return new IndicadoresDto(
            pedidosMes,
            Percentual(noPrazo, entregues.Length),
            Percentual(perdas, movimentos.Count),
            produtoresAtivos,
            cooperativasAtivas,
            Percentual(comEvidencia, totalEntregas),
            TelasAcessiveisPercentual);
    }

    private static double Percentual(int parte, int total) =>
        total == 0 ? 0 : Math.Round(parte * 100d / total, 1);
}

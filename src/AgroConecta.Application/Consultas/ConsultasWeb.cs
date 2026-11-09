using AgroConecta.Domain.Enums;

namespace AgroConecta.Application.Consultas;

public record ProdutorResumo(
    int Id,
    string Nome,
    string Email,
    string Documento,
    string NomePropriedade,
    string Cidade,
    string UF,
    Comunidade Comunidade,
    StatusCadastro StatusCadastro,
    string? Cooperativa,
    DateTime CriadoEm);

public record CooperativaResumo(
    int Id,
    string Nome,
    string Cidade,
    string UF,
    string Tipo,
    bool Ativa,
    int QuantidadeProdutores);

public record FaturaResumo(
    int Id,
    int PedidoId,
    string Numero,
    decimal Valor,
    StatusFatura Status,
    DateTime? EmitidaEm,
    DateTime? PagoEm);

public record UsuarioResumo(
    int Id,
    string Nome,
    string Email,
    PapelUsuario Papel,
    bool Ativo,
    DateTime CriadoEm);

public record LogAcessoResumo(
    long Id,
    string? Usuario,
    string Acao,
    string Entidade,
    string? EntidadeId,
    string? IP,
    DateTime CriadoEm);

public record EntregaResumo(
    int Id,
    int PedidoId,
    string Comprador,
    StatusEntrega Status,
    DateOnly DataPrevista,
    DateTime? DataRealizada,
    string? Motorista,
    string? FotoUrl);

public record MotoristaResumo(int Id, string Nome, string CNH);

public record VeiculoResumo(int Id, string Placa, string Tipo, decimal CapacidadeKg);

public record PedidoSemEntregaResumo(
    int Id,
    string Comprador,
    StatusPedido Status,
    decimal ValorTotal,
    DateTime DataPedido);

/// <summary>Consultas de leitura otimizadas para o painel Web administrativo.</summary>
public interface IConsultasWebRepositorio
{
    Task<IReadOnlyList<ProdutorResumo>> ListarProdutoresAsync(
        StatusCadastro? status = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CooperativaResumo>> ListarCooperativasAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FaturaResumo>> ListarFaturasAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UsuarioResumo>> ListarUsuariosAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LogAcessoResumo>> ListarLogsAsync(
        int limite = 100,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EntregaResumo>> ListarEntregasAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MotoristaResumo>> ListarMotoristasAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VeiculoResumo>> ListarVeiculosAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PedidoSemEntregaResumo>> ListarPedidosSemEntregaAsync(
        CancellationToken cancellationToken = default);
}

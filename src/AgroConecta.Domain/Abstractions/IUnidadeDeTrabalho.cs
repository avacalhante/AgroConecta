namespace AgroConecta.Domain.Abstractions;

/// <summary>Fronteira transacional dos casos de uso (Unit of Work).</summary>
public interface IUnidadeDeTrabalho
{
    Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}

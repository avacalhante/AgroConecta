namespace AgroConecta.Domain.Common;

/// <summary>Chave e auditoria básica compartilhadas pelas entidades de negócio.</summary>
public abstract class EntidadeBase
{
    public int Id { get; set; }

    public DateTime CriadoEm { get; set; }

    public DateTime? AtualizadoEm { get; set; }
}

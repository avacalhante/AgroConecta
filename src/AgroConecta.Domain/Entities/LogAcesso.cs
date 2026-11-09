namespace AgroConecta.Domain.Entities;

/// <summary>Trilha de auditoria de ações sensíveis (login, aprovação, status).</summary>
public class LogAcesso
{
    public long Id { get; set; }

    public int? UsuarioId { get; set; }

    public Usuario? Usuario { get; set; }

    public string Acao { get; set; } = string.Empty;

    public string Entidade { get; set; } = string.Empty;

    public string? EntidadeId { get; set; }

    public string? IP { get; set; }

    public string? UserAgent { get; set; }

    public DateTime CriadoEm { get; set; }
}

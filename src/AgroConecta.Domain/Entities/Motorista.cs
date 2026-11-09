using AgroConecta.Domain.Common;

namespace AgroConecta.Domain.Entities;

public class Motorista : EntidadeBase
{
    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;

    public string CNH { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;

    public ICollection<Rota> Rotas { get; set; } = new List<Rota>();
}

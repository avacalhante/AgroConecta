using AgroConecta.Domain.Common;

namespace AgroConecta.Domain.Entities;

public class Veiculo : EntidadeBase
{
    public string Placa { get; set; } = string.Empty;

    public string Tipo { get; set; } = string.Empty;

    public decimal CapacidadeKg { get; set; }

    public bool Ativo { get; set; } = true;

    public ICollection<Rota> Rotas { get; set; } = new List<Rota>();
}

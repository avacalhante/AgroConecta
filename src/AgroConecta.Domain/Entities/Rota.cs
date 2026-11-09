using AgroConecta.Domain.Common;
using AgroConecta.Domain.Enums;

namespace AgroConecta.Domain.Entities;

public class Rota : EntidadeBase
{
    public DateOnly Data { get; set; }

    public int MotoristaId { get; set; }

    public Motorista Motorista { get; set; } = null!;

    public int VeiculoId { get; set; }

    public Veiculo Veiculo { get; set; } = null!;

    public StatusRota Status { get; set; } = StatusRota.Planejada;

    public ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();
}

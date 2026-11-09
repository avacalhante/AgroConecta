using System.ComponentModel.DataAnnotations;
using AgroConecta.Application.Consultas;

namespace AgroConecta.Web.Models;

public record EntregasViewModel(
    IReadOnlyList<EntregaResumo> Entregas,
    IReadOnlyCollection<AgroConecta.Application.Rotas.RotaDto> Rotas);

public class RotaViewModel
{
    [Required(ErrorMessage = "Informe a data da rota.")]
    [Display(Name = "Data")]
    public DateOnly Data { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [Range(1, int.MaxValue, ErrorMessage = "Selecione o motorista.")]
    [Display(Name = "Motorista")]
    public int MotoristaId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione o veículo.")]
    [Display(Name = "Veículo")]
    public int VeiculoId { get; set; }

    [Display(Name = "Pedidos")]
    public List<int> PedidoIds { get; set; } = [];

    public IReadOnlyList<MotoristaResumo> Motoristas { get; set; } = [];

    public IReadOnlyList<VeiculoResumo> Veiculos { get; set; } = [];

    public IReadOnlyList<PedidoSemEntregaResumo> Pedidos { get; set; } = [];
}

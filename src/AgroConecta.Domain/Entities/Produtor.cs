using AgroConecta.Domain.Common;
using AgroConecta.Domain.Enums;

namespace AgroConecta.Domain.Entities;

public class Produtor : EntidadeBase
{
    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;

    public int? CooperativaId { get; set; }

    public Cooperativa? Cooperativa { get; set; }

    public string NomePropriedade { get; set; } = string.Empty;

    public string Documento { get; set; } = string.Empty;

    public string Cidade { get; set; } = string.Empty;

    public string UF { get; set; } = string.Empty;

    public Comunidade Comunidade { get; set; } = Comunidade.Tradicional;

    public StatusCadastro StatusCadastro { get; set; } = StatusCadastro.Pendente;

    public DateTime? AprovadoEm { get; set; }

    public int? AprovadoPorUsuarioId { get; set; }

    public Usuario? AprovadoPorUsuario { get; set; }

    public ICollection<LoteColheita> Lotes { get; set; } = new List<LoteColheita>();
}

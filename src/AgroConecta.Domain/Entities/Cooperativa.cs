using AgroConecta.Domain.Common;

namespace AgroConecta.Domain.Entities;

public class Cooperativa : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;

    public string Cidade { get; set; } = string.Empty;

    public string UF { get; set; } = string.Empty;

    public string Tipo { get; set; } = string.Empty;

    public bool Ativa { get; set; } = true;

    public ICollection<Produtor> Produtores { get; set; } = new List<Produtor>();
}

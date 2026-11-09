using AgroConecta.Domain.Common;
using AgroConecta.Domain.Enums;

namespace AgroConecta.Domain.Entities;

/// <summary>Usuário autenticável do sistema. Compradores não têm tabela própria.</summary>
public class Usuario : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string SenhaHash { get; set; } = string.Empty;

    public PapelUsuario Papel { get; set; }

    public bool Ativo { get; set; } = true;

    public Produtor? Produtor { get; set; }

    public Motorista? Motorista { get; set; }

    public ICollection<Pedido> PedidosComoComprador { get; set; } = new List<Pedido>();

    public ICollection<LogAcesso> LogsAcesso { get; set; } = new List<LogAcesso>();
}

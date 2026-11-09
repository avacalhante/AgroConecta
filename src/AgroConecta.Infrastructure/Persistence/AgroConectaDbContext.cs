using AgroConecta.Domain.Common;
using AgroConecta.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgroConecta.Infrastructure.Persistence;

public class AgroConectaDbContext(DbContextOptions<AgroConectaDbContext> options)
    : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Cooperativa> Cooperativas => Set<Cooperativa>();

    public DbSet<Produtor> Produtores => Set<Produtor>();

    public DbSet<Produto> Produtos => Set<Produto>();

    public DbSet<LoteColheita> LotesColheita => Set<LoteColheita>();

    public DbSet<MovimentoEstoque> MovimentosEstoque => Set<MovimentoEstoque>();

    public DbSet<Pedido> Pedidos => Set<Pedido>();

    public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();

    public DbSet<Veiculo> Veiculos => Set<Veiculo>();

    public DbSet<Motorista> Motoristas => Set<Motorista>();

    public DbSet<Rota> Rotas => Set<Rota>();

    public DbSet<Entrega> Entregas => Set<Entrega>();

    public DbSet<Fatura> Faturas => Set<Fatura>();

    public DbSet<LogAcesso> LogsAcesso => Set<LogAcesso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgroConectaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        AplicarAuditoria();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        AplicarAuditoria();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void AplicarAuditoria()
    {
        var agora = DateTime.UtcNow;

        foreach (var entrada in ChangeTracker.Entries<EntidadeBase>())
        {
            switch (entrada.State)
            {
                case EntityState.Added:
                    entrada.Entity.CriadoEm = agora;
                    break;
                case EntityState.Modified:
                    entrada.Entity.AtualizadoEm = agora;
                    break;
            }
        }

        foreach (var entrada in ChangeTracker.Entries<LogAcesso>())
        {
            if (entrada.State == EntityState.Added && entrada.Entity.CriadoEm == default)
            {
                entrada.Entity.CriadoEm = agora;
            }
        }
    }
}

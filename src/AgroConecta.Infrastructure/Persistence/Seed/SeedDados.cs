using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgroConecta.Infrastructure.Persistence.Seed;

/// <summary>
/// Carga de desenvolvimento alinhada a <c>contracts/seed.md</c>.
/// Nunca deve ser executada em produção.
/// </summary>
public static class SeedDados
{
    public const string SenhaAdmin = "Admin@123";
    public const string SenhaProdutor = "Produtor@123";
    public const string SenhaComprador = "Comprador@123";
    public const string SenhaMotorista = "Motorista@123";

    private static readonly DateTime DataPedido1 = new(2026, 11, 3, 9, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime DataPedido2 = new(2026, 11, 4, 10, 30, 0, DateTimeKind.Utc);

    /// <summary>Aplica o seed se o banco ainda não tiver usuários. Retorna <c>true</c> quando inseriu.</summary>
    public static async Task<bool> AplicarAsync(
        AgroConectaDbContext context,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        if (await context.Usuarios.AnyAsync(cancellationToken))
        {
            logger?.LogInformation("Seed ignorado: o banco já possui usuários.");
            return false;
        }

        var hasher = new PasswordHasher<Usuario>();

        var admin = NovoUsuario(hasher, "Ana Administradora", "admin@agroconecta.local", SenhaAdmin, PapelUsuario.Admin);
        var usuarioProdutor = NovoUsuario(hasher, "Paulo Produtor", "produtor@agroconecta.local", SenhaProdutor, PapelUsuario.Produtor);
        var usuarioProdutora = NovoUsuario(hasher, "Maria Quilombola", "maria@agroconecta.local", SenhaProdutor, PapelUsuario.Produtor);
        var comprador = NovoUsuario(hasher, "Carla Compradora", "comprador@agroconecta.local", SenhaComprador, PapelUsuario.Comprador);
        var usuarioMotorista = NovoUsuario(hasher, "Marcos Motorista", "motorista@agroconecta.local", SenhaMotorista, PapelUsuario.Motorista);

        var cooperativa = new Cooperativa
        {
            Nome = "Cooperativa Vale Verde",
            Cidade = "Campinas",
            UF = "SP",
            Tipo = "Agricultura familiar",
            Ativa = true,
        };

        var produtorPaulo = new Produtor
        {
            Usuario = usuarioProdutor,
            Cooperativa = cooperativa,
            NomePropriedade = "Sítio Boa Esperança",
            Documento = "123.456.789-00",
            Cidade = "Campinas",
            UF = "SP",
            Comunidade = Comunidade.Tradicional,
            StatusCadastro = StatusCadastro.Aprovado,
            AprovadoEm = DataPedido1,
            AprovadoPorUsuario = admin,
        };

        var produtoraMaria = new Produtor
        {
            Usuario = usuarioProdutora,
            Cooperativa = cooperativa,
            NomePropriedade = "Comunidade Quilombo do Vale",
            Documento = "987.654.321-00",
            Cidade = "Campinas",
            UF = "SP",
            Comunidade = Comunidade.Quilombola,
            StatusCadastro = StatusCadastro.Aprovado,
            AprovadoEm = DataPedido1,
            AprovadoPorUsuario = admin,
        };

        var motorista = new Motorista
        {
            Usuario = usuarioMotorista,
            CNH = "12345678900",
            Ativo = true,
        };

        var veiculo = new Veiculo
        {
            Placa = "ABC-1234",
            Tipo = "Van",
            CapacidadeKg = 1_500m,
            Ativo = true,
        };

        var tomate = new Produto
        {
            Nome = "Tomate italiano",
            Categoria = "Hortifrúti",
            Unidade = "kg",
            PrecoBase = 8.50m,
            Ativo = true,
        };

        var alface = new Produto
        {
            Nome = "Alface crespa",
            Categoria = "Hortifrúti",
            Unidade = "unidade",
            PrecoBase = 3.20m,
            Ativo = true,
        };

        var milho = new Produto
        {
            Nome = "Milho verde",
            Categoria = "Grãos",
            Unidade = "kg",
            PrecoBase = 5.40m,
            Ativo = true,
        };

        var loteTomate = new LoteColheita
        {
            Produtor = produtorPaulo,
            Produto = tomate,
            CodigoRastreio = Guid.Parse("550e8400-e29b-41d4-a716-446655440001"),
            DataColheita = new DateOnly(2026, 11, 1),
            Quantidade = 120m,
            Status = StatusLote.Disponivel,
        };

        var loteAlface = new LoteColheita
        {
            Produtor = produtoraMaria,
            Produto = alface,
            CodigoRastreio = Guid.Parse("550e8400-e29b-41d4-a716-446655440002"),
            DataColheita = new DateOnly(2026, 11, 2),
            Quantidade = 80m,
            Status = StatusLote.Reservado,
        };

        var pedido1 = new Pedido
        {
            Comprador = comprador,
            DataPedido = DataPedido1,
            Status = StatusPedido.EmRota,
            ValorTotal = 425.00m,
            EnderecoEntrega = "Rua das Acácias, 120 — Campinas/SP",
            Itens =
            {
                new ItemPedido
                {
                    Produto = tomate,
                    Lote = loteTomate,
                    Quantidade = 50m,
                    PrecoUnitario = 8.50m,
                    Subtotal = 425.00m,
                },
            },
        };

        var pedido2 = new Pedido
        {
            Comprador = comprador,
            DataPedido = DataPedido2,
            Status = StatusPedido.Aprovado,
            ValorTotal = 320.00m,
            EnderecoEntrega = "Av. dos Ipês, 45 — Valinhos/SP",
            Itens =
            {
                new ItemPedido
                {
                    Produto = alface,
                    Lote = loteAlface,
                    Quantidade = 100m,
                    PrecoUnitario = 3.20m,
                    Subtotal = 320.00m,
                },
            },
        };

        var rota = new Rota
        {
            Data = new DateOnly(2026, 11, 5),
            Motorista = motorista,
            Veiculo = veiculo,
            Status = StatusRota.EmAndamento,
        };

        var entrega1 = new Entrega
        {
            Pedido = pedido1,
            Rota = rota,
            Status = StatusEntrega.EmRota,
            DataPrevista = new DateOnly(2026, 11, 5),
        };

        var entrega2 = new Entrega
        {
            Pedido = pedido2,
            Rota = rota,
            Status = StatusEntrega.Pendente,
            DataPrevista = new DateOnly(2026, 11, 6),
        };

        var fatura1 = new Fatura
        {
            Pedido = pedido1,
            Numero = "FAT-2026-0001",
            Valor = 425.00m,
            Status = StatusFatura.Emitida,
            EmitidaEm = DataPedido2,
        };

        var movimentos = new[]
        {
            new MovimentoEstoque
            {
                Lote = loteTomate,
                Tipo = TipoMovimentoEstoque.Entrada,
                Quantidade = 120m,
            },
            new MovimentoEstoque
            {
                Lote = loteAlface,
                Tipo = TipoMovimentoEstoque.Entrada,
                Quantidade = 80m,
            },
            new MovimentoEstoque
            {
                Lote = loteTomate,
                Tipo = TipoMovimentoEstoque.Saida,
                Quantidade = 50m,
                Pedido = pedido1,
            },
        };

        context.AddRange(admin, usuarioProdutor, usuarioProdutora, comprador, usuarioMotorista);
        context.AddRange(cooperativa, produtorPaulo, produtoraMaria, motorista, veiculo);
        context.AddRange(tomate, alface, milho);
        context.AddRange(loteTomate, loteAlface);
        context.AddRange(pedido1, pedido2);
        context.AddRange(rota, entrega1, entrega2, fatura1);
        context.AddRange(movimentos);
        context.Add(new LogAcesso
        {
            Usuario = admin,
            Acao = "Seed",
            Entidade = "Banco",
            EntidadeId = "inicial",
            IP = "127.0.0.1",
            UserAgent = "SeedDados",
        });

        await context.SaveChangesAsync(cancellationToken);
        logger?.LogInformation("Seed de desenvolvimento aplicado.");
        return true;
    }

    private static Usuario NovoUsuario(
        PasswordHasher<Usuario> hasher,
        string nome,
        string email,
        string senha,
        PapelUsuario papel)
    {
        var usuario = new Usuario
        {
            Nome = nome,
            Email = email,
            Papel = papel,
            Ativo = true,
        };
        usuario.SenhaHash = hasher.HashPassword(usuario, senha);
        return usuario;
    }
}

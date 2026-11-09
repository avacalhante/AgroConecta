using AgroConecta.Application.Entregas;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;
using AgroConecta.Domain.Exceptions;
using Moq;

namespace AgroConecta.Tests.Aplicacao;

public class EntregaCasosDeUsoTests
{
    private static Usuario MotoristaUsuario(int motoristaId = 1) => new()
    {
        Id = 5,
        Nome = "Marcos Motorista",
        Email = "motorista@agroconecta.local",
        Papel = PapelUsuario.Motorista,
        Motorista = new Motorista { Id = motoristaId, UsuarioId = 5 },
    };

    private static Entrega EntregaDaRota(int motoristaId = 1) => new()
    {
        Id = 10,
        Pedido = new Pedido(),
        Rota = new Rota { Id = 1, MotoristaId = motoristaId },
        DataPrevista = new DateOnly(2026, 11, 5),
    };

    private static AlterarStatusEntregaCasoDeUso CriarCasoDeUso(
        Mock<IEntregaRepositorio> entregas,
        Mock<IUsuarioRepositorio> usuarios)
    {
        return new AlterarStatusEntregaCasoDeUso(
            entregas.Object,
            usuarios.Object,
            new Mock<IUnidadeDeTrabalho>().Object);
    }

    [Fact]
    public async Task Motorista_da_rota_altera_status()
    {
        var entrega = EntregaDaRota();
        var entregas = new Mock<IEntregaRepositorio>();
        entregas.Setup(repositorio => repositorio.ObterPorIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entrega);

        var usuarios = new Mock<IUsuarioRepositorio>();
        usuarios.Setup(repositorio => repositorio.ObterPorIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(MotoristaUsuario());

        var resultado = await CriarCasoDeUso(entregas, usuarios).ExecutarAsync(
            new AlterarStatusEntregaComando(10, 5, false, StatusEntrega.EmRota));

        Assert.Equal(StatusEntrega.EmRota, resultado.Status);
    }

    [Fact]
    public async Task Motorista_de_outra_rota_recebe_acesso_negado()
    {
        var entrega = EntregaDaRota(motoristaId: 99);
        var entregas = new Mock<IEntregaRepositorio>();
        entregas.Setup(repositorio => repositorio.ObterPorIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entrega);

        var usuarios = new Mock<IUsuarioRepositorio>();
        usuarios.Setup(repositorio => repositorio.ObterPorIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(MotoristaUsuario());

        await Assert.ThrowsAsync<AcessoNegadoException>(
            () => CriarCasoDeUso(entregas, usuarios).ExecutarAsync(
                new AlterarStatusEntregaComando(10, 5, false, StatusEntrega.EmRota)));
    }

    [Fact]
    public async Task Admin_altera_entrega_de_qualquer_motorista()
    {
        var entrega = EntregaDaRota(motoristaId: 99);
        var entregas = new Mock<IEntregaRepositorio>();
        entregas.Setup(repositorio => repositorio.ObterPorIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entrega);

        var resultado = await CriarCasoDeUso(entregas, new Mock<IUsuarioRepositorio>())
            .ExecutarAsync(new AlterarStatusEntregaComando(10, 1, true, StatusEntrega.EmRota));

        Assert.Equal(StatusEntrega.EmRota, resultado.Status);
    }

    [Fact]
    public async Task Listar_minhas_exige_perfil_de_motorista()
    {
        var usuarios = new Mock<IUsuarioRepositorio>();
        usuarios.Setup(repositorio => repositorio.ObterPorIdAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Usuario { Id = 4, Papel = PapelUsuario.Comprador });

        var casoDeUso = new ListarMinhasEntregasCasoDeUso(
            new Mock<IEntregaRepositorio>().Object,
            usuarios.Object);

        await Assert.ThrowsAsync<AcessoNegadoException>(
            () => casoDeUso.ExecutarAsync(new ListarMinhasEntregasConsulta(4, false)));
    }

    [Fact]
    public async Task Evidencia_exige_foto()
    {
        var entrega = EntregaDaRota();
        var entregas = new Mock<IEntregaRepositorio>();
        entregas.Setup(repositorio => repositorio.ObterPorIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entrega);

        var usuarios = new Mock<IUsuarioRepositorio>();
        usuarios.Setup(repositorio => repositorio.ObterPorIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(MotoristaUsuario());

        var casoDeUso = new RegistrarEvidenciaCasoDeUso(
            entregas.Object,
            usuarios.Object,
            new Mock<IUnidadeDeTrabalho>().Object);

        await Assert.ThrowsAsync<RegraNegocioException>(
            () => casoDeUso.ExecutarAsync(
                new RegistrarEvidenciaComando(10, 5, false, 0, 0, " ")));
    }
}

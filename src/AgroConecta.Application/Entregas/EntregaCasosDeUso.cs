using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;
using AgroConecta.Domain.Exceptions;

namespace AgroConecta.Application.Entregas;

internal static class AutorizacaoEntrega
{
    /// <summary>
    /// Garante que o motorista autenticado é o responsável pela entrega.
    /// Administradores podem operar qualquer entrega.
    /// </summary>
    public static async Task GarantirAcessoAsync(
        IUsuarioRepositorio usuarios,
        Entrega entrega,
        int usuarioId,
        bool ehAdmin,
        CancellationToken cancellationToken)
    {
        if (ehAdmin)
        {
            return;
        }

        var usuario = await usuarios.ObterPorIdAsync(usuarioId, cancellationToken)
            ?? throw new RecursoNaoEncontradoException("Usuário não encontrado.");

        var motorista = usuario.Motorista
            ?? throw new AcessoNegadoException("A operação exige um usuário com perfil de motorista.");

        if (entrega.Rota?.MotoristaId != motorista.Id)
        {
            throw new AcessoNegadoException("A entrega não pertence ao motorista autenticado.");
        }
    }
}

public class ListarMinhasEntregasCasoDeUso(
    IEntregaRepositorio entregas,
    IUsuarioRepositorio usuarios)
    : Common.ICasoDeUso<ListarMinhasEntregasConsulta, IReadOnlyCollection<EntregaDto>>
{
    public async Task<IReadOnlyCollection<EntregaDto>> ExecutarAsync(
        ListarMinhasEntregasConsulta entrada,
        CancellationToken cancellationToken = default)
    {
        int? motoristaId = null;

        if (!entrada.EhAdmin)
        {
            var usuario = await usuarios.ObterPorIdAsync(entrada.UsuarioId, cancellationToken)
                ?? throw new RecursoNaoEncontradoException("Usuário não encontrado.");

            motoristaId = usuario.Motorista?.Id
                ?? throw new AcessoNegadoException(
                    "A operação exige um usuário com perfil de motorista.");
        }

        var resultado = await entregas.ListarAsync(motoristaId, entrada.Status, cancellationToken);

        return resultado.Select(MapeadorEntrega.ParaDto).ToArray();
    }
}

public class AlterarStatusEntregaCasoDeUso(
    IEntregaRepositorio entregas,
    IUsuarioRepositorio usuarios,
    IUnidadeDeTrabalho unidadeDeTrabalho)
    : Common.ICasoDeUso<AlterarStatusEntregaComando, EntregaDto>
{
    public async Task<EntregaDto> ExecutarAsync(
        AlterarStatusEntregaComando entrada,
        CancellationToken cancellationToken = default)
    {
        var entrega = await entregas.ObterPorIdAsync(entrada.EntregaId, cancellationToken)
            ?? throw new RecursoNaoEncontradoException("Entrega não encontrada.");

        await AutorizacaoEntrega.GarantirAcessoAsync(
            usuarios,
            entrega,
            entrada.UsuarioId,
            entrada.EhAdmin,
            cancellationToken);

        entrega.AlterarStatus(entrada.Status);
        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);

        return MapeadorEntrega.ParaDto(entrega);
    }
}

public class RegistrarEvidenciaCasoDeUso(
    IEntregaRepositorio entregas,
    IUsuarioRepositorio usuarios,
    IUnidadeDeTrabalho unidadeDeTrabalho)
    : Common.ICasoDeUso<RegistrarEvidenciaComando, EntregaDto>
{
    public async Task<EntregaDto> ExecutarAsync(
        RegistrarEvidenciaComando entrada,
        CancellationToken cancellationToken = default)
    {
        var entrega = await entregas.ObterPorIdAsync(entrada.EntregaId, cancellationToken)
            ?? throw new RecursoNaoEncontradoException("Entrega não encontrada.");

        await AutorizacaoEntrega.GarantirAcessoAsync(
            usuarios,
            entrega,
            entrada.UsuarioId,
            entrada.EhAdmin,
            cancellationToken);

        entrega.RegistrarEvidencia(
            entrada.Latitude,
            entrada.Longitude,
            entrada.FotoUrl,
            entrada.Observacao);

        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);

        return MapeadorEntrega.ParaDto(entrega);
    }
}

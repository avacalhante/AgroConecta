using AgroConecta.Application.Common;
using AgroConecta.Application.Entregas;
using AgroConecta.Domain.Enums;
using AgroConecta.Web.Api.Contratos;
using AgroConecta.Web.Servicos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Api.Controllers;

[ApiController]
[Route("api/v1/entregas")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
public class EntregasController(
    ICasoDeUso<ListarMinhasEntregasConsulta, IReadOnlyCollection<EntregaDto>> listarMinhas,
    ICasoDeUso<AlterarStatusEntregaComando, EntregaDto> alterarStatus,
    ICasoDeUso<RegistrarEvidenciaComando, EntregaDto> registrarEvidencia,
    ArmazenamentoEvidencia armazenamento) : ControllerBase
{
    /// <summary>Lista as entregas do motorista autenticado.</summary>
    [HttpGet("minhas")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyCollection<EntregaDto>>> Minhas(
        [FromQuery] StatusEntrega? status = null,
        CancellationToken cancellationToken = default)
    {
        var resultado = await listarMinhas.ExecutarAsync(
            new ListarMinhasEntregasConsulta(User.ObterUsuarioId(), User.EhAdmin(), status),
            cancellationToken);

        return Ok(resultado);
    }

    /// <summary>Atualiza o status da entrega. Operação idempotente.</summary>
    [HttpPatch("{id:int}/status")]
    [Authorize(Policy = "Entregar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<EntregaDto>> AlterarStatus(
        int id,
        [FromBody] EntregaStatusRequest request,
        CancellationToken cancellationToken)
    {
        if (request.OcorridoEm == default)
        {
            throw new ValidacaoException(new Dictionary<string, string[]>
            {
                [nameof(request.OcorridoEm)] = ["O horário do ocorrido é obrigatório."],
            });
        }

        return Ok(await alterarStatus.ExecutarAsync(
            new AlterarStatusEntregaComando(
                id,
                User.ObterUsuarioId(),
                User.EhAdmin(),
                request.Status,
                request.Observacao),
            cancellationToken));
    }

    /// <summary>Envia ou substitui a evidência (foto e coordenadas). Operação idempotente.</summary>
    [HttpPut("{id:int}/evidencia")]
    [Authorize(Policy = "Entregar")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<EntregaDto>> EnviarEvidencia(
        int id,
        IFormFile? file,
        [FromForm] string latitude,
        [FromForm] string longitude,
        [FromForm] string? observacao,
        CancellationToken cancellationToken)
    {
        var latitudeConvertida = ConverterCoordenada(latitude, nameof(latitude));
        var longitudeConvertida = ConverterCoordenada(longitude, nameof(longitude));

        var fotoUrl = await armazenamento.SalvarAsync(file, cancellationToken);

        return Ok(await registrarEvidencia.ExecutarAsync(
            new RegistrarEvidenciaComando(
                id,
                User.ObterUsuarioId(),
                User.EhAdmin(),
                latitudeConvertida,
                longitudeConvertida,
                fotoUrl,
                observacao),
            cancellationToken));
    }

    /// <summary>
    /// Aceita ponto ou vírgula decimal, independentemente da cultura do servidor,
    /// para que o aplicativo móvel envie sempre o mesmo formato.
    /// </summary>
    private static double ConverterCoordenada(string valor, string campo)
    {
        if (double.TryParse(
                valor,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out var numero)
            || double.TryParse(
                valor,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.CurrentCulture,
                out numero))
        {
            return numero;
        }

        throw new ValidacaoException(new Dictionary<string, string[]>
        {
            [campo] = ["Informe uma coordenada numérica válida."],
        });
    }
}

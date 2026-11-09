using System.Security.Claims;
using AgroConecta.Application.Auth;
using AgroConecta.Application.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Api.Controllers;

[ApiController]
[Route("api/v1")]
[Produces("application/json")]
public class AuthController(
    ICasoDeUso<LoginComando, LoginResultado> autenticar,
    ICasoDeUso<RenovarTokenComando, LoginResultado> renovar,
    ICasoDeUso<int, UsuarioDto> obterUsuarioAtual) : ControllerBase
{
    /// <summary>Autentica o usuário e devolve o par de tokens.</summary>
    [HttpPost("auth/login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResultado>> Login(
        [FromBody] LoginComando comando,
        CancellationToken cancellationToken)
    {
        return Ok(await autenticar.ExecutarAsync(comando, cancellationToken));
    }

    /// <summary>Renova o par de tokens a partir de um refresh token válido.</summary>
    [HttpPost("auth/refresh")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResultado>> Refresh(
        [FromBody] RenovarTokenComando comando,
        CancellationToken cancellationToken)
    {
        return Ok(await renovar.ExecutarAsync(comando, cancellationToken));
    }

    /// <summary>
    /// Encerra a sessão. Como os tokens são autocontidos, o cliente deve descartá-los;
    /// a resposta apenas confirma o logout.
    /// </summary>
    [HttpPost("auth/logout")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Logout() => NoContent();

    /// <summary>Devolve o usuário autenticado no token de acesso.</summary>
    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UsuarioDto>> Me(CancellationToken cancellationToken)
    {
        var identificador = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(identificador, out var usuarioId))
        {
            return Unauthorized();
        }

        return Ok(await obterUsuarioAtual.ExecutarAsync(usuarioId, cancellationToken));
    }
}

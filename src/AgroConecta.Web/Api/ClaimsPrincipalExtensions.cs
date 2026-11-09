using System.Security.Claims;
using AgroConecta.Domain.Enums;

namespace AgroConecta.Web.Api;

/// <summary>Leitura dos dados do usuário autenticado a partir das claims do token.</summary>
public static class ClaimsPrincipalExtensions
{
    public static int ObterUsuarioId(this ClaimsPrincipal principal)
    {
        var valor = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(valor, out var usuarioId)
            ? usuarioId
            : throw new UnauthorizedAccessException("Token sem identificador de usuário.");
    }

    public static bool EhAdmin(this ClaimsPrincipal principal) =>
        principal.IsInRole(nameof(PapelUsuario.Admin));
}

using System.Security.Claims;
using AgroConecta.Application.Auth;
using AgroConecta.Application.Common;
using AgroConecta.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Controllers;

public class AccountController(ICasoDeUso<LoginComando, LoginResultado> autenticar) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel modelo,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        try
        {
            var resultado = await autenticar.ExecutarAsync(
                new LoginComando(modelo.Email, modelo.Senha),
                cancellationToken);

            await EntrarAsync(resultado.Usuario, modelo.Lembrar);

            if (!string.IsNullOrWhiteSpace(modelo.ReturnUrl) && Url.IsLocalUrl(modelo.ReturnUrl))
            {
                return Redirect(modelo.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
        catch (ValidacaoException excecao)
        {
            foreach (var (campo, mensagens) in excecao.Erros)
            {
                foreach (var mensagem in mensagens)
                {
                    ModelState.AddModelError(campo, mensagem);
                }
            }
        }
        catch (UnauthorizedAccessException excecao)
        {
            ModelState.AddModelError(string.Empty, excecao.Message);
        }

        return View(modelo);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AcessoNegado() => View();

    private async Task EntrarAsync(UsuarioDto usuario, bool persistente)
    {
        var identidade = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Papel.ToString()),
            ],
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identidade),
            new AuthenticationProperties
            {
                IsPersistent = persistente,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(persistente ? 8 : 2),
            });
    }
}

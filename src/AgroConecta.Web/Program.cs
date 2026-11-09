using System.Text.Json.Serialization;
using AgroConecta.Application;
using AgroConecta.Domain.Enums;
using AgroConecta.Infrastructure;
using AgroConecta.Infrastructure.Persistence;
using AgroConecta.Infrastructure.Persistence.Seed;
using AgroConecta.Infrastructure.Seguranca;
using AgroConecta.Web.Api;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddApplication();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "AgroConecta API",
        Version = "0.1.0",
        Description = "API corporativa do AgroConecta: catálogo, lotes, pedidos, entregas e indicadores.",
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Informe apenas o token JWT retornado por /api/v1/auth/login.",
    });

    options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = [],
    });
});

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException(
        "A string de conexão 'Default' não foi configurada. Veja appsettings.json ou docker/docker-compose.yml.");

var jwtConfiguracao = builder.Configuration
    .GetSection(JwtConfiguracao.Secao)
    .Get<JwtConfiguracao>()
    ?? throw new InvalidOperationException(
        "A seção 'Jwt' não foi configurada. Veja appsettings.json.");

builder.Services.AddInfrastructure(connectionString, jwtConfiguracao);
builder.Services.AddScoped<AgroConecta.Web.Servicos.ArmazenamentoEvidencia>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.Cookie.Name = "AgroConecta.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AcessoNegado";
        options.SlidingExpiration = true;

        // Requisições de API nunca devem redirecionar para a tela de login.
        options.Events.OnRedirectToLogin = contexto =>
        {
            if (contexto.Request.Path.StartsWithSegments("/api"))
            {
                contexto.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }

            contexto.Response.Redirect(contexto.RedirectUri);
            return Task.CompletedTask;
        };

        options.Events.OnRedirectToAccessDenied = contexto =>
        {
            if (contexto.Request.Path.StartsWithSegments("/api"))
            {
                contexto.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }

            contexto.Response.Redirect(contexto.RedirectUri);
            return Task.CompletedTask;
        };
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtConfiguracao.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtConfiguracao.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = GeradorTokenJwt.CriarChave(jwtConfiguracao.Chave),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = System.Security.Claims.ClaimTypes.Name,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async contexto =>
            {
                contexto.HandleResponse();

                var problema = new Microsoft.AspNetCore.Mvc.ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Type = "https://agroconecta.local/errors/nao-autenticado",
                    Title = "Não autenticado",
                    Detail = contexto.AuthenticateFailure is null
                        ? "Token de acesso ausente."
                        : "Token de acesso inválido ou expirado.",
                    Instance = contexto.Request.Path,
                };

                problema.Extensions["traceId"] =
                    System.Diagnostics.Activity.Current?.Id ?? contexto.HttpContext.TraceIdentifier;

                await Results.Problem(problema).ExecuteAsync(contexto.HttpContext);
            },
            OnForbidden = async contexto =>
            {
                var problema = new Microsoft.AspNetCore.Mvc.ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Type = "https://agroconecta.local/errors/sem-permissao",
                    Title = "Sem permissão",
                    Detail = "O usuário autenticado não possui o papel necessário.",
                    Instance = contexto.Request.Path,
                };

                problema.Extensions["traceId"] =
                    System.Diagnostics.Activity.Current?.Id ?? contexto.HttpContext.TraceIdentifier;

                await Results.Problem(problema).ExecuteAsync(contexto.HttpContext);
            },
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SomenteAdmin", politica =>
        politica.RequireRole(nameof(PapelUsuario.Admin)));

    options.AddPolicy("GerenciarCatalogo", politica =>
        politica.RequireRole(nameof(PapelUsuario.Admin), nameof(PapelUsuario.Produtor)));

    options.AddPolicy("Comprar", politica =>
        politica.RequireRole(nameof(PapelUsuario.Admin), nameof(PapelUsuario.Comprador)));

    options.AddPolicy("OperarPedidos", politica =>
        politica.RequireRole(
            nameof(PapelUsuario.Admin),
            nameof(PapelUsuario.Produtor),
            nameof(PapelUsuario.Motorista)));

    options.AddPolicy("Entregar", politica =>
        politica.RequireRole(nameof(PapelUsuario.Admin), nameof(PapelUsuario.Motorista)));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await using var escopo = app.Services.CreateAsyncScope();
    var contexto = escopo.ServiceProvider.GetRequiredService<AgroConectaDbContext>();
    await contexto.Database.MigrateAsync();
    await SeedDados.AplicarAsync(contexto, app.Logger);
}

app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "AgroConecta API v1"));
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

/// <summary>Exposição para testes de integração com WebApplicationFactory.</summary>
public partial class Program;

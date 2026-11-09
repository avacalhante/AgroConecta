namespace AgroConecta.Infrastructure.Seguranca;

/// <summary>Configuração de emissão de tokens JWT (seção <c>Jwt</c>).</summary>
public class JwtConfiguracao
{
    public const string Secao = "Jwt";

    public string Issuer { get; set; } = "AgroConecta";

    public string Audience { get; set; } = "AgroConecta";

    /// <summary>Chave simétrica usada na assinatura. Apenas para desenvolvimento.</summary>
    public string Chave { get; set; } = string.Empty;

    public int AccessTokenMinutos { get; set; } = 15;

    public int RefreshTokenDias { get; set; } = 7;
}

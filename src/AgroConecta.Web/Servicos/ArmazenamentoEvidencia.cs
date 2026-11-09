using AgroConecta.Application.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace AgroConecta.Web.Servicos;

/// <summary>
/// Armazena as fotos de evidência de entrega em <c>wwwroot/uploads/entregas</c>.
/// Em produção o mesmo contrato pode apontar para um storage externo.
/// </summary>
public class ArmazenamentoEvidencia(IWebHostEnvironment ambiente)
{
    private const long TamanhoMaximoBytes = 5 * 1024 * 1024;

    private static readonly IReadOnlyDictionary<string, string> ExtensoesPorTipo =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["image/jpeg"] = ".jpg",
            ["image/png"] = ".png",
            ["image/webp"] = ".webp",
        };

    public async Task<string> SalvarAsync(IFormFile? arquivo, CancellationToken cancellationToken = default)
    {
        if (arquivo is null || arquivo.Length == 0)
        {
            throw Erro("file", "O arquivo da evidência é obrigatório.");
        }

        if (arquivo.Length > TamanhoMaximoBytes)
        {
            throw Erro("file", "O arquivo deve ter no máximo 5 MB.");
        }

        if (!ExtensoesPorTipo.TryGetValue(arquivo.ContentType, out var extensao))
        {
            throw Erro("file", "Envie uma imagem JPEG, PNG ou WebP.");
        }

        var raiz = ambiente.WebRootPath ?? Path.Combine(ambiente.ContentRootPath, "wwwroot");
        var pasta = Path.Combine(raiz, "uploads", "entregas");
        Directory.CreateDirectory(pasta);

        var nome = $"{Guid.NewGuid():N}{extensao}";

        await using var destino = File.Create(Path.Combine(pasta, nome));
        await arquivo.CopyToAsync(destino, cancellationToken);

        return $"/uploads/entregas/{nome}";
    }

    private static ValidacaoException Erro(string campo, string mensagem) =>
        new(new Dictionary<string, string[]> { [campo] = [mensagem] });
}

using AgroConecta.Domain.Enums;

namespace AgroConecta.Application.Produtos;

public record ProdutoDto(
    int Id,
    string Nome,
    string Categoria,
    string Unidade,
    decimal PrecoBase,
    bool Ativo);

public record ListarProdutosConsulta(int Pagina, int TamanhoPagina, bool? Ativo = null);

public record CriarProdutoComando(string Nome, string Categoria, string Unidade, decimal PrecoBase);

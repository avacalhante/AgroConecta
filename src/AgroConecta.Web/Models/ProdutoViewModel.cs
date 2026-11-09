using System.ComponentModel.DataAnnotations;

namespace AgroConecta.Web.Models;

public class ProdutoViewModel
{
    [Required(ErrorMessage = "Informe o nome.")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a categoria.")]
    [Display(Name = "Categoria")]
    public string Categoria { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a unidade.")]
    [Display(Name = "Unidade")]
    public string Unidade { get; set; } = "kg";

    [Range(0, double.MaxValue, ErrorMessage = "O preço-base não pode ser negativo.")]
    [Display(Name = "Preço-base")]
    public decimal PrecoBase { get; set; }
}

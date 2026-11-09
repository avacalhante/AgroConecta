using System.ComponentModel.DataAnnotations;

namespace AgroConecta.Web.Models;

public class CooperativaViewModel
{
    [Required(ErrorMessage = "Informe o nome.")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a cidade.")]
    [Display(Name = "Cidade")]
    public string Cidade { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a UF.")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "A UF deve ter duas letras.")]
    [Display(Name = "UF")]
    public string UF { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o tipo.")]
    [Display(Name = "Tipo")]
    public string Tipo { get; set; } = "Agricultura familiar";
}

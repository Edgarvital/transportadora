using System.ComponentModel.DataAnnotations;

namespace Transportadora.DTOs.Solicitacoes;

public sealed class CargaDTO
{
    [Required(ErrorMessage = "Descricao da carga e obrigatoria.")]
    [MaxLength(200, ErrorMessage = "Descricao da carga deve ter no maximo 200 caracteres.")]
    public string DescricaoNome { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tipo da carga e obrigatorio.")]
    [MaxLength(100, ErrorMessage = "Tipo da carga deve ter no maximo 100 caracteres.")]
    public string Tipo { get; init; } = string.Empty;

    [Range(0.001, double.MaxValue, ErrorMessage = "Peso deve ser maior que zero.")]
    public decimal Peso { get; init; }

    [Range(0.001, double.MaxValue, ErrorMessage = "Altura deve ser maior que zero.")]
    public decimal Altura { get; init; }

    [Range(0.001, double.MaxValue, ErrorMessage = "Largura deve ser maior que zero.")]
    public decimal Largura { get; init; }

    [Range(0.001, double.MaxValue, ErrorMessage = "Comprimento deve ser maior que zero.")]
    public decimal Comprimento { get; init; }
}

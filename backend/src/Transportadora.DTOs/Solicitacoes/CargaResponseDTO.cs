namespace Transportadora.DTOs.Solicitacoes;

public sealed class CargaResponseDTO
{
    public int Id { get; init; }
    public string DescricaoNome { get; init; } = string.Empty;
    public string Tipo { get; init; } = string.Empty;
    public decimal Peso { get; init; }
    public decimal Altura { get; init; }
    public decimal Largura { get; init; }
    public decimal Comprimento { get; init; }
}

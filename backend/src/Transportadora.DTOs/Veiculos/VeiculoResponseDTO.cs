namespace Transportadora.DTOs.Veiculos;

public sealed class VeiculoResponseDTO
{
    public int Id { get; init; }
    public string Modelo { get; init; } = string.Empty;
    public string Placa { get; init; } = string.Empty;
}

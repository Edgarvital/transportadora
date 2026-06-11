namespace Transportadora.DTOs.Motoristas;

public sealed class MotoristaResponseDTO
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Documento { get; init; } = string.Empty;
}

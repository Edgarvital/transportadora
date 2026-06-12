using Transportadora.Models.Enums;

namespace Transportadora.DTOs.Solicitacoes;

public sealed class OcorrenciaResponseDTO
{
    public int Id { get; init; }
    public OcorrenciaTipo Tipo { get; init; }
    public string Descricao { get; init; } = string.Empty;
    public DateTime DataHora { get; init; }
    public int UserId { get; init; }
    public string UsuarioNome { get; init; } = string.Empty;
}

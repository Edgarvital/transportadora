using Transportadora.Models.Enums;

namespace Transportadora.Models.Entities;

public class Ocorrencia
{
    public int Id { get; set; }
    public int SolicitacaoId { get; set; }
    public int UserId { get; set; }
    public OcorrenciaTipo Tipo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }

    public SolicitacaoColeta Solicitacao { get; set; } = null!;
    public User User { get; set; } = null!;
}

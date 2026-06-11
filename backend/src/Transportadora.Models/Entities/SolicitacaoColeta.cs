using Transportadora.Models.Enums;

namespace Transportadora.Models.Entities;

public class SolicitacaoColeta
{
    public int Id { get; set; }
    public int RemetenteId { get; set; }
    public int DestinatarioId { get; set; }
    public int CargaId { get; set; }
    public int CriadoPorUserId { get; set; }
    public DateTime DataSolicitacao { get; set; }
    public DateTime DataPrevistaRetirada { get; set; }
    public PrioridadeNivel Prioridade { get; set; }
    public StatusColeta Status { get; set; }
    public int? MotoristaId { get; set; }
    public int? VeiculoId { get; set; }
    public string? ObservacoesGerais { get; set; }

    public User Remetente { get; set; } = null!;
    public User Destinatario { get; set; } = null!;
    public User CriadoPorUser { get; set; } = null!;
    public Carga Carga { get; set; } = null!;
    public Motorista? Motorista { get; set; }
    public Veiculo? Veiculo { get; set; }
    public ICollection<Ocorrencia> Ocorrencias { get; set; } = new List<Ocorrencia>();
}

namespace Transportadora.Models.Entities;

public class Veiculo
{
    public int Id { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;

    public ICollection<SolicitacaoColeta> Solicitacoes { get; set; } = new List<SolicitacaoColeta>();
}

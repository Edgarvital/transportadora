namespace Transportadora.Models.Entities;

public class Motorista
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;

    public ICollection<SolicitacaoColeta> Solicitacoes { get; set; } = new List<SolicitacaoColeta>();
}

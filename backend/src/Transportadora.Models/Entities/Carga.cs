namespace Transportadora.Models.Entities;

public class Carga
{
    public int Id { get; set; }
    public string DescricaoNome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal Peso { get; set; }
    public decimal Altura { get; set; }
    public decimal Largura { get; set; }
    public decimal Comprimento { get; set; }

    public SolicitacaoColeta? SolicitacaoColeta { get; set; }
}

using Transportadora.Models.Enums;

namespace Transportadora.Models.Entities;

public class User
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? SenhaHash { get; set; }
    public int RoleId { get; set; }
    public int EnderecoId { get; set; }
    public SituacaoUser Situacao { get; set; }

    public Role Role { get; set; } = null!;
    public Endereco Endereco { get; set; } = null!;
    public ICollection<SolicitacaoColeta> SolicitacoesComoRemetente { get; set; } = new List<SolicitacaoColeta>();
    public ICollection<SolicitacaoColeta> SolicitacoesComoDestinatario { get; set; } = new List<SolicitacaoColeta>();
    public ICollection<SolicitacaoColeta> SolicitacoesCriadas { get; set; } = new List<SolicitacaoColeta>();
    public ICollection<Ocorrencia> Ocorrencias { get; set; } = new List<Ocorrencia>();
}

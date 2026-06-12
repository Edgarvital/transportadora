namespace Transportadora.Shared.Errors;

public static class SolicitacaoErrorCodes
{
    public const string RemetenteNotFound = "REMETENTE_NOT_FOUND";
    public const string DestinatarioNotFound = "DESTINATARIO_NOT_FOUND";
    public const string CriadorNotFound = "CRIADOR_NOT_FOUND";
    public const string SolicitationNotFound = "SOLICITACAO_NOT_FOUND";
    public const string MotoristaNotFound = "MOTORISTA_NOT_FOUND";
    public const string VeiculoNotFound = "VEICULO_NOT_FOUND";
    public const string InvalidStatusTransition = "INVALID_STATUS_TRANSITION";
    public const string ColetadaNeedsRouting = "COLETADA_NEEDS_ROUTING";
}

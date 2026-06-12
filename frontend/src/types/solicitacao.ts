// Enums baseados no contrato da API C#
export enum StatusColeta {
  Aberta = 1,
  Roteirizada = 2,
  EmColeta = 3,
  Concluida = 4,
  Cancelada = 5,
}

export enum PrioridadeNivel {
  Baixa = 1,
  Normal = 2,
  Alta = 3,
}

export enum TipoOcorrencia {
  ClienteAusente = 1,
  EnderecoIncorreto = 2,
  DivergenciaCarga = 3,
  Outros = 4,
}

// Interfaces
export interface Carga {
  id: number;
  descricaoNome: string;
  tipo: string;
  peso: number;
  altura: number;
  largura: number;
  comprimento: number;
}

export interface Ocorrencia {
  id: number;
  tipo: TipoOcorrencia;
  descricao: string;
  dataHora: string;
  userId: number;
  usuarioNome: string;
}

export interface Solicitacao {
  id: number;
  remetenteId: number;
  remetenteNome: string;
  destinatarioId: number;
  destinatarioNome: string;
  criadoPorUserId: number;
  criadoPorNome: string;
  dataSolicitacao: string;
  dataPrevistaRetirada: string;
  prioridade: PrioridadeNivel;
  status: StatusColeta;
  motoristaId?: number;
  motoristaNome?: string;
  veiculoId?: number;
  veiculoModelo?: string;
  veiculoPlaca?: string;
  observacoesGerais?: string;
  carga: Carga;
  ocorrencias: Ocorrencia[];
}

// Interface genérica de paginação
export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

// DTOs para requisições
export interface CreateSolicitacaoDto {
  remetenteId: number;
  destinatarioId: number;
  dataPrevistaRetirada: string;
  prioridade: PrioridadeNivel;
  observacoesGerais?: string;
  carga: {
    descricaoNome: string;
    tipo: string;
    peso: number;
    altura: number;
    largura: number;
    comprimento: number;
  };
}

export interface RoteirizarDto {
  motoristaId: number;
  veiculoId: number;
}

export interface CreateOcorrenciaDto {
  tipo: TipoOcorrencia;
  descricao: string;
}

export interface UpdateStatusDto {
  novoStatus: StatusColeta;
}

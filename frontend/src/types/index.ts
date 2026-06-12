export interface User {
  id: string;
  name: string;
  email: string;
  roles?: string[];
}

export interface LoginCredentials {
  email: string;
  password: string;
}

export enum StatusColeta {
  PENDENTE = 'PENDENTE',
  EM_ANDAMENTO = 'EM_ANDAMENTO',
  CONCLUIDA = 'CONCLUIDA',
  CANCELADA = 'CANCELADA',
}

export enum PrioridadeNivel {
  BAIXA = 'BAIXA',
  MEDIA = 'MEDIA',
  ALTA = 'ALTA',
}

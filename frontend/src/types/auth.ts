export interface LoginCredentials {
  email: string;
  senha: string;
}

export interface User {
  id: string;
  nome: string;
  email: string;
  role?: string;
}

export interface AuthResponse {
  token: string;
  user?: User; 
}

export interface Address {
  cep: string;
  logradouro: string;
  numero: string;
  complemento?: string;
  bairro: string;
  cidade: string;
  uf: string;
}

export interface RegisterPayload {
  nome: string;
  documento: string;
  endereco: Address;
  email: string;
  senha: string;
}

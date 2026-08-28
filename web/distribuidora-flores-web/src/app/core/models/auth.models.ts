export interface LoginRequest {
  email: string;
  senha: string;
}

export interface RegistrarComercianteRequest {
  nomeFantasia: string;
  documento: string;
  telefone: string;
  endereco: string;
  email: string;
  senha: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
}

export interface UsuarioLogado {
  id: string;
  email: string;
  role: 'Admin' | 'Comerciante';
  clienteId?: string;
}
// Tipos para as entidades do sistema
export interface Pessoa {
  id?: number;
  nome: string;
  idade: number;
  email?: string;
  telefone?: string;
  isMenorDeIdade?: boolean;
  totalTransacoes?: number;
}

export interface Categoria {
  id?: number;
  descricao: string;
  finalidade: number; // 1 = Despesa, 2 = Receita, 3 = Ambas
  finalidadeDescricao?: string;
  totalTransacoes?: number;
}

export interface Transacao {
  id?: number;
  descricao: string;
  valor: number;
  data: string;
  dataCriacao?: string; // Campo do backend em camelCase (JavaScript style)
  DataCriacao?: string; // Campo do backend em PascalCase (C# style)
  tipo: number; // 1 = Despesa, 2 = Receita
  tipoDescricao?: string;
  pessoaId?: number;
  categoriaId?: number;
  pessoa?: Pessoa;
  categoria?: Categoria;
}

// DTOs para requisições
export interface CreatePessoaDto {
  nome: string;
  idade: number;
  email?: string;
  telefone?: string;
}

export interface CreateCategoriaDto {
  descricao: string;
  finalidade: number;
}

export interface CreateTransacaoDto {
  descricao: string;
  valor: number;
  data: string; // Para uso no formulário
  Data?: string; // Para envio ao backend C# (PascalCase)
  tipo: number;
  pessoaId?: number;
  categoriaId?: number;
}

// Tipos para formulários
export interface FormErrors {
  [key: string]: string;
}

export interface ApiResponse<T> {
  data?: T;
  error?: string;
  message?: string;
}
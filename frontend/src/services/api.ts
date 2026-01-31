import { Pessoa, Categoria, Transacao } from '../types';

// API Service para comunicação com o backend
const API_BASE_URL = 'http://localhost:5118/api';

// Classe para tratar erros da API
export class ApiError extends Error {
  constructor(public status: number, message: string) {
    super(message);
    this.name = 'ApiError';
  }
}

// Função utilitária para fazer requisições
async function apiRequest<T>(
  endpoint: string, 
  options: RequestInit = {}
): Promise<T> {
  try {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      headers: {
        'Content-Type': 'application/json',
        ...options.headers
      },
      ...options
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new ApiError(response.status, errorText || response.statusText);
    }

    const data = await response.json();
    return data;
  } catch (error) {
    if (error instanceof ApiError) {
      throw error;
    }
    throw new ApiError(0, 'Erro de rede ou conexão');
  }
}

// Serviços para cada entidade
export const pessoasService = {
  async getAll(): Promise<Pessoa[]> {
    return apiRequest<Pessoa[]>('/pessoas');
  },

  async getById(id: number): Promise<Pessoa> {
    return apiRequest<Pessoa>(`/pessoas/${id}`);
  },

  async create(data: any): Promise<Pessoa> {
    return apiRequest<Pessoa>('/pessoas', {
      method: 'POST',
      body: JSON.stringify(data)
    });
  },

  async update(id: number, data: any): Promise<Pessoa> {
    return apiRequest<Pessoa>(`/pessoas/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  },

  async delete(id: number): Promise<void> {
    return apiRequest<void>(`/pessoas/${id}`, {
      method: 'DELETE'
    });
  }
};

export const categoriasService = {
  async getAll(): Promise<Categoria[]> {
    return apiRequest<Categoria[]>('/categorias');
  },

  async getById(id: number): Promise<Categoria> {
    return apiRequest<Categoria>(`/categorias/${id}`);
  },

  async create(data: any): Promise<Categoria> {
    return apiRequest<Categoria>('/categorias', {
      method: 'POST',
      body: JSON.stringify(data)
    });
  },

  async update(id: number, data: any): Promise<Categoria> {
    return apiRequest<Categoria>(`/categorias/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  },

  async delete(id: number): Promise<void> {
    return apiRequest<void>(`/categorias/${id}`, {
      method: 'DELETE'
    });
  }
};

export const transacoesService = {
  async getAll(): Promise<Transacao[]> {
    const data = await apiRequest<any[]>('/transacoes');
    // Mapear DataCriacao para data (backend C# retorna em PascalCase)
    return data.map(item => {
      const dataValue = item.dataCriacao || item.DataCriacao;
      console.log('Item recebido:', { id: item.id, descricao: item.descricao, dataCriacao: item.dataCriacao, DataCriacao: item.DataCriacao });
      return {
        ...item,
        data: dataValue || new Date().toISOString()
      };
    });
  },

  async getById(id: number): Promise<Transacao> {
    const item = await apiRequest<any>(`/transacoes/${id}`);
    // Mapear DataCriacao para data
    return {
      ...item,
      data: item.dataCriacao || item.DataCriacao || new Date().toISOString()
    };
  },

  async create(data: any): Promise<Transacao> {
    const item = await apiRequest<any>('/transacoes', {
      method: 'POST',
      body: JSON.stringify(data)
    });
    // Mapear DataCriacao para data
    return {
      ...item,
      data: item.dataCriacao || item.DataCriacao || new Date().toISOString()
    };
  },

  async update(id: number, data: any): Promise<Transacao> {
    return apiRequest<Transacao>(`/transacoes/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  },

  async delete(id: number): Promise<void> {
    return apiRequest<void>(`/transacoes/${id}`, {
      method: 'DELETE'
    });
  }
};
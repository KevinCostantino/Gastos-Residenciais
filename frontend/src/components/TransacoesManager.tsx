import React, { useState, useEffect } from 'react';
import { Transacao, Pessoa, Categoria, CreateTransacaoDto, FormErrors } from '../types';
import { transacoesService, pessoasService, categoriasService, ApiError } from '../services/api';

const TransacoesManager: React.FC = () => {
  const [transacoes, setTransacoes] = useState<Transacao[]>([]);
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [categorias, setCategorias] = useState<Categoria[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [editingTransacao, setEditingTransacao] = useState<Transacao | null>(null);
  const [message, setMessage] = useState<{ text: string; type: 'success' | 'error' | 'warning' } | null>(null);
  
  const [formData, setFormData] = useState<CreateTransacaoDto>({
    descricao: '',
    valor: 0,
    data: new Date().toISOString().split('T')[0],
    tipo: 1, // 1 = Despesa, 2 = Receita
    pessoaId: undefined,
    categoriaId: undefined
  });
  
  const [errors, setErrors] = useState<FormErrors>({});

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [transacoesData, pessoasData, categoriasData] = await Promise.all([
        transacoesService.getAll(),
        pessoasService.getAll(),
        categoriasService.getAll()
      ]);
      
      setTransacoes(transacoesData);
      setPessoas(pessoasData);
      setCategorias(categoriasData);
      showMessage('Dados carregados com sucesso', 'success');
    } catch (error) {
      console.error('Erro ao carregar dados:', error);
      showMessage('Erro ao carregar dados', 'error');
    } finally {
      setLoading(false);
    }
  };

  const showMessage = (text: string, type: 'success' | 'error' | 'warning') => {
    setMessage({ text, type });
    setTimeout(() => setMessage(null), 5000);
  };

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};
    
    if (!formData.descricao.trim()) {
      newErrors.descricao = 'Descrição é obrigatória';
    }
    
    if (!formData.valor || formData.valor <= 0) {
      newErrors.valor = 'Valor deve ser maior que zero';
    }
    
    if (!formData.data) {
      newErrors.data = 'Data é obrigatória';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) {
      return;
    }

    try {
      const submitData = {
        ...formData,
        pessoaId: formData.pessoaId || undefined,
        categoriaId: formData.categoriaId || undefined
      };

      if (editingTransacao) {
        await transacoesService.update(editingTransacao.id!, submitData);
        showMessage('Transação atualizada com sucesso', 'success');
      } else {
        await transacoesService.create(submitData);
        showMessage('Transação criada com sucesso', 'success');
      }
      
      resetForm();
      loadData();
    } catch (error) {
      if (error instanceof ApiError) {
        showMessage(`Erro: ${error.message}`, 'error');
      } else {
        showMessage('Erro inesperado', 'error');
      }
    }
  };

  const handleEdit = (transacao: Transacao) => {
    setEditingTransacao(transacao);
    setFormData({
      descricao: transacao.descricao,
      valor: transacao.valor,
      data: transacao.data.split('T')[0], // Converter para formato date input
      tipo: transacao.tipo,
      pessoaId: transacao.pessoaId,
      categoriaId: transacao.categoriaId
    });
    setShowForm(true);
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Tem certeza que deseja excluir esta transação?')) {
      return;
    }

    try {
      await transacoesService.delete(id);
      showMessage('Transação excluída com sucesso', 'success');
      loadData();
    } catch (error) {
      if (error instanceof ApiError) {
        showMessage(`Erro ao excluir: ${error.message}`, 'error');
      } else {
        showMessage('Erro inesperado', 'error');
      }
    }
  };

  const resetForm = () => {
    setFormData({
      descricao: '',
      valor: 0,
      data: new Date().toISOString().split('T')[0],
      tipo: 1,
      pessoaId: undefined,
      categoriaId: undefined
    });
    setEditingTransacao(null);
    setShowForm(false);
    setErrors({});
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    
    let processedValue: any = value;
    
    if (name === 'valor') {
      processedValue = parseFloat(value) || 0;
    } else if (name === 'pessoaId' || name === 'categoriaId') {
      processedValue = value ? parseInt(value) : undefined;
    }
    
    setFormData(prev => ({ ...prev, [name]: processedValue }));
    
    // Limpar erro do campo quando usuário começar a digitar
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  const handleSelectChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const { name, value } = e.target;
    
    let processedValue: any = value;
    
    if (name === 'tipo') {
      processedValue = parseInt(value);
    } else if (name === 'pessoaId' || name === 'categoriaId') {
      processedValue = value ? parseInt(value) : undefined;
    }
    
    setFormData(prev => ({ ...prev, [name]: processedValue }));
    
    // Limpar erro do campo quando usuário começar a digitar
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  const formatCurrency = (value: number): string => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  };

  const formatDate = (dateString: string): string => {
    try {
      // Tentar diferentes formatos de data
      let date: Date;
      
      if (!dateString) {
        return 'Sem data';
      }
      
      // Se já está no formato brasileiro
      if (dateString.includes('/')) {
        return dateString;
      }
      
      date = new Date(dateString);
      
      if (isNaN(date.getTime())) {
        // Tentar parseamento alternativo
        const parts = dateString.split(/[-T]/);
        if (parts.length >= 3) {
          date = new Date(parseInt(parts[0]), parseInt(parts[1]) - 1, parseInt(parts[2]));
        }
      }
      
      if (isNaN(date.getTime())) {
        console.warn('Data inválida recebida:', dateString);
        return 'Data inválida';
      }
      
      return date.toLocaleDateString('pt-BR');
    } catch (error) {
      console.error('Erro ao formatar data:', error, 'Data:', dateString);
      return 'Erro na data';
    }
  };

  const calculateTotal = () => {
    return transacoes.reduce((total, transacao) => {
      return transacao.tipo === 2 // 2 = Receita
        ? total + transacao.valor 
        : total - transacao.valor;
    }, 0);
  };

  const getReceitas = () => {
    return transacoes.filter(t => t.tipo === 2).reduce((sum, t) => sum + t.valor, 0);
  };

  const getDespesas = () => {
    return transacoes.filter(t => t.tipo === 1).reduce((sum, t) => sum + t.valor, 0);
  };

  return (
    <div className="info-card">
      <h2 className="card-title">
        <span className="nav-icon">💰</span> Gestão de Transações
      </h2>

      {message && (
        <div className={`message ${message.type}`}>
          {message.text}
        </div>
      )}

      {/* Resumo Financeiro */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '15px', marginBottom: '30px' }}>
        <div className="info-card" style={{ textAlign: 'center', padding: '15px' }}>
          <div style={{ color: 'var(--success-green)', fontSize: '1.1rem', fontWeight: '700' }}>
            RECEITAS
          </div>
          <div style={{ fontSize: '1.5rem', color: 'var(--neon-blue)' }}>
            {formatCurrency(getReceitas())}
          </div>
        </div>
        
        <div className="info-card" style={{ textAlign: 'center', padding: '15px' }}>
          <div style={{ color: 'var(--error-red)', fontSize: '1.1rem', fontWeight: '700' }}>
            DESPESAS
          </div>
          <div style={{ fontSize: '1.5rem', color: 'var(--neon-blue)' }}>
            {formatCurrency(getDespesas())}
          </div>
        </div>
        
        <div className="info-card" style={{ textAlign: 'center', padding: '15px' }}>
          <div style={{ color: 'var(--text-accent)', fontSize: '1.1rem', fontWeight: '700' }}>
            SALDO
          </div>
          <div style={{ 
            fontSize: '1.5rem', 
            color: calculateTotal() >= 0 ? 'var(--success-green)' : 'var(--error-red)' 
          }}>
            {formatCurrency(calculateTotal())}
          </div>
        </div>
      </div>

      <div style={{ marginBottom: '20px' }}>
        <button 
          className="btn btn-primary"
          onClick={() => setShowForm(!showForm)}
        >
          {showForm ? 'Cancelar' : '+ Nova Transação'}
        </button>
      </div>

      {showForm && (
        <form onSubmit={handleSubmit} className="info-card" style={{ marginBottom: '30px' }}>
          <h3 className="card-title">
            {editingTransacao ? 'Editar Transação' : 'Nova Transação'}
          </h3>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))', gap: '20px' }}>
            <div className="form-group">
              <label className="form-label">Descrição *</label>
              <input
                type="text"
                name="descricao"
                className={`form-input ${errors.descricao ? 'error' : ''}`}
                value={formData.descricao}
                onChange={handleInputChange}
                placeholder="Digite a descrição"
              />
              {errors.descricao && <span style={{ color: 'var(--error-red)', fontSize: '0.8rem' }}>{errors.descricao}</span>}
            </div>

            <div className="form-group">
              <label className="form-label">Valor *</label>
              <input
                type="number"
                name="valor"
                step="0.01"
                min="0"
                className={`form-input ${errors.valor ? 'error' : ''}`}
                value={formData.valor}
                onChange={handleInputChange}
                placeholder="0.00"
              />
              {errors.valor && <span style={{ color: 'var(--error-red)', fontSize: '0.8rem' }}>{errors.valor}</span>}
            </div>

            <div className="form-group">
              <label className="form-label">Data *</label>
              <input
                type="date"
                name="data"
                className={`form-input ${errors.data ? 'error' : ''}`}
                value={formData.data}
                onChange={handleInputChange}
              />
              {errors.data && <span style={{ color: 'var(--error-red)', fontSize: '0.8rem' }}>{errors.data}</span>}
            </div>

            <div className="form-group">
              <label className="form-label">Tipo *</label>
              <select
                name="tipo"
                className="form-select"
                value={formData.tipo}
                onChange={handleSelectChange}
              >
                <option value={1}>Despesa</option>
                <option value={2}>Receita</option>
              </select>
            </div>

            <div className="form-group">
              <label className="form-label">Pessoa</label>
              <select
                name="pessoaId"
                className="form-select"
                value={formData.pessoaId || ''}
                onChange={handleSelectChange}
              >
                <option value="">Selecione uma pessoa</option>
                {pessoas.map(pessoa => (
                  <option key={pessoa.id} value={pessoa.id}>
                    {pessoa.nome}
                  </option>
                ))}
              </select>
            </div>

            <div className="form-group">
              <label className="form-label">Categoria</label>
              <select
                name="categoriaId"
                className="form-select"
                value={formData.categoriaId || ''}
                onChange={handleSelectChange}
              >
                <option value="">Selecione uma categoria</option>
                {categorias.map(categoria => (
                  <option key={categoria.id} value={categoria.id}>
                    {categoria.descricao}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <div style={{ display: 'flex', gap: '10px', marginTop: '20px' }}>
            <button type="submit" className="btn btn-primary">
              {editingTransacao ? 'Atualizar' : 'Salvar'}
            </button>
            <button type="button" className="btn btn-secondary" onClick={resetForm}>
              Cancelar
            </button>
          </div>
        </form>
      )}

      {loading ? (
        <div className="loading">Carregando transações...</div>
      ) : (
        <div style={{ overflowX: 'auto' }}>
          <table className="data-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Descrição</th>
                <th>Valor</th>
                <th>Data</th>
                <th>Tipo</th>
                <th>Pessoa</th>
                <th>Categoria</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              {transacoes.map(transacao => (
                <tr key={transacao.id}>
                  <td>{transacao.id}</td>
                  <td>{transacao.descricao}</td>
                  <td style={{ 
                    color: transacao.tipo === 2 ? 'var(--success-green)' : 'var(--error-red)',
                    fontWeight: '600'
                  }}>
                    {formatCurrency(transacao.valor)}
                  </td>
                  <td>{formatDate(transacao.data)}</td>
                  <td>
                    <span style={{
                      padding: '4px 8px',
                      borderRadius: '4px',
                      fontSize: '0.8rem',
                      fontWeight: '600',
                      backgroundColor: transacao.tipo === 2 ? 'rgba(0, 255, 128, 0.2)' : 'rgba(255, 64, 64, 0.2)',
                      color: transacao.tipo === 2 ? 'var(--success-green)' : 'var(--error-red)',
                      border: `1px solid ${transacao.tipo === 2 ? 'var(--success-green)' : 'var(--error-red)'}`
                    }}>
                      {transacao.tipo === 1 ? 'DESPESA' : 'RECEITA'}
                    </span>
                  </td>
                  <td>{transacao.pessoa?.nome || '-'}</td>
                  <td>
                    {transacao.categoria ? (
                      <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                        {transacao.categoria.descricao}
                      </div>
                    ) : '-'}
                  </td>
                  <td>
                    <div style={{ display: 'flex', gap: '8px' }}>
                      <button 
                        className="btn btn-secondary" 
                        style={{ padding: '6px 12px', fontSize: '0.8rem' }}
                        onClick={() => handleEdit(transacao)}
                      >
                        Editar
                      </button>
                      <button 
                        className="btn btn-danger" 
                        style={{ padding: '6px 12px', fontSize: '0.8rem' }}
                        onClick={() => handleDelete(transacao.id!)}
                      >
                        Excluir
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          {transacoes.length === 0 && (
            <div style={{ textAlign: 'center', padding: '40px', color: 'var(--text-accent)' }}>
              Nenhuma transação cadastrada
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default TransacoesManager;
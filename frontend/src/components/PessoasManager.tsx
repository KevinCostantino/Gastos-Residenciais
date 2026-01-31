import React, { useState, useEffect } from 'react';
import { Pessoa, CreatePessoaDto, FormErrors } from '../types';
import { pessoasService, ApiError } from '../services/api';

const PessoasManager: React.FC = () => {
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [editingPessoa, setEditingPessoa] = useState<Pessoa | null>(null);
  const [message, setMessage] = useState<{ text: string; type: 'success' | 'error' | 'warning' } | null>(null);
  
  const [formData, setFormData] = useState<CreatePessoaDto>({
    nome: '',
    idade: 0
  });
  
  const [errors, setErrors] = useState<FormErrors>({});

  useEffect(() => {
    loadPessoas();
  }, []);

  const loadPessoas = async () => {
    try {
      setLoading(true);
      const data = await pessoasService.getAll();
      setPessoas(data as Pessoa[]);
      showMessage('Pessoas carregadas com sucesso', 'success');
    } catch (error) {
      console.error('Erro ao carregar pessoas:', error);
      showMessage('Erro ao carregar pessoas', 'error');
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
    
    if (!formData.nome.trim()) {
      newErrors.nome = 'Nome é obrigatório';
    }
    
    if (!formData.idade || formData.idade < 0 || formData.idade > 150) {
      newErrors.idade = 'Idade deve ser um número válido entre 0 e 150';
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
      if (editingPessoa) {
        await pessoasService.update(editingPessoa.id!, formData);
        showMessage('Pessoa atualizada com sucesso', 'success');
      } else {
        await pessoasService.create(formData);
        showMessage('Pessoa criada com sucesso', 'success');
      }
      
      resetForm();
      loadPessoas();
    } catch (error) {
      if (error instanceof ApiError) {
        showMessage(`Erro: ${error.message}`, 'error');
      } else {
        showMessage('Erro inesperado', 'error');
      }
    }
  };

  const handleEdit = (pessoa: Pessoa) => {
    setEditingPessoa(pessoa);
    setFormData({
      nome: pessoa.nome,
      idade: pessoa.idade
    });
    setShowForm(true);
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Tem certeza que deseja excluir esta pessoa?')) {
      return;
    }

    try {
      await pessoasService.delete(id);
      showMessage('Pessoa excluída com sucesso', 'success');
      loadPessoas();
    } catch (error) {
      if (error instanceof ApiError) {
        showMessage(`Erro ao excluir: ${error.message}`, 'error');
      } else {
        showMessage('Erro inesperado', 'error');
      }
    }
  };

  const resetForm = () => {
    setFormData({ nome: '', idade: 0 });
    setEditingPessoa(null);
    setShowForm(false);
    setErrors({});
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ 
      ...prev, 
      [name]: name === 'idade' ? parseInt(value) || 0 : value 
    }));
    
    // Limpar erro do campo quando usuário começar a digitar
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  return (
    <div className="electronic-panel">
      <div className="panel-header">
        <h2 className="panel-title">
          <span className="title-bracket">[</span>
          GESTÃO DE PESSOAS
          <span className="title-bracket">]</span>
        </h2>
      </div>

      {message && (
        <div className={`message ${message.type}`}>
          {message.text}
        </div>
      )}

      <div className="panel-content">
        <div style={{ marginBottom: '20px' }}>
          <button 
            className="btn btn-primary"
            onClick={() => setShowForm(!showForm)}
          >
            {showForm ? 'Cancelar' : 'Nova Pessoa'}
          </button>
        </div>

        {showForm && (
          <form onSubmit={handleSubmit} style={{ marginBottom: '30px' }}>
            <div className="form-group">
              <label className="form-label">Nome *</label>
              <input
                type="text"
                name="nome"
                className={`form-input ${errors.nome ? 'error' : ''}`}
                value={formData.nome}
                onChange={handleInputChange}
                placeholder="Digite o nome"
              />
              {errors.nome && <span style={{ color: 'var(--error-red)', fontSize: '0.8rem' }}>{errors.nome}</span>}
            </div>

            <div className="form-group">
              <label className="form-label">Idade *</label>
              <input
                type="number"
                name="idade"
                className={`form-input ${errors.idade ? 'error' : ''}`}
                value={formData.idade}
                onChange={handleInputChange}
                placeholder="Digite a idade"
                min="0"
                max="150"
              />
              {errors.idade && <span style={{ color: 'var(--error-red)', fontSize: '0.8rem' }}>{errors.idade}</span>}
            </div>

            <div style={{ display: 'flex', gap: '10px' }}>
              <button type="submit" className="btn btn-primary">
                {editingPessoa ? 'Atualizar' : 'Salvar'}
              </button>
              <button type="button" className="btn btn-secondary" onClick={resetForm}>
                Cancelar
              </button>
            </div>
          </form>
        )}

        {loading ? (
          <div className="loading">Carregando pessoas...</div>
        ) : (
          <div style={{ overflowX: 'auto' }}>
            <table className="data-table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Nome</th>
                  <th>Idade</th>
                  <th>Status</th>
                  <th>Transações</th>
                  <th>Ações</th>
                </tr>
              </thead>
              <tbody>
                {pessoas.map(pessoa => (
                  <tr key={pessoa.id}>
                    <td>{pessoa.id}</td>
                    <td>{pessoa.nome}</td>
                    <td>{pessoa.idade} anos</td>
                    <td>
                      <span className={`status-badge ${pessoa.isMenorDeIdade ? 'minor' : 'adult'}`}>
                        {pessoa.isMenorDeIdade ? 'Menor de idade' : 'Maior de idade'}
                      </span>
                    </td>
                    <td>{pessoa.totalTransacoes || 0}</td>
                    <td>
                      <div style={{ display: 'flex', gap: '8px' }}>
                        <button 
                          className="btn btn-secondary" 
                          style={{ padding: '6px 12px', fontSize: '0.8rem' }}
                          onClick={() => handleEdit(pessoa)}
                        >
                          Editar
                        </button>
                        <button 
                          className="btn btn-danger" 
                          style={{ padding: '6px 12px', fontSize: '0.8rem' }}
                          onClick={() => handleDelete(pessoa.id!)}
                        >
                          Excluir
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>

            {pessoas.length === 0 && (
              <div style={{ textAlign: 'center', padding: '40px', color: 'var(--text-accent)' }}>
                Nenhuma pessoa cadastrada
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default PessoasManager;
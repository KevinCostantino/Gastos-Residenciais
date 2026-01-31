import React, { useState, useEffect } from 'react';
import { Categoria, CreateCategoriaDto, FormErrors } from '../types';
import { categoriasService, ApiError } from '../services/api';

const CategoriasManager: React.FC = () => {
  const [categorias, setCategorias] = useState<Categoria[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [editingCategoria, setEditingCategoria] = useState<Categoria | null>(null);
  const [message, setMessage] = useState<{ text: string; type: 'success' | 'error' | 'warning' } | null>(null);
  
  const [formData, setFormData] = useState<CreateCategoriaDto>({
    descricao: '',
    finalidade: 1
  });
  
  const [errors, setErrors] = useState<FormErrors>({});

  useEffect(() => {
    loadCategorias();
  }, []);

  const loadCategorias = async () => {
    try {
      setLoading(true);
      const data = await categoriasService.getAll();
      setCategorias(data);
      showMessage('Categorias carregadas com sucesso', 'success');
    } catch (error) {
      console.error('Erro ao carregar categorias:', error);
      showMessage('Erro ao carregar categorias', 'error');
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

    if (!formData.finalidade || ![1, 2, 3].includes(formData.finalidade)) {
      newErrors.finalidade = 'Finalidade deve ser selecionada';
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
      if (editingCategoria) {
        await categoriasService.update(editingCategoria.id!, formData);
        showMessage('Categoria atualizada com sucesso', 'success');
      } else {
        await categoriasService.create(formData);
        showMessage('Categoria criada com sucesso', 'success');
      }
      
      resetForm();
      loadCategorias();
    } catch (error) {
      if (error instanceof ApiError) {
        showMessage(`Erro: ${error.message}`, 'error');
      } else {
        showMessage('Erro inesperado', 'error');
      }
    }
  };

  const handleEdit = (categoria: Categoria) => {
    setEditingCategoria(categoria);
    setFormData({
      descricao: categoria.descricao,
      finalidade: categoria.finalidade
    });
    setShowForm(true);
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Tem certeza que deseja excluir esta categoria?')) {
      return;
    }

    try {
      await categoriasService.delete(id);
      showMessage('Categoria excluída com sucesso', 'success');
      loadCategorias();
    } catch (error) {
      if (error instanceof ApiError) {
        showMessage(`Erro ao excluir: ${error.message}`, 'error');
      } else {
        showMessage('Erro inesperado', 'error');
      }
    }
  };

  const resetForm = () => {
    setFormData({ descricao: '', finalidade: 1 });
    setEditingCategoria(null);
    setShowForm(false);
    setErrors({});
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    
    // Limpar erro do campo quando usuário começar a digitar
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  const handleSelectChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: parseInt(value) }));
    
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
          GESTÃO DE CATEGORIAS
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
            {showForm ? 'Cancelar' : 'Nova Categoria'}
          </button>
        </div>

        {showForm && (
          <form onSubmit={handleSubmit} style={{ marginBottom: '30px' }}>
            <div className="form-group">
              <label className="form-label">Descrição *</label>
              <input
                type="text"
                name="descricao"
                className={`form-input ${errors.descricao ? 'error' : ''}`}
                value={formData.descricao}
                onChange={handleInputChange}
                placeholder="Digite a descrição da categoria"
              />
              {errors.descricao && <span style={{ color: 'var(--error-red)', fontSize: '0.8rem' }}>{errors.descricao}</span>}
            </div>

            <div className="form-group">
              <label className="form-label">Finalidade *</label>
              <select
                name="finalidade"
                className={`form-input ${errors.finalidade ? 'error' : ''}`}
                value={formData.finalidade}
                onChange={handleSelectChange}
              >
                <option value={1}>Despesa</option>
                <option value={2}>Receita</option>
                <option value={3}>Ambas</option>
              </select>
              {errors.finalidade && <span style={{ color: 'var(--error-red)', fontSize: '0.8rem' }}>{errors.finalidade}</span>}
            </div>

            <div style={{ display: 'flex', gap: '10px' }}>
              <button type="submit" className="btn btn-primary">
                {editingCategoria ? 'Atualizar' : 'Salvar'}
              </button>
              <button type="button" className="btn btn-secondary" onClick={resetForm}>
                Cancelar
              </button>
            </div>
          </form>
        )}

        {loading ? (
          <div className="loading">Carregando categorias...</div>
        ) : (
          <div style={{ overflowX: 'auto' }}>
            <table className="data-table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Descrição</th>
                  <th>Finalidade</th>
                  <th>Transações</th>
                  <th>Ações</th>
                </tr>
              </thead>
              <tbody>
                {categorias.map(categoria => (
                  <tr key={categoria.id}>
                    <td>{categoria.id}</td>
                    <td>{categoria.descricao}</td>
                    <td>
                      <span className={`finalidade-badge finalidade-${categoria.finalidade}`}>
                        {categoria.finalidade === 1 ? 'Despesa' : 
                         categoria.finalidade === 2 ? 'Receita' : 'Ambas'}
                      </span>
                    </td>
                    <td>{categoria.totalTransacoes || 0}</td>
                    <td>
                      <div style={{ display: 'flex', gap: '8px' }}>
                        <button 
                          className="btn btn-secondary" 
                          style={{ padding: '6px 12px', fontSize: '0.8rem' }}
                          onClick={() => handleEdit(categoria)}
                        >
                          Editar
                        </button>
                        <button 
                          className="btn btn-danger" 
                          style={{ padding: '6px 12px', fontSize: '0.8rem' }}
                          onClick={() => handleDelete(categoria.id!)}
                        >
                          Excluir
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>

            {categorias.length === 0 && (
              <div style={{ textAlign: 'center', padding: '40px', color: 'var(--text-accent)' }}>
                Nenhuma categoria cadastrada
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default CategoriasManager;
import React, { useState, useEffect } from 'react';
import { Transacao, Pessoa, Categoria } from '../types';
import { transacoesService, pessoasService, categoriasService } from '../services/api';

const RelatoriosManager: React.FC = () => {
  const [transacoes, setTransacoes] = useState<Transacao[]>([]);
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [categorias, setCategorias] = useState<Categoria[]>([]);
  const [loading, setLoading] = useState(true);

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
    } catch (error) {
      console.error('Erro ao carregar dados para relatórios:', error);
    } finally {
      setLoading(false);
    }
  };

  const calcularEstatisticas = () => {
    const receitas = transacoes.filter(t => t.tipo === 2);
    const despesas = transacoes.filter(t => t.tipo === 1);
    
    const totalReceitas = receitas.reduce((sum, t) => sum + t.valor, 0);
    const totalDespesas = despesas.reduce((sum, t) => sum + t.valor, 0);
    const saldo = totalReceitas - totalDespesas;

    return {
      totalReceitas,
      totalDespesas,
      saldo,
      totalTransacoes: transacoes.length,
      totalPessoas: pessoas.length,
      totalCategorias: categorias.length
    };
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('pt-BR');
  };

  const estatisticas = calcularEstatisticas();

  if (loading) {
    return (
      <div className="electronic-panel">
        <div className="panel-header">
          <h2 className="panel-title">
            <span className="title-bracket">[</span>
            CARREGANDO RELATÓRIOS
            <span className="title-bracket">]</span>
          </h2>
        </div>
        <div className="loading-container">
          <div className="loading-text">PROCESSANDO DADOS...</div>
        </div>
      </div>
    );
  }

  return (
    <div className="electronic-panel">
      <div className="panel-header">
        <h2 className="panel-title">
          <span className="title-bracket">[</span>
          RELATÓRIOS FINANCEIROS
          <span className="title-bracket">]</span>
        </h2>
      </div>

      <div className="panel-content">
        {/* Estatísticas Gerais */}
        <div className="report-section">
          <h3 className="section-title">
            <span className="section-bracket">►</span>
            ESTATÍSTICAS GERAIS
          </h3>
          
          <div className="stats-grid">
            <div className="stat-card">
              <div className="stat-label">TOTAL RECEITAS</div>
              <div className="stat-value positive">{formatCurrency(estatisticas.totalReceitas)}</div>
            </div>
            
            <div className="stat-card">
              <div className="stat-label">TOTAL DESPESAS</div>
              <div className="stat-value negative">{formatCurrency(estatisticas.totalDespesas)}</div>
            </div>
            
            <div className="stat-card">
              <div className="stat-label">SALDO ATUAL</div>
              <div className={`stat-value ${estatisticas.saldo >= 0 ? 'positive' : 'negative'}`}>
                {formatCurrency(estatisticas.saldo)}
              </div>
            </div>
            
            <div className="stat-card">
              <div className="stat-label">TRANSAÇÕES</div>
              <div className="stat-value">{estatisticas.totalTransacoes}</div>
            </div>
            
            <div className="stat-card">
              <div className="stat-label">PESSOAS</div>
              <div className="stat-value">{estatisticas.totalPessoas}</div>
            </div>
            
            <div className="stat-card">
              <div className="stat-label">CATEGORIAS</div>
              <div className="stat-value">{estatisticas.totalCategorias}</div>
            </div>
          </div>
        </div>

        {/* Transações por Pessoa */}
        <div className="report-section">
          <h3 className="section-title">
            <span className="section-bracket">►</span>
            TRANSAÇÕES POR PESSOA
          </h3>
          
          <div className="data-table">
            <div className="table-header">
              <div>PESSOA</div>
              <div>RECEITAS</div>
              <div>DESPESAS</div>
              <div>SALDO</div>
            </div>
            
            {pessoas.map(pessoa => {
              const transacoesPessoa = transacoes.filter(t => t.pessoaId === pessoa.id);
              const receitasPessoa = transacoesPessoa.filter(t => t.tipo === 2).reduce((sum, t) => sum + t.valor, 0);
              const despesasPessoa = transacoesPessoa.filter(t => t.tipo === 1).reduce((sum, t) => sum + t.valor, 0);
              const saldoPessoa = receitasPessoa - despesasPessoa;
              
              return (
                <div key={pessoa.id} className="table-row">
                  <div className="person-info">
                    {pessoa.nome} <small>({pessoa.idade} anos)</small>
                  </div>
                  <div className="value positive">{formatCurrency(receitasPessoa)}</div>
                  <div className="value negative">{formatCurrency(despesasPessoa)}</div>
                  <div className={`value ${saldoPessoa >= 0 ? 'positive' : 'negative'}`}>
                    {formatCurrency(saldoPessoa)}
                  </div>
                </div>
              );
            })}
          </div>
        </div>

        {/* Transações por Categoria */}
        <div className="report-section">
          <h3 className="section-title">
            <span className="section-bracket">►</span>
            TRANSAÇÕES POR CATEGORIA
          </h3>
          
          <div className="data-table">
            <div className="table-header">
              <div>CATEGORIA</div>
              <div>TIPO</div>
              <div>TRANSAÇÕES</div>
              <div>VALOR TOTAL</div>
            </div>
            
            {categorias.map(categoria => {
              const transacoesCategoria = transacoes.filter(t => t.categoriaId === categoria.id);
              const valorTotal = transacoesCategoria.reduce((sum, t) => sum + t.valor, 0);
              
              return (
                <div key={categoria.id} className="table-row">
                  <div>{categoria.descricao}</div>
                  <div>
                    {categoria.finalidade === 1 ? 'Despesa' : 
                     categoria.finalidade === 2 ? 'Receita' : 'Ambas'}
                  </div>
                  <div>{transacoesCategoria.length}</div>
                  <div className="value">{formatCurrency(valorTotal)}</div>
                </div>
              );
            })}
          </div>
        </div>

        {/* Últimas Transações */}
        <div className="report-section">
          <h3 className="section-title">
            <span className="section-bracket">►</span>
            ÚLTIMAS TRANSAÇÕES
          </h3>
          
          <div className="data-table">
            <div className="table-header">
              <div>DATA</div>
              <div>DESCRIÇÃO</div>
              <div>TIPO</div>
              <div>VALOR</div>
            </div>
            
            {transacoes
              .sort((a, b) => new Date(b.data).getTime() - new Date(a.data).getTime())
              .slice(0, 10)
              .map(transacao => (
                <div key={transacao.id} className="table-row">
                  <div>{formatDate(transacao.data)}</div>
                  <div>{transacao.descricao}</div>
                  <div className={`type-badge ${transacao.tipo === 1 ? 'despesa' : 'receita'}`}>
                    {transacao.tipo === 1 ? 'DESPESA' : 'RECEITA'}
                  </div>
                  <div className={`value ${transacao.tipo === 1 ? 'negative' : 'positive'}`}>
                    {formatCurrency(transacao.valor)}
                  </div>
                </div>
              ))
            }
          </div>
        </div>
      </div>
    </div>
  );
};

export default RelatoriosManager;
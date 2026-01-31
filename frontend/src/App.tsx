import React, { useState } from 'react';
import './App.css';
import PessoasManager from './components/PessoasManager';
import CategoriasManager from './components/CategoriasManager';
import TransacoesManager from './components/TransacoesManager';
import RelatoriosManager from './components/RelatoriosManager';

type ActiveTab = 'pessoas' | 'categorias' | 'transacoes' | 'relatorios';

function App() {
  const [activeTab, setActiveTab] = useState<ActiveTab>('pessoas');

  return (
    <div className="electronic-container">
      <div className="scan-line"></div>
      <div className="grid-bg"></div>
      
      <header className="system-header">
        <h1 className="system-title">
          <span className="title-bracket">[</span>
          SISTEMA ELETRÔNICO DE CONTROLE
          <span className="title-bracket">]</span>
        </h1>
        <div className="status-bar">
          <span className="status-indicator active">●</span>
          <span className="status-text">SISTEMA OPERACIONAL</span>
        </div>
      </header>

      <nav className="navigation-panel">
        <button 
          className={`nav-button ${activeTab === 'pessoas' ? 'active' : ''}`}
          onClick={() => setActiveTab('pessoas')}
        >
          <span className="nav-icon">👤</span>
          PESSOAS
        </button>
        <button 
          className={`nav-button ${activeTab === 'categorias' ? 'active' : ''}`}
          onClick={() => setActiveTab('categorias')}
        >
          <span className="nav-icon">📂</span>
          CATEGORIAS
        </button>
        <button 
          className={`nav-button ${activeTab === 'transacoes' ? 'active' : ''}`}
          onClick={() => setActiveTab('transacoes')}
        >
          <span className="nav-icon">💰</span>
          TRANSAÇÕES
        </button>
        <button 
          className={`nav-button ${activeTab === 'relatorios' ? 'active' : ''}`}
          onClick={() => setActiveTab('relatorios')}
        >
          <span className="nav-icon">📊</span>
          RELATÓRIOS
        </button>
      </nav>

      <main className="content-area">
        <div className="module-container">
          {activeTab === 'pessoas' && <PessoasManager />}
          {activeTab === 'categorias' && <CategoriasManager />}
          {activeTab === 'transacoes' && <TransacoesManager />}
          {activeTab === 'relatorios' && <RelatoriosManager />}
        </div>
      </main>

      <footer className="system-footer">
        <div className="footer-info">
          <span>VERSÃO 1.1.6</span>
          <span>REACT + TYPESCRIPT</span>
          <span>STATUS: ATIVO</span>
        </div>
      </footer>
    </div>
  );
}

export default App;

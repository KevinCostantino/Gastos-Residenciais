import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import reportWebVitals from './reportWebVitals';

// Aguarda o DOM estar pronto e verifica se o elemento root existe
const initializeApp = () => {
  const rootElement = document.getElementById('root');
  
  if (!rootElement) {
    console.error('❌ Elemento root não encontrado no DOM!');
    document.body.innerHTML = `
      <div style="
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
        height: 100vh;
        background: linear-gradient(135deg, #000011 0%, #001122 100%);
        color: #ff4040;
        font-family: 'JetBrains Mono', monospace;
        text-align: center;
      ">
        <h1 style="margin-bottom: 20px;">⚠️ ERRO DO SISTEMA</h1>
        <p>Elemento 'root' não encontrado no DOM</p>
        <p style="margin-top: 10px; font-size: 0.9rem; opacity: 0.8;">
          Verifique se o index.html está correto
        </p>
      </div>
    `;
    return;
  }

  const root = ReactDOM.createRoot(rootElement);
  
  root.render(
    <React.StrictMode>
      <App />
    </React.StrictMode>
  );
};

// Aguarda o DOM estar completamente carregado
if (document.readyState === 'loading') {
  document.addEventListener('DOMContentLoaded', initializeApp);
} else {
  initializeApp();
}

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();

# Sistema Eletrônico de Controle de Gastos Residenciais

## Descrição

Sistema completo para controle de gastos residenciais desenvolvido em .NET 9.0 e React com TypeScript.

## Tecnologias

**Backend:** .NET 9.0 + Entity Framework + SQLite  
**Frontend:** React 18 + TypeScript + CSS3  
**API:** REST com CORS habilitado

## Arquitetura

```
├── backend/                       # API .NET 9.0
│   ├── Controllers/               # REST Controllers
│   ├── Models/                    # Entidades
│   ├── DTOs/                     # Data Transfer Objects
│   └── Data/                     # Entity Framework Context
├── frontend/                      # React + TypeScript
│   ├── src/components/           # Componentes React
│   ├── src/services/             # API Services
│   └── src/types/                # TypeScript Types
└── README.md
```

## Execução Rápida

**Backend:** `cd backend && dotnet run` → http://localhost:5118  
**Frontend:** `cd frontend && npm install && npm start` → http://localhost:3000

## Funcionalidades

### 👥 Pessoas
- CRUD completo com validações
- Exclusão cascata de transações
- Relatório de totais

### 📊 Categorias  
- Finalidades: Despesa, Receita, Ambas
- Proteção contra exclusão em uso
- Relatório por categoria

### 💰 Transações
- Registro de receitas/despesas
- Validação por idade (menores só despesas)
- Filtros e estatísticas

### 📈 Relatórios
- Dashboard com totais gerais
- Relatórios por pessoa/categoria
- Transações recentes

## API 

### Pessoas
- `GET /api/pessoas` - Lista todas as pessoas
- `GET /api/pessoas/{id}` - Obtém pessoa específica
- `POST /api/pessoas` - Cria nova pessoa
- `PUT /api/pessoas/{id}` - Atualiza pessoa existente
- `DELETE /api/pessoas/{id}` - Remove pessoa e suas transações
- `GET /api/pessoas/relatorio-totais` - Relatório de totais por pessoa

### Categorias
- `GET /api/categorias` - Lista todas as categorias
- `GET /api/categorias/{id}` - Obtém categoria específica
- `POST /api/categorias` - Cria nova categoria
- `GET /api/categorias/por-tipo/{tipo}` - Categorias filtradas por tipo
- `GET /api/categorias/relatorio-totais` - Relatório de totais por categoria
- `GET /api/categorias/{id}/pode-remover` - Verifica se categoria pode ser removida

### Transações
- `GET /api/transacoes` - Lista transações (com filtros opcionais)
- `GET /api/transacoes/{id}` - Obtém transação específica
- `POST /api/transacoes` - Cria nova transação
- `GET /api/transacoes/estatisticas` - Estatísticas gerais
- `GET /api/transacoes/recentes` - Últimas transações
- `GET /api/transacoes/validar` - Valida parâmetros de transação

## Executando o Sistema

### Pré-requisitos
- .NET 9.0 SDK
- Node.js 18+ com npm

### 🔧 Iniciando o Backend
```bash
cd backend
dotnet restore
dotnet run
```
**API disponível em:** `http://localhost:5118`  
**Documentação Swagger:** `http://localhost:5118/swagger`

### 🎨 Iniciando o Frontend
```bash
cd frontend
npm install
npm start
```

## Validações Implementadas

### Front-end
- Validação de formulários em tempo real
- Feedback visual de erros
- Confirmação de ações críticas (exclusões)

### Back-end
- Validação de regras de negócio
- Validação de integridade referencial
- Mensagens de tratamento de erros 


## 🔧 Solução de Problemas

### Problemas de CORS
Se encontrar erros de CORS, verifique:
- Backend rodando em `localhost:5118`
- Frontend rodando em `localhost:3000`
- CORS configurado no backend para porta 3000

### Problemas de Dependências
```bash
# Limpar cache e reinstalar
cd frontend
rm -rf node_modules package-lock.json
npm install
```

## Recursos 

- **🖥️ Interface Principal**: http://localhost:3000 
- **⚙️ API Backend**: http://localhost:5118
- **📄 Documentação API**: http://localhost:5118/swagger

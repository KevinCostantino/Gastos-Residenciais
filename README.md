# Sistema Eletrônico de Controle de Gastos Residenciais

## Descrição

Sistema para controle de gastos residenciais com funcionalidades de cadastro de pessoas, categorias e transações financeiras.

## Tecnologias Utilizadas

### Back-end
- **C# .NET 9.0** - Framework principal
- **ASP.NET Core** - API REST
- **Entity Framework Core 9.0** - ORM para acesso ao banco de dados
- **SQLite** - Banco de dados para persistência
- **CORS** - Configurado para comunicação com frontend

### Front-end
- **React 18 + TypeScript** - Interface principal 
- **CSS3** - Estilização 
- **Hooks e Context** - Gerenciamento de estado


## Arquitetura

### Estrutura Reorganizada

```
Teste técnico - Desenvolvedor Full Stack/
├── 🔧 backend/                    # API .NET Core
│   ├── Controllers/               # Controllers REST
│   ├── Models/                    # Entidades do domínio
│   ├── DTOs/                     # Data Transfer Objects
│   ├── Data/                     # Contexto Entity Framework
│   ├── GastosResiduenciais.sln   # Solution Visual Studio
│   └── *.cs, *.csproj            # Arquivos do projeto
├── 🎨 frontend/                   # Interface React + TypeScript
│   ├── src/                      # Código fonte principal
│   │   ├── components/           # Componentes React
│   │   ├── types/                # Definições TypeScript
│   │   ├── services/             # APIs e serviços
│   │   └── App.tsx               # Componente principal
│   ├── public/                   # Arquivos estáticos
│   ├── package.json              # Dependências npm
│   └── tsconfig.json             # Configuração TypeScript
├── 📄 .gitignore                 # Arquivos ignorados pelo Git
└── 📚 README.md                  # Esta documentação
```

## Funcionalidades Implementadas

### 1. Cadastro de Pessoas
- **Create** - Criação de nova pessoa
- **Read** - Listagem e consulta individual
- **Update** - Atualização de dados existentes
- **Delete** - Remoção (com exclusão em cascata das transações)

#### Regras de Negócio:
- Nome único 
- Nome limitado a 200 caracteres
- Idade obrigatória (0-150 anos)
- Ao deletar pessoa, todas suas transações são removidas

### 2. Cadastro de Categorias
- **Create** - Criação de nova categoria
- **Read** - Listagem e consulta individual

#### Regras de Negócio:
- Descrição única 
- Descrição limitada a 400 caracteres
- Finalidade obrigatória: Despesa, Receita ou Ambas
- Categorias em uso não podem ser removidas

### 3. Cadastro de Transações
- **Create** - Registro de nova transação
- **Read** - Listagem com filtros opcionais

#### Regras de Negócio:
- Descrição obrigatória (máximo 400 caracteres)
- Valor obrigatório e positivo
- Tipo obrigatório (Despesa ou Receita)
- Menores de idade só podem registrar despesas
- Categoria deve ser compatível com o tipo de transação
- Data de criação automática

### 4. Consulta de Totais por Pessoa
- Lista todas as pessoas com:
  - Total de receitas
  - Total de despesas  
  - Saldo líquido (receitas - despesas)
- Exibe totais gerais de todo o sistema

### 5. Consulta de Totais por Categoria (Opcional)
- Lista todas as categorias com:
  - Total de receitas
  - Total de despesas
  - Saldo líquido
- Exibe totais gerais por categoria

## API Endpoints

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

## 🚀 Executando o Sistema

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
**Interface React disponível em:** `http://localhost:3000`

### 🚀 Execução Completa
```bash
# Terminal 1 - Backend
cd backend && dotnet run

# Terminal 2 - Frontend
cd frontend && npm start

# Acesse: http://localhost:3000
```

## Dados Iniciais

O sistema inclui categorias padrão para facilitar o uso:
- **Despesas**: Alimentação, Transporte, Moradia, Saúde, Educação, Lazer
- **Receitas**: Salário, Freelance, Investimentos
- **Ambas**: Outros

## Validações Implementadas

### Front-end
- Validação de formulários em tempo real
- Feedback visual de erros
- Confirmação de ações críticas (exclusões)

### Back-end
- Validação de regras de negócio
- Validação de integridade referencial
- Tratamento de erros com mensagens claras

## 🔧 Solução de Problemas

### ⚠️ Problemas de CORS
Se encontrar erros de CORS, verifique:
- Backend rodando em `localhost:5118`
- Frontend rodando em `localhost:3000`
- CORS configurado no backend para porta 3000

### 🔌 Conflitos de Porta
**Backend (porta 5118 ocupada):**
```bash
netstat -ano | findstr :5118
# Finalizar processo se necessário
```

**Frontend (porta 3000 ocupada):**
```bash
# React start automaticamente escolhe porta alternativa
npm start
```

### 📦 Problemas de Dependências
```bash
# Limpar cache e reinstalar
cd frontend
rm -rf node_modules package-lock.json
npm install
```

### ⚡ Erro createRoot DOM Element
Se encontrar erro "Target container is not a DOM element":
- ✅ **CORRIGIDO**: index.html atualizado com elemento `<div id="root"></div>`
- ✅ **CORRIGIDO**: index.tsx com validação robusta do DOM
- Interface HTML original mantida como backup em `index_html_original.html`


## 🎯 Recursos Disponíveis

- **🖥️ Interface Principal**: http://localhost:3000 
- **⚙️ API Backend**: http://localhost:5118
- **📄 Documentação API**: http://localhost:5118/swagger

## Autor

Sistema desenvolvido como teste técnico para vaga de Desenvolvedor Full Stack.
Data: Janeiro 2026
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GastosResiduenciais.Api.Data;
using GastosResiduenciais.Api.Models;
using GastosResiduenciais.Api.DTOs;

namespace GastosResiduenciais.Api.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de transações no sistema.
    /// Implementa operações de criação e listagem com validações de regras de negócio.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TransacoesController : ControllerBase
    {
        private readonly GastosResiduenciaisContext _context;

        /// <summary>
        /// Construtor do controller que recebe o contexto do banco de dados via injeção de dependência.
        /// </summary>
        /// <param name="context">Contexto do Entity Framework</param>
        public TransacoesController(GastosResiduenciaisContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todas as transações cadastradas no sistema.
        /// </summary>
        /// <param name="pessoaId">Filtro opcional por pessoa</param>
        /// <param name="categoriaId">Filtro opcional por categoria</param>
        /// <param name="tipo">Filtro opcional por tipo (1=Despesa, 2=Receita)</param>
        /// <returns>Lista de transações com informações completas</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransacaoDTO>>> GetTransacoes(
            [FromQuery] int? pessoaId = null,
            [FromQuery] int? categoriaId = null,
            [FromQuery] int? tipo = null)
        {
            var query = _context.Transacoes
                .Include(t => t.Pessoa)
                .Include(t => t.Categoria)
                .AsQueryable();

            // Aplicar filtros opcionais
            if (pessoaId.HasValue)
                query = query.Where(t => t.PessoaId == pessoaId.Value);

            if (categoriaId.HasValue)
                query = query.Where(t => t.CategoriaId == categoriaId.Value);

            if (tipo.HasValue && Enum.IsDefined(typeof(TipoTransacao), tipo.Value))
                query = query.Where(t => (int)t.Tipo == tipo.Value);

            var transacoes = await query
                .Select(t => new TransacaoDTO
                {
                    Id = t.Id,
                    Descricao = t.Descricao,
                    Valor = t.Valor,
                    Tipo = t.Tipo,
                    TipoDescricao = t.Tipo == TipoTransacao.Despesa ? "Despesa" : "Receita",
                    DataCriacao = t.DataCriacao,
                    CategoriaId = t.CategoriaId,
                    CategoriaNome = t.Categoria.Descricao,
                    PessoaId = t.PessoaId,
                    PessoaNome = t.Pessoa.Nome,
                    ValorComSinal = t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor
                })
                .OrderByDescending(t => t.DataCriacao)
                .ToListAsync();

            return Ok(transacoes);
        }

        /// <summary>
        /// Obtém uma transação específica pelo ID.
        /// </summary>
        /// <param name="id">Identificador único da transação</param>
        /// <returns>Dados completos da transação ou NotFound se não existir</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TransacaoDTO>> GetTransacao(int id)
        {
            var transacao = await _context.Transacoes
                .Include(t => t.Pessoa)
                .Include(t => t.Categoria)
                .Where(t => t.Id == id)
                .Select(t => new TransacaoDTO
                {
                    Id = t.Id,
                    Descricao = t.Descricao,
                    Valor = t.Valor,
                    Tipo = t.Tipo,
                    TipoDescricao = t.Tipo == TipoTransacao.Despesa ? "Despesa" : "Receita",
                    DataCriacao = t.DataCriacao,
                    CategoriaId = t.CategoriaId,
                    CategoriaNome = t.Categoria.Descricao,
                    PessoaId = t.PessoaId,
                    PessoaNome = t.Pessoa.Nome,
                    ValorComSinal = t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor
                })
                .FirstOrDefaultAsync();

            if (transacao == null)
            {
                return NotFound($"Transação com ID {id} não foi encontrada.");
            }

            return Ok(transacao);
        }

        /// <summary>
        /// Cria uma nova transação no sistema.
        /// Aplica todas as regras de negócio: validação de menor de idade, compatibilidade de categoria, etc.
        /// </summary>
        /// <param name="criarTransacaoDTO">Dados da transação a ser criada</param>
        /// <returns>Dados da transação criada com ID gerado</returns>
        [HttpPost]
        public async Task<ActionResult<TransacaoDTO>> CreateTransacao(CriarTransacaoDTO criarTransacaoDTO)
        {
            // Validação do enum de tipo
            if (!Enum.IsDefined(typeof(TipoTransacao), criarTransacaoDTO.Tipo))
            {
                return BadRequest("Tipo deve ser: 1 = Despesa, 2 = Receita");
            }

            // Buscar pessoa e verificar se existe
            var pessoa = await _context.Pessoas.FindAsync(criarTransacaoDTO.PessoaId);
            if (pessoa == null)
            {
                return BadRequest($"Pessoa com ID {criarTransacaoDTO.PessoaId} não foi encontrada.");
            }

            // Buscar categoria e verificar se existe
            var categoria = await _context.Categorias.FindAsync(criarTransacaoDTO.CategoriaId);
            if (categoria == null)
            {
                return BadRequest($"Categoria com ID {criarTransacaoDTO.CategoriaId} não foi encontrada.");
            }

            // REGRA DE NEGÓCIO: Menor de idade só pode registrar despesas
            if (pessoa.IsMenorDeIdade && criarTransacaoDTO.Tipo == TipoTransacao.Receita)
            {
                return BadRequest($"Pessoa menor de idade ({pessoa.Nome}, {pessoa.Idade} anos) só pode registrar despesas.");
            }

            // REGRA DE NEGÓCIO: Validar compatibilidade da categoria com o tipo de transação
            if (!categoria.PermiteTipoTransacao(criarTransacaoDTO.Tipo))
            {
                var finalidadeDesc = categoria.Finalidade == TipoFinalidade.Despesa ? "despesas" :
                                   categoria.Finalidade == TipoFinalidade.Receita ? "receitas" : "ambas";
                var tipoDesc = criarTransacaoDTO.Tipo == TipoTransacao.Despesa ? "despesa" : "receita";
                
                return BadRequest($"A categoria '{categoria.Descricao}' só permite {finalidadeDesc}, mas você está tentando registrar uma {tipoDesc}.");
            }

            // Criação da nova transação
            var transacao = new Transacao
            {
                Descricao = criarTransacaoDTO.Descricao.Trim(),
                Valor = criarTransacaoDTO.Valor,
                Tipo = criarTransacaoDTO.Tipo,
                CategoriaId = criarTransacaoDTO.CategoriaId,
                PessoaId = criarTransacaoDTO.PessoaId,
                DataCriacao = DateTime.Now
            };

            _context.Transacoes.Add(transacao);
            await _context.SaveChangesAsync();

            // Buscar a transação criada com as informações completas
            var transacaoCompleta = await _context.Transacoes
                .Include(t => t.Pessoa)
                .Include(t => t.Categoria)
                .Where(t => t.Id == transacao.Id)
                .Select(t => new TransacaoDTO
                {
                    Id = t.Id,
                    Descricao = t.Descricao,
                    Valor = t.Valor,
                    Tipo = t.Tipo,
                    TipoDescricao = t.Tipo == TipoTransacao.Despesa ? "Despesa" : "Receita",
                    DataCriacao = t.DataCriacao,
                    CategoriaId = t.CategoriaId,
                    CategoriaNome = t.Categoria.Descricao,
                    PessoaId = t.PessoaId,
                    PessoaNome = t.Pessoa.Nome,
                    ValorComSinal = t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetTransacao), new { id = transacao.Id }, transacaoCompleta);
        }

        /// <summary>
        /// Obtém estatísticas resumidas das transações.
        /// </summary>
        /// <returns>Totais de transações, receitas, despesas e saldo geral</returns>
        [HttpGet("estatisticas")]
        public async Task<ActionResult<object>> GetEstatisticas()
        {
            var totalTransacoes = await _context.Transacoes.CountAsync();
            var totalReceitas = await _context.Transacoes
                .Where(t => t.Tipo == TipoTransacao.Receita)
                .SumAsync(t => (decimal?)t.Valor) ?? 0;
            var totalDespesas = await _context.Transacoes
                .Where(t => t.Tipo == TipoTransacao.Despesa)
                .SumAsync(t => (decimal?)t.Valor) ?? 0;

            var estatisticas = new
            {
                totalTransacoes = totalTransacoes,
                totalReceitas = totalReceitas,
                totalDespesas = totalDespesas,
                saldoGeral = totalReceitas - totalDespesas,
                ultimaTransacao = await _context.Transacoes
                    .OrderByDescending(t => t.DataCriacao)
                    .Select(t => new { t.Id, t.Descricao, t.DataCriacao })
                    .FirstOrDefaultAsync()
            };

            return Ok(estatisticas);
        }

        /// <summary>
        /// Obtém as últimas transações registradas.
        /// </summary>
        /// <param name="limite">Quantidade máxima de transações a retornar (padrão: 10)</param>
        /// <returns>Lista das transações mais recentes</returns>
        [HttpGet("recentes")]
        public async Task<ActionResult<IEnumerable<TransacaoDTO>>> GetTransacoesRecentes([FromQuery] int limite = 10)
        {
            if (limite <= 0 || limite > 100)
            {
                limite = 10;
            }

            var transacoes = await _context.Transacoes
                .Include(t => t.Pessoa)
                .Include(t => t.Categoria)
                .OrderByDescending(t => t.DataCriacao)
                .Take(limite)
                .Select(t => new TransacaoDTO
                {
                    Id = t.Id,
                    Descricao = t.Descricao,
                    Valor = t.Valor,
                    Tipo = t.Tipo,
                    TipoDescricao = t.Tipo == TipoTransacao.Despesa ? "Despesa" : "Receita",
                    DataCriacao = t.DataCriacao,
                    CategoriaId = t.CategoriaId,
                    CategoriaNome = t.Categoria.Descricao,
                    PessoaId = t.PessoaId,
                    PessoaNome = t.Pessoa.Nome,
                    ValorComSinal = t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor
                })
                .ToListAsync();

            return Ok(transacoes);
        }

        /// <summary>
        /// Valida se uma transação pode ser criada com os parâmetros especificados.
        /// Útil para validação no frontend antes de submeter o formulário.
        /// </summary>
        /// <param name="pessoaId">ID da pessoa</param>
        /// <param name="categoriaId">ID da categoria</param>
        /// <param name="tipo">Tipo da transação (1=Despesa, 2=Receita)</param>
        /// <returns>Resultado da validação com detalhes de possíveis problemas</returns>
        [HttpGet("validar")]
        public async Task<ActionResult<object>> ValidarTransacao(
            [FromQuery] int pessoaId,
            [FromQuery] int categoriaId,
            [FromQuery] int tipo)
        {
            var validacao = new
            {
                valida = true,
                problemas = new List<string>()
            };

            // Verificar se o tipo é válido
            if (!Enum.IsDefined(typeof(TipoTransacao), tipo))
            {
                validacao.problemas.Add("Tipo deve ser: 1 = Despesa, 2 = Receita");
                return Ok(new { valida = false, problemas = validacao.problemas });
            }

            var tipoTransacao = (TipoTransacao)tipo;

            // Verificar pessoa
            var pessoa = await _context.Pessoas.FindAsync(pessoaId);
            if (pessoa == null)
            {
                validacao.problemas.Add($"Pessoa com ID {pessoaId} não foi encontrada.");
            }
            else if (pessoa.IsMenorDeIdade && tipoTransacao == TipoTransacao.Receita)
            {
                validacao.problemas.Add($"Pessoa menor de idade ({pessoa.Nome}, {pessoa.Idade} anos) só pode registrar despesas.");
            }

            // Verificar categoria
            var categoria = await _context.Categorias.FindAsync(categoriaId);
            if (categoria == null)
            {
                validacao.problemas.Add($"Categoria com ID {categoriaId} não foi encontrada.");
            }
            else if (!categoria.PermiteTipoTransacao(tipoTransacao))
            {
                var finalidadeDesc = categoria.Finalidade == TipoFinalidade.Despesa ? "despesas" :
                                   categoria.Finalidade == TipoFinalidade.Receita ? "receitas" : "ambas";
                var tipoDesc = tipoTransacao == TipoTransacao.Despesa ? "despesa" : "receita";
                
                validacao.problemas.Add($"A categoria '{categoria.Descricao}' só permite {finalidadeDesc}, mas você está tentando registrar uma {tipoDesc}.");
            }

            return Ok(new { 
                valida = !validacao.problemas.Any(),
                problemas = validacao.problemas
            });
        }
    }
}
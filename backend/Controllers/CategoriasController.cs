using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GastosResiduenciais.Api.Data;
using GastosResiduenciais.Api.Models;
using GastosResiduenciais.Api.DTOs;

namespace GastosResiduenciais.Api.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de categorias no sistema.
    /// Implementa operações de criação e listagem conforme especificação.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly GastosResiduenciaisContext _context;

        /// <summary>
        /// Construtor do controller que recebe o contexto do banco de dados via injeção de dependência.
        /// </summary>
        /// <param name="context">Contexto do Entity Framework</param>
        public CategoriasController(GastosResiduenciaisContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todas as categorias cadastradas no sistema.
        /// </summary>
        /// <returns>Lista de categorias com informações básicas e estatísticas</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategorias()
        {
            var categorias = await _context.Categorias
                .Include(c => c.Transacoes)
                .Select(c => new CategoriaDTO
                {
                    Id = c.Id,
                    Descricao = c.Descricao,
                    Finalidade = c.Finalidade,
                    FinalidadeDescricao = c.Finalidade == TipoFinalidade.Despesa ? "Despesa" :
                                        c.Finalidade == TipoFinalidade.Receita ? "Receita" : "Ambas",
                    TotalTransacoes = c.Transacoes.Count
                })
                .OrderBy(c => c.Descricao)
                .ToListAsync();

            return Ok(categorias);
        }

        /// <summary>
        /// Obtém uma categoria específica pelo ID.
        /// </summary>
        /// <param name="id">Identificador único da categoria</param>
        /// <returns>Dados completos da categoria ou NotFound se não existir</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDTO>> GetCategoria(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Transacoes)
                .Where(c => c.Id == id)
                .Select(c => new CategoriaDTO
                {
                    Id = c.Id,
                    Descricao = c.Descricao,
                    Finalidade = c.Finalidade,
                    FinalidadeDescricao = c.Finalidade == TipoFinalidade.Despesa ? "Despesa" :
                                        c.Finalidade == TipoFinalidade.Receita ? "Receita" : "Ambas",
                    TotalTransacoes = c.Transacoes.Count
                })
                .FirstOrDefaultAsync();

            if (categoria == null)
            {
                return NotFound($"Categoria com ID {id} não foi encontrada.");
            }

            return Ok(categoria);
        }

        /// <summary>
        /// Cria uma nova categoria no sistema.
        /// </summary>
        /// <param name="criarCategoriaDTO">Dados da categoria a ser criada</param>
        /// <returns>Dados da categoria criada com ID gerado</returns>
        [HttpPost]
        public async Task<ActionResult<CategoriaDTO>> CreateCategoria(CriarCategoriaDTO criarCategoriaDTO)
        {
            // Validação: verificar se já existe categoria com a mesma descrição
            var categoriaExistente = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Descricao.ToLower() == criarCategoriaDTO.Descricao.ToLower());

            if (categoriaExistente != null)
            {
                return BadRequest($"Já existe uma categoria cadastrada com a descrição '{criarCategoriaDTO.Descricao}'.");
            }

            // Validação do enum de finalidade
            if (!Enum.IsDefined(typeof(TipoFinalidade), criarCategoriaDTO.Finalidade))
            {
                return BadRequest("Finalidade deve ser: 1 = Despesa, 2 = Receita, 3 = Ambas");
            }

            // Criação da nova categoria
            var categoria = new Categoria
            {
                Descricao = criarCategoriaDTO.Descricao.Trim(),
                Finalidade = criarCategoriaDTO.Finalidade
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            // Retorno dos dados da categoria criada
            var categoriaDTO = new CategoriaDTO
            {
                Id = categoria.Id,
                Descricao = categoria.Descricao,
                Finalidade = categoria.Finalidade,
                FinalidadeDescricao = categoria.Finalidade == TipoFinalidade.Despesa ? "Despesa" :
                                    categoria.Finalidade == TipoFinalidade.Receita ? "Receita" : "Ambas",
                TotalTransacoes = 0
            };

            return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, categoriaDTO);
        }

        /// <summary>
        /// Obtém as categorias filtradas por tipo de finalidade.
        /// Útil para exibir apenas categorias compatíveis com um tipo de transação.
        /// </summary>
        /// <param name="tipo">Tipo da transação (1=Despesa, 2=Receita)</param>
        /// <returns>Lista de categorias que permitem o tipo especificado</returns>
        [HttpGet("por-tipo/{tipo}")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasPorTipo(int tipo)
        {
            if (!Enum.IsDefined(typeof(TipoTransacao), tipo))
            {
                return BadRequest("Tipo deve ser: 1 = Despesa, 2 = Receita");
            }

            var tipoTransacao = (TipoTransacao)tipo;

            var categorias = await _context.Categorias
                .Include(c => c.Transacoes)
                .Where(c => c.Finalidade == TipoFinalidade.Ambas ||
                           (c.Finalidade == TipoFinalidade.Despesa && tipoTransacao == TipoTransacao.Despesa) ||
                           (c.Finalidade == TipoFinalidade.Receita && tipoTransacao == TipoTransacao.Receita))
                .Select(c => new CategoriaDTO
                {
                    Id = c.Id,
                    Descricao = c.Descricao,
                    Finalidade = c.Finalidade,
                    FinalidadeDescricao = c.Finalidade == TipoFinalidade.Despesa ? "Despesa" :
                                        c.Finalidade == TipoFinalidade.Receita ? "Receita" : "Ambas",
                    TotalTransacoes = c.Transacoes.Count
                })
                .OrderBy(c => c.Descricao)
                .ToListAsync();

            return Ok(categorias);
        }

        /// <summary>
        /// Obtém o relatório de totais por categoria (funcionalidade opcional).
        /// Mostra receitas, despesas e saldo de cada categoria, mais o total geral.
        /// </summary>
        /// <returns>Lista de totais por categoria e totais gerais</returns>
        [HttpGet("relatorio-totais")]
        public async Task<ActionResult<object>> GetRelatorioTotaisPorCategoria()
        {
            // Busca os totais por categoria usando Group By e agregações
            var totaisPorCategoria = await _context.Categorias
                .Select(c => new TotalPorCategoriaDTO
                {
                    CategoriaId = c.Id,
                    Descricao = c.Descricao,
                    Finalidade = c.Finalidade,
                    TotalReceitas = c.Transacoes
                        .Where(t => t.Tipo == TipoTransacao.Receita)
                        .Sum(t => (decimal?)t.Valor) ?? 0,
                    TotalDespesas = c.Transacoes
                        .Where(t => t.Tipo == TipoTransacao.Despesa)
                        .Sum(t => (decimal?)t.Valor) ?? 0,
                    Saldo = c.Transacoes
                        .Where(t => t.Tipo == TipoTransacao.Receita)
                        .Sum(t => (decimal?)t.Valor) ?? 0 -
                        c.Transacoes
                        .Where(t => t.Tipo == TipoTransacao.Despesa)
                        .Sum(t => (decimal?)t.Valor) ?? 0
                })
                .OrderBy(c => c.Descricao)
                .ToListAsync();

            // Cálculo dos totais gerais
            var totalGeral = new TotalGeralDTO
            {
                TotalReceitas = totaisPorCategoria.Sum(c => c.TotalReceitas),
                TotalDespesas = totaisPorCategoria.Sum(c => c.TotalDespesas),
                SaldoLiquido = totaisPorCategoria.Sum(c => c.Saldo)
            };

            return Ok(new
            {
                totaisPorCategoria = totaisPorCategoria,
                totalGeral = totalGeral
            });
        }

        /// <summary>
        /// Verifica se uma categoria pode ser removida (não tem transações associadas).
        /// </summary>
        /// <param name="id">ID da categoria a verificar</param>
        /// <returns>Informação sobre se a categoria pode ser removida</returns>
        [HttpGet("{id}/pode-remover")]
        public async Task<ActionResult<object>> PodeRemoverCategoria(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Transacoes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
            {
                return NotFound($"Categoria com ID {id} não foi encontrada.");
            }

            var podeRemover = !categoria.Transacoes.Any();
            var totalTransacoes = categoria.Transacoes.Count;

            return Ok(new
            {
                podeRemover = podeRemover,
                totalTransacoes = totalTransacoes,
                mensagem = podeRemover 
                    ? "Categoria pode ser removida" 
                    : $"Categoria não pode ser removida pois possui {totalTransacoes} transação(ões) associada(s)"
            });
        }
    }
}
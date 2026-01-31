using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GastosResiduenciais.Api.Data;
using GastosResiduenciais.Api.Models;
using GastosResiduenciais.Api.DTOs;

namespace GastosResiduenciais.Api.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de pessoas no sistema.
    /// Implementa operações CRUD completas: Create, Read, Update, Delete.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PessoasController : ControllerBase
    {
        private readonly GastosResiduenciaisContext _context;

        /// <summary>
        /// Construtor do controller que recebe o contexto do banco de dados via injeção de dependência.
        /// </summary>
        /// <param name="context">Contexto do Entity Framework</param>
        public PessoasController(GastosResiduenciaisContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todas as pessoas cadastradas no sistema.
        /// </summary>
        /// <returns>Lista de pessoas com informações básicas e estatísticas</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PessoaDTO>>> GetPessoas()
        {
            var pessoas = await _context.Pessoas
                .Include(p => p.Transacoes)
                .Select(p => new PessoaDTO
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Idade = p.Idade,
                    IsMenorDeIdade = p.Idade < 18,
                    TotalTransacoes = p.Transacoes.Count
                })
                .ToListAsync();

            return Ok(pessoas);
        }

        /// <summary>
        /// Obtém uma pessoa específica pelo ID.
        /// </summary>
        /// <param name="id">Identificador único da pessoa</param>
        /// <returns>Dados completos da pessoa ou NotFound se não existir</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PessoaDTO>> GetPessoa(int id)
        {
            var pessoa = await _context.Pessoas
                .Include(p => p.Transacoes)
                .Where(p => p.Id == id)
                .Select(p => new PessoaDTO
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Idade = p.Idade,
                    IsMenorDeIdade = p.Idade < 18,
                    TotalTransacoes = p.Transacoes.Count
                })
                .FirstOrDefaultAsync();

            if (pessoa == null)
            {
                return NotFound($"Pessoa com ID {id} não foi encontrada.");
            }

            return Ok(pessoa);
        }

        /// <summary>
        /// Cria uma nova pessoa no sistema.
        /// </summary>
        /// <param name="criarPessoaDTO">Dados da pessoa a ser criada</param>
        /// <returns>Dados da pessoa criada com ID gerado</returns>
        [HttpPost]
        public async Task<ActionResult<PessoaDTO>> CreatePessoa(CriarPessoaDTO criarPessoaDTO)
        {
            // Validação: verificar se já existe pessoa com o mesmo nome
            var pessoaExistente = await _context.Pessoas
                .FirstOrDefaultAsync(p => p.Nome.ToLower() == criarPessoaDTO.Nome.ToLower());

            if (pessoaExistente != null)
            {
                return BadRequest($"Já existe uma pessoa cadastrada com o nome '{criarPessoaDTO.Nome}'.");
            }

            // Criação da nova pessoa
            var pessoa = new Pessoa
            {
                Nome = criarPessoaDTO.Nome.Trim(),
                Idade = criarPessoaDTO.Idade
            };

            _context.Pessoas.Add(pessoa);
            await _context.SaveChangesAsync();

            // Retorno dos dados da pessoa criada
            var pessoaDTO = new PessoaDTO
            {
                Id = pessoa.Id,
                Nome = pessoa.Nome,
                Idade = pessoa.Idade,
                IsMenorDeIdade = pessoa.IsMenorDeIdade,
                TotalTransacoes = 0
            };

            return CreatedAtAction(nameof(GetPessoa), new { id = pessoa.Id }, pessoaDTO);
        }

        /// <summary>
        /// Atualiza os dados de uma pessoa existente.
        /// </summary>
        /// <param name="id">Identificador da pessoa a ser atualizada</param>
        /// <param name="atualizarPessoaDTO">Novos dados da pessoa</param>
        /// <returns>Dados atualizados da pessoa ou erro se não encontrada</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<PessoaDTO>> UpdatePessoa(int id, AtualizarPessoaDTO atualizarPessoaDTO)
        {
            var pessoa = await _context.Pessoas
                .Include(p => p.Transacoes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pessoa == null)
            {
                return NotFound($"Pessoa com ID {id} não foi encontrada.");
            }

            // Validação: verificar se o novo nome não pertence a outra pessoa
            var pessoaComMesmoNome = await _context.Pessoas
                .FirstOrDefaultAsync(p => p.Id != id && p.Nome.ToLower() == atualizarPessoaDTO.Nome.ToLower());

            if (pessoaComMesmoNome != null)
            {
                return BadRequest($"Já existe outra pessoa cadastrada com o nome '{atualizarPessoaDTO.Nome}'.");
            }

            // Atualização dos dados
            pessoa.Nome = atualizarPessoaDTO.Nome.Trim();
            pessoa.Idade = atualizarPessoaDTO.Idade;

            await _context.SaveChangesAsync();

            // Retorno dos dados atualizados
            var pessoaDTO = new PessoaDTO
            {
                Id = pessoa.Id,
                Nome = pessoa.Nome,
                Idade = pessoa.Idade,
                IsMenorDeIdade = pessoa.IsMenorDeIdade,
                TotalTransacoes = pessoa.Transacoes.Count
            };

            return Ok(pessoaDTO);
        }

        /// <summary>
        /// Remove uma pessoa do sistema.
        /// IMPORTANTE: Remove também todas as transações associadas (cascade delete).
        /// </summary>
        /// <param name="id">Identificador da pessoa a ser removida</param>
        /// <returns>Confirmação da remoção ou erro se não encontrada</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePessoa(int id)
        {
            var pessoa = await _context.Pessoas
                .Include(p => p.Transacoes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pessoa == null)
            {
                return NotFound($"Pessoa com ID {id} não foi encontrada.");
            }

            var totalTransacoes = pessoa.Transacoes.Count;

            // Remove a pessoa (e automaticamente suas transações por cascade)
            _context.Pessoas.Remove(pessoa);
            await _context.SaveChangesAsync();

            return Ok(new { 
                message = $"Pessoa '{pessoa.Nome}' foi removida com sucesso.",
                transacoesRemovidas = totalTransacoes
            });
        }

        /// <summary>
        /// Obtém o relatório de totais por pessoa.
        /// Mostra receitas, despesas e saldo de cada pessoa, mais o total geral.
        /// </summary>
        /// <returns>Lista de totais por pessoa e totais gerais</returns>
        [HttpGet("relatorio-totais")]
        public async Task<ActionResult<object>> GetRelatorioTotaisPorPessoa()
        {
            // Busca os totais por pessoa usando Group By e agregações
            var totaisPorPessoa = await _context.Pessoas
                .Select(p => new TotalPorPessoaDTO
                {
                    PessoaId = p.Id,
                    Nome = p.Nome,
                    TotalReceitas = p.Transacoes
                        .Where(t => t.Tipo == TipoTransacao.Receita)
                        .Sum(t => (decimal?)t.Valor) ?? 0,
                    TotalDespesas = p.Transacoes
                        .Where(t => t.Tipo == TipoTransacao.Despesa)
                        .Sum(t => (decimal?)t.Valor) ?? 0,
                    Saldo = p.Transacoes
                        .Where(t => t.Tipo == TipoTransacao.Receita)
                        .Sum(t => (decimal?)t.Valor) ?? 0 -
                        p.Transacoes
                        .Where(t => t.Tipo == TipoTransacao.Despesa)
                        .Sum(t => (decimal?)t.Valor) ?? 0
                })
                .OrderBy(p => p.Nome)
                .ToListAsync();

            // Cálculo dos totais gerais
            var totalGeral = new TotalGeralDTO
            {
                TotalReceitas = totaisPorPessoa.Sum(p => p.TotalReceitas),
                TotalDespesas = totaisPorPessoa.Sum(p => p.TotalDespesas),
                SaldoLiquido = totaisPorPessoa.Sum(p => p.Saldo)
            };

            return Ok(new
            {
                totaisPorPessoa = totaisPorPessoa,
                totalGeral = totalGeral
            });
        }
    }
}
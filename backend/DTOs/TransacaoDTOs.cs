using System.ComponentModel.DataAnnotations;
using GastosResiduenciais.Api.Models;

namespace GastosResiduenciais.Api.DTOs
{
    /// <summary>
    /// DTO para criação de nova transação.
    /// Inclui validações específicas de negócio.
    /// </summary>
    public class CriarTransacaoDTO
    {
        /// <summary>
        /// Descrição da transação. Campo obrigatório com tamanho máximo de 400 caracteres.
        /// </summary>
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [MaxLength(400, ErrorMessage = "Descrição não pode exceder 400 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Valor da transação. Deve ser positivo.
        /// </summary>
        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser positivo")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Tipo da transação (Despesa ou Receita).
        /// </summary>
        [Required(ErrorMessage = "Tipo é obrigatório")]
        public TipoTransacao Tipo { get; set; }

        /// <summary>
        /// Identificador da categoria.
        /// Será validado se é compatível com o tipo de transação.
        /// </summary>
        [Required(ErrorMessage = "Categoria é obrigatória")]
        public int CategoriaId { get; set; }

        /// <summary>
        /// Identificador da pessoa.
        /// Será validado se menor de idade pode registrar receitas.
        /// </summary>
        [Required(ErrorMessage = "Pessoa é obrigatória")]
        public int PessoaId { get; set; }
    }

    /// <summary>
    /// DTO para retorno completo de dados da transação.
    /// Inclui informações das entidades relacionadas.
    /// </summary>
    public class TransacaoDTO
    {
        /// <summary>
        /// Identificador único da transação.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Descrição da transação.
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Valor da transação.
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Tipo da transação.
        /// </summary>
        public TipoTransacao Tipo { get; set; }

        /// <summary>
        /// Descrição textual do tipo para exibição.
        /// </summary>
        public string TipoDescricao { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora de criação da transação.
        /// </summary>
        public DateTime DataCriacao { get; set; }

        /// <summary>
        /// Identificador da categoria.
        /// </summary>
        public int CategoriaId { get; set; }

        /// <summary>
        /// Nome da categoria.
        /// </summary>
        public string CategoriaNome { get; set; } = string.Empty;

        /// <summary>
        /// Identificador da pessoa.
        /// </summary>
        public int PessoaId { get; set; }

        /// <summary>
        /// Nome da pessoa.
        /// </summary>
        public string PessoaNome { get; set; } = string.Empty;

        /// <summary>
        /// Valor com sinal (positivo para receitas, negativo para despesas).
        /// Útil para cálculos de totais.
        /// </summary>
        public decimal ValorComSinal { get; set; }
    }

    /// <summary>
    /// DTO para resumo dos totais gerais do sistema.
    /// Utilizado nos relatórios de consulta.
    /// </summary>
    public class TotalGeralDTO
    {
        /// <summary>
        /// Total geral de receitas de todas as pessoas/categorias.
        /// </summary>
        public decimal TotalReceitas { get; set; }

        /// <summary>
        /// Total geral de despesas de todas as pessoas/categorias.
        /// </summary>
        public decimal TotalDespesas { get; set; }

        /// <summary>
        /// Saldo líquido geral (total de receitas - total de despesas).
        /// </summary>
        public decimal SaldoLiquido { get; set; }
    }
}
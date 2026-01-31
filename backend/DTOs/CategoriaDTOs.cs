using System.ComponentModel.DataAnnotations;
using GastosResiduenciais.Api.Models;

namespace GastosResiduenciais.Api.DTOs
{
    /// <summary>
    /// DTO para criação de nova categoria.
    /// Contém os campos necessários para cadastro.
    /// </summary>
    public class CriarCategoriaDTO
    {
        /// <summary>
        /// Descrição da categoria. Campo obrigatório com tamanho máximo de 400 caracteres.
        /// </summary>
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [MaxLength(400, ErrorMessage = "Descrição não pode exceder 400 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Finalidade da categoria (Despesa, Receita ou Ambas).
        /// </summary>
        [Required(ErrorMessage = "Finalidade é obrigatória")]
        public TipoFinalidade Finalidade { get; set; }
    }

    /// <summary>
    /// DTO para retorno de dados da categoria.
    /// Inclui informações básicas e estatísticas de uso.
    /// </summary>
    public class CategoriaDTO
    {
        /// <summary>
        /// Identificador único da categoria.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Descrição da categoria.
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Finalidade da categoria.
        /// </summary>
        public TipoFinalidade Finalidade { get; set; }

        /// <summary>
        /// Descrição textual da finalidade para exibição.
        /// </summary>
        public string FinalidadeDescricao { get; set; } = string.Empty;

        /// <summary>
        /// Quantidade de transações que utilizam esta categoria.
        /// </summary>
        public int TotalTransacoes { get; set; }
    }

    /// <summary>
    /// DTO para exibir totais financeiros por categoria.
    /// Utilizado no relatório de consulta por categoria.
    /// </summary>
    public class TotalPorCategoriaDTO
    {
        /// <summary>
        /// Identificador da categoria.
        /// </summary>
        public int CategoriaId { get; set; }

        /// <summary>
        /// Descrição da categoria.
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Finalidade da categoria.
        /// </summary>
        public TipoFinalidade Finalidade { get; set; }

        /// <summary>
        /// Total de receitas da categoria.
        /// </summary>
        public decimal TotalReceitas { get; set; }

        /// <summary>
        /// Total de despesas da categoria.
        /// </summary>
        public decimal TotalDespesas { get; set; }

        /// <summary>
        /// Saldo líquido (receitas - despesas).
        /// </summary>
        public decimal Saldo { get; set; }
    }
}
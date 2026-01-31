using System.ComponentModel.DataAnnotations;

namespace GastosResiduenciais.Api.DTOs
{
    /// <summary>
    /// DTO para criação de nova pessoa.
    /// Contém apenas os campos necessários para cadastro.
    /// </summary>
    public class CriarPessoaDTO
    {
        /// <summary>
        /// Nome da pessoa. Campo obrigatório com tamanho máximo de 200 caracteres.
        /// </summary>
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(200, ErrorMessage = "Nome não pode exceder 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Idade da pessoa. Campo obrigatório.
        /// </summary>
        [Required(ErrorMessage = "Idade é obrigatória")]
        [Range(0, 150, ErrorMessage = "Idade deve estar entre 0 e 150 anos")]
        public int Idade { get; set; }
    }

    /// <summary>
    /// DTO para atualização de dados de pessoa existente.
    /// Permite modificar nome e idade.
    /// </summary>
    public class AtualizarPessoaDTO
    {
        /// <summary>
        /// Novo nome da pessoa.
        /// </summary>
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(200, ErrorMessage = "Nome não pode exceder 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Nova idade da pessoa.
        /// </summary>
        [Required(ErrorMessage = "Idade é obrigatória")]
        [Range(0, 150, ErrorMessage = "Idade deve estar entre 0 e 150 anos")]
        public int Idade { get; set; }
    }

    /// <summary>
    /// DTO para retorno de dados completos da pessoa.
    /// Inclui informações calculadas e relacionamentos.
    /// </summary>
    public class PessoaDTO
    {
        /// <summary>
        /// Identificador único da pessoa.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome da pessoa.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Idade da pessoa.
        /// </summary>
        public int Idade { get; set; }

        /// <summary>
        /// Indica se a pessoa é menor de idade (< 18 anos).
        /// Importante para validações de transações.
        /// </summary>
        public bool IsMenorDeIdade { get; set; }

        /// <summary>
        /// Quantidade total de transações da pessoa.
        /// </summary>
        public int TotalTransacoes { get; set; }
    }

    /// <summary>
    /// DTO para exibir totais financeiros por pessoa.
    /// Utilizado no relatório de consulta por pessoa.
    /// </summary>
    public class TotalPorPessoaDTO
    {
        /// <summary>
        /// Identificador da pessoa.
        /// </summary>
        public int PessoaId { get; set; }

        /// <summary>
        /// Nome da pessoa.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Total de receitas da pessoa.
        /// </summary>
        public decimal TotalReceitas { get; set; }

        /// <summary>
        /// Total de despesas da pessoa.
        /// </summary>
        public decimal TotalDespesas { get; set; }

        /// <summary>
        /// Saldo líquido (receitas - despesas).
        /// </summary>
        public decimal Saldo { get; set; }
    }
}
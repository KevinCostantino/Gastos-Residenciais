using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GastosResiduenciais.Api.Models
{
    /// <summary>
    /// Enumeração que define os tipos de transação disponíveis no sistema.
    /// </summary>
    public enum TipoTransacao
    {
        /// <summary>
        /// Transação de despesa (saída de dinheiro)
        /// </summary>
        Despesa = 1,

        /// <summary>
        /// Transação de receita (entrada de dinheiro)
        /// </summary>
        Receita = 2
    }

    /// <summary>
    /// Entidade que representa uma transação financeira no sistema.
    /// Pode ser uma despesa ou receita associada a uma pessoa e categoria.
    /// </summary>
    public class Transacao
    {
        /// <summary>
        /// Identificador único da transação, gerado automaticamente pelo sistema.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Descrição detalhada da transação. Campo obrigatório com tamanho máximo de 400 caracteres.
        /// Ex: "Compra no supermercado", "Pagamento do salário", etc.
        /// </summary>
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [MaxLength(400, ErrorMessage = "Descrição não pode exceder 400 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Valor da transação. Deve ser sempre positivo.
        /// Para despesas, representa o valor gasto; para receitas, o valor recebido.
        /// </summary>
        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser positivo")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Tipo da transação (Despesa ou Receita).
        /// Importante: Menores de idade só podem registrar despesas.
        /// </summary>
        [Required(ErrorMessage = "Tipo é obrigatório")]
        public TipoTransacao Tipo { get; set; }

        /// <summary>
        /// Data e hora em que a transação foi registrada no sistema.
        /// Preenchida automaticamente no momento da criação.
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        /// <summary>
        /// Identificador da categoria associada à transação.
        /// </summary>
        [Required(ErrorMessage = "Categoria é obrigatória")]
        public int CategoriaId { get; set; }

        /// <summary>
        /// Categoria associada à transação.
        /// Relacionamento N:1 - Várias transações podem usar a mesma categoria.
        /// </summary>
        [ForeignKey("CategoriaId")]
        public virtual Categoria Categoria { get; set; } = null!;

        /// <summary>
        /// Identificador da pessoa associada à transação.
        /// </summary>
        [Required(ErrorMessage = "Pessoa é obrigatória")]
        public int PessoaId { get; set; }

        /// <summary>
        /// Pessoa associada à transação.
        /// Relacionamento N:1 - Várias transações podem pertencer à mesma pessoa.
        /// </summary>
        [ForeignKey("PessoaId")]
        public virtual Pessoa Pessoa { get; set; } = null!;

        /// <summary>
        /// Calcula o valor considerando o sinal da transação.
        /// Retorna valor positivo para receitas e negativo para despesas.
        /// Útil para cálculos de saldo e totais.
        /// </summary>
        public decimal ValorComSinal => Tipo == TipoTransacao.Receita ? Valor : -Valor;
    }
}
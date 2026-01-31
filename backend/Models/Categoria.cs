using System.ComponentModel.DataAnnotations;

namespace GastosResiduenciais.Api.Models
{
    /// <summary>
    /// Enumeração que define os tipos de finalidade que uma categoria pode ter.
    /// Controla quais tipos de transação podem usar cada categoria.
    /// </summary>
    public enum TipoFinalidade
    {
        /// <summary>
        /// Categoria apenas para despesas (saídas de dinheiro)
        /// </summary>
        Despesa = 1,

        /// <summary>
        /// Categoria apenas para receitas (entradas de dinheiro)
        /// </summary>
        Receita = 2,

        /// <summary>
        /// Categoria pode ser usada tanto para despesas quanto receitas
        /// </summary>
        Ambas = 3
    }

    /// <summary>
    /// Entidade que representa uma categoria de transação no sistema.
    /// Utilizada para classificar e organizar as transações por tipo.
    /// </summary>
    public class Categoria
    {
        /// <summary>
        /// Identificador único da categoria, gerado automaticamente pelo sistema.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Descrição da categoria. Campo obrigatório com tamanho máximo de 400 caracteres.
        /// Ex: "Alimentação", "Salário", "Transporte", etc.
        /// </summary>
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [MaxLength(400, ErrorMessage = "Descrição não pode exceder 400 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Define se a categoria pode ser usada para despesas, receitas ou ambas.
        /// Controla a validação na criação de transações.
        /// </summary>
        [Required(ErrorMessage = "Finalidade é obrigatória")]
        public TipoFinalidade Finalidade { get; set; }

        /// <summary>
        /// Coleção das transações que utilizam esta categoria.
        /// Relacionamento 1:N - Uma categoria pode ser usada em várias transações.
        /// </summary>
        public virtual ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();

        /// <summary>
        /// Verifica se a categoria permite transações do tipo especificado.
        /// </summary>
        /// <param name="tipoTransacao">Tipo da transação a ser validado</param>
        /// <returns>True se a categoria permite o tipo de transação</returns>
        public bool PermiteTipoTransacao(TipoTransacao tipoTransacao)
        {
            return Finalidade == TipoFinalidade.Ambas ||
                   (Finalidade == TipoFinalidade.Despesa && tipoTransacao == TipoTransacao.Despesa) ||
                   (Finalidade == TipoFinalidade.Receita && tipoTransacao == TipoTransacao.Receita);
        }
    }
}
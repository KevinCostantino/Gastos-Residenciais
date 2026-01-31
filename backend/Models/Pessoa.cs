using System.ComponentModel.DataAnnotations;

namespace GastosResiduenciais.Api.Models
{
    /// <summary>
    /// Entidade que representa uma pessoa no sistema de controle de gastos.
    /// Contém as informações básicas de identificação e controle das transações associadas.
    /// </summary>
    public class Pessoa
    {
        /// <summary>
        /// Identificador único da pessoa, gerado automaticamente pelo sistema.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome da pessoa. Campo obrigatório com tamanho máximo de 200 caracteres.
        /// </summary>
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(200, ErrorMessage = "Nome não pode exceder 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Idade da pessoa. Campo obrigatório.
        /// Importante: Menores de 18 anos só podem registrar despesas.
        /// </summary>
        [Required(ErrorMessage = "Idade é obrigatória")]
        [Range(0, 150, ErrorMessage = "Idade deve estar entre 0 e 150 anos")]
        public int Idade { get; set; }

        /// <summary>
        /// Coleção das transações associadas a esta pessoa.
        /// Relacionamento 1:N - Uma pessoa pode ter várias transações.
        /// </summary>
        public virtual ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();

        /// <summary>
        /// Propriedade calculada que indica se a pessoa é menor de idade.
        /// Utilizada para validação de regras de negócio (menores só podem ter despesas).
        /// </summary>
        public bool IsMenorDeIdade => Idade < 18;
    }
}
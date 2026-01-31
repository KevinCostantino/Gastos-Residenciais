using Microsoft.EntityFrameworkCore;
using GastosResiduenciais.Api.Models;

namespace GastosResiduenciais.Api.Data
{
    /// <summary>
    /// Contexto do Entity Framework para o sistema de gastos residenciais.
    /// Responsável pela configuração das entidades e relacionamentos do banco de dados.
    /// </summary>
    public class GastosResiduenciaisContext : DbContext
    {
        /// <summary>
        /// Construtor que recebe as opções de configuração do contexto.
        /// </summary>
        /// <param name="options">Opções de configuração do Entity Framework</param>
        public GastosResiduenciaisContext(DbContextOptions<GastosResiduenciaisContext> options) : base(options)
        {
        }

        /// <summary>
        /// DbSet para gerenciamento das entidades Pessoa.
        /// Representa a tabela de pessoas no banco de dados.
        /// </summary>
        public DbSet<Pessoa> Pessoas { get; set; }

        /// <summary>
        /// DbSet para gerenciamento das entidades Categoria.
        /// Representa a tabela de categorias no banco de dados.
        /// </summary>
        public DbSet<Categoria> Categorias { get; set; }

        /// <summary>
        /// DbSet para gerenciamento das entidades Transacao.
        /// Representa a tabela de transações no banco de dados.
        /// </summary>
        public DbSet<Transacao> Transacoes { get; set; }

        /// <summary>
        /// Configuração do modelo do banco de dados.
        /// Define relacionamentos, índices, restrições e outras configurações específicas.
        /// </summary>
        /// <param name="modelBuilder">Construtor do modelo Entity Framework</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da entidade Pessoa
            modelBuilder.Entity<Pessoa>(entity =>
            {
                // Configuração da chave primária
                entity.HasKey(p => p.Id);
                
                // Configuração do campo Nome
                entity.Property(p => p.Nome)
                    .IsRequired()
                    .HasMaxLength(200);
                
                // Configuração do campo Idade
                entity.Property(p => p.Idade)
                    .IsRequired();

                // Índice único para evitar nomes duplicados
                entity.HasIndex(p => p.Nome)
                    .IsUnique();

                // Configuração do relacionamento com Transacoes
                entity.HasMany(p => p.Transacoes)
                    .WithOne(t => t.Pessoa)
                    .HasForeignKey(t => t.PessoaId)
                    .OnDelete(DeleteBehavior.Cascade); // Delete em cascata conforme requisito
            });

            // Configuração da entidade Categoria
            modelBuilder.Entity<Categoria>(entity =>
            {
                // Configuração da chave primária
                entity.HasKey(c => c.Id);
                
                // Configuração do campo Descricao
                entity.Property(c => c.Descricao)
                    .IsRequired()
                    .HasMaxLength(400);
                
                // Configuração do enum Finalidade
                entity.Property(c => c.Finalidade)
                    .IsRequired()
                    .HasConversion<int>(); // Salva como int no banco

                // Configuração do relacionamento com Transacoes
                entity.HasMany(c => c.Transacoes)
                    .WithOne(t => t.Categoria)
                    .HasForeignKey(t => t.CategoriaId)
                    .OnDelete(DeleteBehavior.Restrict); // Não permite deletar categoria em uso
            });

            // Configuração da entidade Transacao
            modelBuilder.Entity<Transacao>(entity =>
            {
                // Configuração da chave primária
                entity.HasKey(t => t.Id);
                
                // Configuração do campo Descricao
                entity.Property(t => t.Descricao)
                    .IsRequired()
                    .HasMaxLength(400);
                
                // Configuração do campo Valor
                entity.Property(t => t.Valor)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)") // Precisão para valores monetários
                    .HasPrecision(18, 2);
                
                // Configuração do enum Tipo
                entity.Property(t => t.Tipo)
                    .IsRequired()
                    .HasConversion<int>(); // Salva como int no banco
                
                // Configuração do campo DataCriacao
                entity.Property(t => t.DataCriacao)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP"); // Valor padrão no banco

                // Configuração dos relacionamentos
                entity.HasOne(t => t.Pessoa)
                    .WithMany(p => p.Transacoes)
                    .HasForeignKey(t => t.PessoaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(t => t.Categoria)
                    .WithMany(c => c.Transacoes)
                    .HasForeignKey(t => t.CategoriaId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices para melhorar performance das consultas
                entity.HasIndex(t => t.PessoaId);
                entity.HasIndex(t => t.CategoriaId);
                entity.HasIndex(t => t.DataCriacao);
                entity.HasIndex(t => t.Tipo);
            });

            // Seed de dados iniciais (categorias padrão)
            SeedData(modelBuilder);
        }

        /// <summary>
        /// Método para inserir dados iniciais no banco (seed data).
        /// Cria categorias padrão para facilitar o uso do sistema.
        /// </summary>
        /// <param name="modelBuilder">Construtor do modelo Entity Framework</param>
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Categorias padrão para facilitar o uso do sistema
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Descricao = "Alimentação", Finalidade = TipoFinalidade.Despesa },
                new Categoria { Id = 2, Descricao = "Transporte", Finalidade = TipoFinalidade.Despesa },
                new Categoria { Id = 3, Descricao = "Moradia", Finalidade = TipoFinalidade.Despesa },
                new Categoria { Id = 4, Descricao = "Saúde", Finalidade = TipoFinalidade.Despesa },
                new Categoria { Id = 5, Descricao = "Educação", Finalidade = TipoFinalidade.Despesa },
                new Categoria { Id = 6, Descricao = "Lazer", Finalidade = TipoFinalidade.Despesa },
                new Categoria { Id = 7, Descricao = "Salário", Finalidade = TipoFinalidade.Receita },
                new Categoria { Id = 8, Descricao = "Freelance", Finalidade = TipoFinalidade.Receita },
                new Categoria { Id = 9, Descricao = "Investimentos", Finalidade = TipoFinalidade.Receita },
                new Categoria { Id = 10, Descricao = "Outros", Finalidade = TipoFinalidade.Ambas }
            );
        }
    }
}
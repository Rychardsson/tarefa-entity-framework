using Microsoft.EntityFrameworkCore;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Context
{
    public class OrganizadorContext : DbContext
    {
        public OrganizadorContext(DbContextOptions<OrganizadorContext> options) : base(options)
        {
            
        }

        public DbSet<Tarefa> Tarefas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tarefa>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(e => e.Descricao)
                    .HasMaxLength(1000);
                
                entity.Property(e => e.Data)
                    .IsRequired();
                
                entity.Property(e => e.Status)
                    .IsRequired();
                
                entity.Property(e => e.DataCriacao)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                entity.Property(e => e.IsDeleted)
                    .HasDefaultValue(false);

                // Índices para melhorar performance das consultas
                entity.HasIndex(e => e.Data);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.IsDeleted);
                entity.HasIndex(e => new { e.IsDeleted, e.DataCriacao });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
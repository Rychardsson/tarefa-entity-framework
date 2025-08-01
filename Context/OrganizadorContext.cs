using Microsoft.EntityFrameworkCore;
using TrilhaApiDesafio.Models;
using TrilhaApiDesafio.Authentication;

namespace TrilhaApiDesafio.Context
{
    public class OrganizadorContext : DbContext
    {
        public OrganizadorContext(DbContextOptions<OrganizadorContext> options) : base(options)
        {
            
        }

        public DbSet<Tarefa> Tarefas { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuração da entidade Tarefa
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
                
                entity.Property(e => e.Prioridade)
                    .HasDefaultValue(1);
                
                entity.Property(e => e.Tags)
                    .HasMaxLength(500);

                // Índices para melhorar performance das consultas
                entity.HasIndex(e => e.Data);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Prioridade);
                entity.HasIndex(e => e.IsDeleted);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => new { e.IsDeleted, e.DataCriacao });
                entity.HasIndex(e => new { e.Status, e.Prioridade });
                entity.HasIndex(e => new { e.Data, e.Status });
            });

            // Configuração da entidade User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                // Índices únicos
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configuração da entidade Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);

                // Índice único
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Configuração da entidade UserRole (many-to-many)
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.AssignedAt).IsRequired();
            });

            // Seed inicial de roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "Administrador do sistema" },
                new Role { Id = 2, Name = "User", Description = "Usuário padrão do sistema" }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
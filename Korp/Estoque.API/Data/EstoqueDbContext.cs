using Estoque.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Estoque.API.Data {
    public sealed class EstoqueDbContext : DbContext {
        public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options) { }

        public DbSet<Produto> Produtos => Set<Produto>();

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Produto>(entity => {
                entity.HasKey(produto => produto.Id);
                entity.HasIndex(produto => produto.Codigo).IsUnique();
                entity.Property(produto => produto.Codigo).HasMaxLength(30).IsRequired();
                entity.Property(produto => produto.Descricao).HasMaxLength(120).IsRequired();
                entity.Property(produto => produto.Saldo).IsRequired();
                entity.Property(produto => produto.CriadoEm).IsRequired();
                entity.Property(produto => produto.ModificadoEm).IsRequired();
            });
        }
    }
}

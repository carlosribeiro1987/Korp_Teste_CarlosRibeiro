using Faturamento.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Faturamento.API.Data {
    public sealed class FaturamentoDbContext : DbContext {
        public FaturamentoDbContext(DbContextOptions<FaturamentoDbContext> options)
        : base(options) {
        }

        public DbSet<NotaFiscal> NotasFiscais => Set<NotaFiscal>();
        public DbSet<ItemNotaFiscal> ItensNotaFiscal => Set<ItemNotaFiscal>();

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<NotaFiscal>(entity =>
            {
                entity.HasKey(nota => nota.NotaFiscalId);
                entity.Property(nota => nota.NotaFiscalId).ValueGeneratedNever();
                entity.HasIndex(nota => nota.Numero).IsUnique();
                entity.Property(nota => nota.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
                entity.Property(nota => nota.CriadaEm).IsRequired();
                entity.HasMany(nota => nota.Itens)
                    .WithOne()
                    .HasForeignKey(item => item.NotaFiscalId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ItemNotaFiscal>(entity =>
            {
                entity.HasKey(item => item.ItemId);
                entity.Property(item => item.ItemId).ValueGeneratedNever();
                entity.Property(item => item.CodigoProduto).HasMaxLength(30).IsRequired();
                entity.Property(item => item.DescricaoProduto).HasMaxLength(120).IsRequired();
                entity.Property(item => item.Quantidade).IsRequired();
            });
        }
    }
}

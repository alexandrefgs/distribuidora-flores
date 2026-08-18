using Microsoft.EntityFrameworkCore;
using DistribuidoraFlores.Api.Modules.Catalogo.Domain;

namespace DistribuidoraFlores.Api.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Lote> Lotes => Set<Lote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produto>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).ValueGeneratedNever();
            entity.Property(p => p.Nome).IsRequired().HasMaxLength(200);
            entity.Property(p => p.PrecoUnitario).HasColumnType("decimal(10,2)");

            entity.HasMany(p => p.Lotes)
                  .WithOne()
                  .HasForeignKey(l => l.ProdutoId);
        });

        modelBuilder.Entity<Lote>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Id).ValueGeneratedNever();
        });

        base.OnModelCreating(modelBuilder);
    }
}
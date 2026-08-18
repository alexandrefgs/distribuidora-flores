using Microsoft.EntityFrameworkCore;
using DistribuidoraFlores.Api.Modules.Catalogo.Domain;
using DistribuidoraFlores.Api.Modules.Clientes.Domain;

namespace DistribuidoraFlores.Api.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Lote> Lotes => Set<Lote>();
    public DbSet<Cliente> Clientes => Set<Cliente>();

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

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).ValueGeneratedNever();
            entity.Property(c => c.NomeFantasia).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Email).IsRequired().HasMaxLength(200);

            entity.OwnsOne(c => c.Documento, documento =>
            {
                documento.Property(d => d.Numero)
                    .HasColumnName("Documento")
                    .IsRequired()
                    .HasMaxLength(14);

                documento.Property(d => d.Tipo)
                    .HasColumnName("TipoDocumento")
                    .HasConversion<string>()
                    .IsRequired();

                documento.HasIndex(d => d.Numero).IsUnique();
            });
        });

        base.OnModelCreating(modelBuilder);
    }
}
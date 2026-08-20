using Microsoft.EntityFrameworkCore;
using DistribuidoraFlores.Api.Modules.Catalogo.Domain;
using DistribuidoraFlores.Api.Modules.Clientes.Domain;
using DistribuidoraFlores.Api.Modules.Pedidos.Domain;
using DistribuidoraFlores.Api.Modules.Frota.Domain;
using DistribuidoraFlores.Api.Modules.Identidade.Domain;

namespace DistribuidoraFlores.Api.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Lote> Lotes => Set<Lote>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<Veiculo> Veiculos => Set<Veiculo>();
    public DbSet<Motorista> Motoristas => Set<Motorista>();
    public DbSet<Entrega> Entregas => Set<Entrega>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

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

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).ValueGeneratedNever();
            entity.Property(p => p.Status).HasConversion<string>();
            entity.Ignore(p => p.Total);

            entity.HasMany(p => p.Itens)
                  .WithOne()
                  .HasForeignKey(i => i.PedidoId);
        });

        modelBuilder.Entity<ItemPedido>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Id).ValueGeneratedNever();
            entity.Property(i => i.NomeProduto).IsRequired().HasMaxLength(200);
            entity.Property(i => i.PrecoUnitario).HasColumnType("decimal(10,2)");
        });

        modelBuilder.Entity<Veiculo>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Id).ValueGeneratedNever();
            entity.Property(v => v.Placa).IsRequired().HasMaxLength(10);
            entity.Property(v => v.Modelo).IsRequired().HasMaxLength(100);

            entity.HasIndex(v => v.Placa).IsUnique();
        });

        modelBuilder.Entity<Motorista>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).ValueGeneratedNever();
            entity.Property(m => m.Nome).IsRequired().HasMaxLength(200);
            entity.Property(m => m.Cnh).IsRequired().HasMaxLength(20);

            entity.HasIndex(m => m.Cnh).IsUnique();
        });

        modelBuilder.Entity<Entrega>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Status).HasConversion<string>();
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).ValueGeneratedNever();
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
            entity.Property(u => u.SenhaHash).IsRequired();
            entity.Property(u => u.Role).HasConversion<string>();

            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.Property(rt => rt.Id).ValueGeneratedNever();
            entity.Property(rt => rt.TokenHash).IsRequired();

            entity.HasIndex(rt => rt.TokenHash).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
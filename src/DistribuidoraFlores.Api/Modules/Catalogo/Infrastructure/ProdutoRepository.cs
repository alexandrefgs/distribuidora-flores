using Microsoft.EntityFrameworkCore;
using DistribuidoraFlores.Api.Infrastructure.Persistence;
using DistribuidoraFlores.Api.Modules.Catalogo.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Catalogo.Domain;

namespace DistribuidoraFlores.Api.Modules.Catalogo.Infrastructure;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Produto?> ObterPorIdAsync(Guid id)
    {
        return await _context.Produtos
            .Include(p => p.Lotes)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Produto>> ListarAtivosAsync()
    {
        return await _context.Produtos
            .Include(p => p.Lotes)
            .Where(p => p.Ativo)
            .ToListAsync();
    }

    public async Task AdicionarAsync(Produto produto)
    {
        await _context.Produtos.AddAsync(produto);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
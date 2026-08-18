using Microsoft.EntityFrameworkCore;
using DistribuidoraFlores.Api.Infrastructure.Persistence;
using DistribuidoraFlores.Api.Modules.Pedidos.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Pedidos.Domain;

namespace DistribuidoraFlores.Api.Modules.Pedidos.Infrastructure;

public class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _context;

    public PedidoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Pedido?> ObterPorIdAsync(Guid id)
    {
        return await _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Pedido>> ListarPorClienteAsync(Guid clienteId)
    {
        return await _context.Pedidos
            .Include(p => p.Itens)
            .Where(p => p.ClienteId == clienteId)
            .ToListAsync();
    }

    public async Task<List<Pedido>> ListarTodosAsync()
    {
        return await _context.Pedidos
            .Include(p => p.Itens)
            .ToListAsync();
    }

    public async Task AdicionarAsync(Pedido pedido)
    {
        await _context.Pedidos.AddAsync(pedido);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
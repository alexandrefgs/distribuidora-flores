using Microsoft.EntityFrameworkCore;
using DistribuidoraFlores.Api.Infrastructure.Persistence;
using DistribuidoraFlores.Api.Modules.Frota.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Frota.Domain;

namespace DistribuidoraFlores.Api.Modules.Frota.Infrastructure;

public class EntregaRepository : IEntregaRepository
{
    private readonly AppDbContext _context;

    public EntregaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Entrega?> ObterPorIdAsync(Guid id)
    {
        return await _context.Entregas.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<Entrega>> ListarTodasAsync()
    {
        return await _context.Entregas.ToListAsync();
    }

    public async Task AdicionarAsync(Entrega entrega)
    {
        await _context.Entregas.AddAsync(entrega);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
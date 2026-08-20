using Microsoft.EntityFrameworkCore;
using DistribuidoraFlores.Api.Infrastructure.Persistence;
using DistribuidoraFlores.Api.Modules.Frota.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Frota.Domain;

namespace DistribuidoraFlores.Api.Modules.Frota.Infrastructure;

public class MotoristaRepository : IMotoristaRepository
{
    private readonly AppDbContext _context;

    public MotoristaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Motorista?> ObterPorIdAsync(Guid id)
    {
        return await _context.Motoristas.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Motorista>> ListarAtivosAsync()
    {
        return await _context.Motoristas.Where(m => m.Ativo).ToListAsync();
    }

    public async Task AdicionarAsync(Motorista motorista)
    {
        await _context.Motoristas.AddAsync(motorista);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
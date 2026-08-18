using Microsoft.EntityFrameworkCore;
using DistribuidoraFlores.Api.Infrastructure.Persistence;
using DistribuidoraFlores.Api.Modules.Clientes.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Clientes.Domain;

namespace DistribuidoraFlores.Api.Modules.Clientes.Infrastructure;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> ObterPorIdAsync(Guid id)
    {
        return await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Cliente?> ObterPorDocumentoAsync(string documento)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Documento.Numero == documento);
    }

    public async Task<List<Cliente>> ListarAtivosAsync()
    {
        return await _context.Clientes
            .Where(c => c.Ativo)
            .ToListAsync();
    }

    public async Task AdicionarAsync(Cliente cliente)
    {
        await _context.Clientes.AddAsync(cliente);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
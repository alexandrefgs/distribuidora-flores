using DistribuidoraFlores.Api.Modules.Clientes.Application.Interfaces;

namespace DistribuidoraFlores.Api.Modules.Clientes.Application.UseCases;

public class AtivarClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;

    public AtivarClienteUseCase(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task ExecutarAsync(Guid clienteId)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(clienteId);

        if (cliente is null)
            throw new InvalidOperationException("Cliente não encontrado.");

        cliente.Ativar();

        await _clienteRepository.SalvarAlteracoesAsync();
    }
}
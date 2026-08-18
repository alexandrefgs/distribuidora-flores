using DistribuidoraFlores.Api.Modules.Clientes.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Clientes.Domain;

namespace DistribuidoraFlores.Api.Modules.Clientes.Application.UseCases;

public class CadastrarClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;

    public CadastrarClienteUseCase(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<Guid> ExecutarAsync(string nomeFantasia, string documento, string telefone, string email, string endereco)
    {
        var documentoLimpo = new string(documento.Where(char.IsDigit).ToArray());
        var clienteExistente = await _clienteRepository.ObterPorDocumentoAsync(documentoLimpo);

        if (clienteExistente is not null)
            throw new InvalidOperationException("Já existe um cliente cadastrado com esse documento.");

        var cliente = new Cliente(nomeFantasia, documento, telefone, email, endereco);

        await _clienteRepository.AdicionarAsync(cliente);
        await _clienteRepository.SalvarAlteracoesAsync();

        return cliente.Id;
    }
}
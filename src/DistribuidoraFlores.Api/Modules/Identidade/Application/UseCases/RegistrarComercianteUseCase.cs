using DistribuidoraFlores.Api.Modules.Identidade.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Identidade.Domain;
using DistribuidoraFlores.Api.Modules.Clientes.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Clientes.Domain;

namespace DistribuidoraFlores.Api.Modules.Identidade.Application.UseCases;

public class RegistrarComercianteUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegistrarComercianteUseCase(
        IUsuarioRepository usuarioRepository,
        IClienteRepository clienteRepository,
        IPasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _clienteRepository = clienteRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> ExecutarAsync(
        string nomeFantasia, string documento, string telefone, string endereco,
        string email, string senha)
    {
        var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(email);
        if (usuarioExistente is not null)
            throw new InvalidOperationException("Já existe um usuário cadastrado com esse email.");

        var documentoLimpo = new string(documento.Where(char.IsDigit).ToArray());
        var clienteExistente = await _clienteRepository.ObterPorDocumentoAsync(documentoLimpo);
        if (clienteExistente is not null)
            throw new InvalidOperationException("Já existe um cliente cadastrado com esse documento.");

        var cliente = new Cliente(nomeFantasia, documento, telefone, email, endereco);
        await _clienteRepository.AdicionarAsync(cliente);
        await _clienteRepository.SalvarAlteracoesAsync();

        var senhaHash = _passwordHasher.GerarHash(senha);
        var usuario = Usuario.CriarComerciante(email, senhaHash, cliente.Id);
        await _usuarioRepository.AdicionarAsync(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync();

        return usuario.Id;
    }
}
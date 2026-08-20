using DistribuidoraFlores.Api.Modules.Identidade.Application.Interfaces;

namespace DistribuidoraFlores.Api.Modules.Identidade.Application.UseCases;

public class LoginUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginUseCase(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<string> ExecutarAsync(string email, string senha)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(email);

        if (usuario is null || !usuario.Ativo || !_passwordHasher.Verificar(senha, usuario.SenhaHash))
            throw new InvalidOperationException("Email ou senha inválidos.");

        return _tokenService.GerarToken(usuario);
    }
}
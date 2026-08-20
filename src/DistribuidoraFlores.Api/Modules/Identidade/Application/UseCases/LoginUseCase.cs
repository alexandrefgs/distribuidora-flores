using DistribuidoraFlores.Api.Modules.Identidade.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Identidade.Domain;

namespace DistribuidoraFlores.Api.Modules.Identidade.Application.UseCases;

public record LoginResult(string AccessToken, string RefreshToken);

public class LoginUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginUseCase(
        IUsuarioRepository usuarioRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _usuarioRepository = usuarioRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResult> ExecutarAsync(string email, string senha)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(email);

        if (usuario is null || !usuario.Ativo || !_passwordHasher.Verificar(senha, usuario.SenhaHash))
            throw new InvalidOperationException("Email ou senha inválidos.");

        var accessToken = _tokenService.GerarAccessToken(usuario);
        var refreshTokenBruto = _tokenService.GerarRefreshTokenBruto();
        var refreshTokenHash = _tokenService.HashRefreshToken(refreshTokenBruto);

        var refreshToken = new RefreshToken(usuario.Id, refreshTokenHash, DateTime.UtcNow.AddDays(7));
        await _refreshTokenRepository.AdicionarAsync(refreshToken);
        await _refreshTokenRepository.SalvarAlteracoesAsync();

        return new LoginResult(accessToken, refreshTokenBruto);
    }
}
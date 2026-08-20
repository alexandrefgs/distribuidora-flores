using DistribuidoraFlores.Api.Modules.Identidade.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Identidade.Domain;

namespace DistribuidoraFlores.Api.Modules.Identidade.Application.UseCases;

public class RefreshTokenUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;

    public RefreshTokenUseCase(
        IUsuarioRepository usuarioRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService)
    {
        _usuarioRepository = usuarioRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
    }

    public async Task<LoginResult> ExecutarAsync(string refreshTokenBruto)
    {
        var hash = _tokenService.HashRefreshToken(refreshTokenBruto);
        var refreshTokenAtual = await _refreshTokenRepository.ObterPorTokenHashAsync(hash);

        if (refreshTokenAtual is null || !refreshTokenAtual.EstaValido())
            throw new InvalidOperationException("Refresh token inválido ou expirado.");

        var usuario = await _usuarioRepository.ObterPorIdAsync(refreshTokenAtual.UsuarioId);
        if (usuario is null || !usuario.Ativo)
            throw new InvalidOperationException("Usuário não encontrado ou inativo.");

        // Rotação: revoga o token usado e gera um novo
        refreshTokenAtual.Revogar();

        var novoAccessToken = _tokenService.GerarAccessToken(usuario);
        var novoRefreshTokenBruto = _tokenService.GerarRefreshTokenBruto();
        var novoRefreshTokenHash = _tokenService.HashRefreshToken(novoRefreshTokenBruto);

        var novoRefreshToken = new RefreshToken(usuario.Id, novoRefreshTokenHash, DateTime.UtcNow.AddDays(7));
        await _refreshTokenRepository.AdicionarAsync(novoRefreshToken);
        await _refreshTokenRepository.SalvarAlteracoesAsync();

        return new LoginResult(novoAccessToken, novoRefreshTokenBruto);
    }
}
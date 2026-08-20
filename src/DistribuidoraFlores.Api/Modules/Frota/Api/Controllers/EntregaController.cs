using Microsoft.AspNetCore.Mvc;
using DistribuidoraFlores.Api.Modules.Frota.Api.DTOs;
using DistribuidoraFlores.Api.Modules.Frota.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Frota.Application.UseCases;

namespace DistribuidoraFlores.Api.Modules.Frota.Api.Controllers;

[ApiController]
[Route("api/entregas")]
public class EntregaController : ControllerBase
{
    private readonly CriarEntregaUseCase _criarEntregaUseCase;
    private readonly IniciarRotaUseCase _iniciarRotaUseCase;
    private readonly ConcluirEntregaUseCase _concluirEntregaUseCase;
    private readonly IEntregaRepository _entregaRepository;

    public EntregaController(
        CriarEntregaUseCase criarEntregaUseCase,
        IniciarRotaUseCase iniciarRotaUseCase,
        ConcluirEntregaUseCase concluirEntregaUseCase,
        IEntregaRepository entregaRepository)
    {
        _criarEntregaUseCase = criarEntregaUseCase;
        _iniciarRotaUseCase = iniciarRotaUseCase;
        _concluirEntregaUseCase = concluirEntregaUseCase;
        _entregaRepository = entregaRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarEntregaRequest request)
    {
        try
        {
            var id = await _criarEntregaUseCase.ExecutarAsync(request.PedidoId, request.VeiculoId, request.MotoristaId);
            return CreatedAtAction(nameof(ObterPorId), new { id }, new { id });
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(new { erro = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var entrega = await _entregaRepository.ObterPorIdAsync(id);
        return entrega is null ? NotFound() : Ok(EntregaResponse.FromDomain(entrega));
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var entregas = await _entregaRepository.ListarTodasAsync();
        return Ok(entregas.Select(EntregaResponse.FromDomain));
    }

    [HttpPatch("{id:guid}/iniciar-rota")]
    public async Task<IActionResult> IniciarRota(Guid id) => await ExecutarTransicao(() => _iniciarRotaUseCase.ExecutarAsync(id));

    [HttpPatch("{id:guid}/concluir")]
    public async Task<IActionResult> Concluir(Guid id) => await ExecutarTransicao(() => _concluirEntregaUseCase.ExecutarAsync(id));

    private async Task<IActionResult> ExecutarTransicao(Func<Task> acao)
    {
        try
        {
            await acao();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }
}
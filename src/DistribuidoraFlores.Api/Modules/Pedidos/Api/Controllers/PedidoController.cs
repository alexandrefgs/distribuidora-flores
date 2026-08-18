using Microsoft.AspNetCore.Mvc;
using DistribuidoraFlores.Api.Modules.Pedidos.Api.DTOs;
using DistribuidoraFlores.Api.Modules.Pedidos.Application.DTOs;
using DistribuidoraFlores.Api.Modules.Pedidos.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Pedidos.Application.UseCases;

namespace DistribuidoraFlores.Api.Modules.Pedidos.Api.Controllers;

[ApiController]
[Route("api/pedidos")]
public class PedidoController : ControllerBase
{
    private readonly CriarPedidoUseCase _criarPedidoUseCase;
    private readonly AprovarPedidoUseCase _aprovarPedidoUseCase;
    private readonly MarcarPedidoSeparadoUseCase _marcarPedidoSeparadoUseCase;
    private readonly MarcarPedidoEmRotaUseCase _marcarPedidoEmRotaUseCase;
    private readonly MarcarPedidoEntregueUseCase _marcarPedidoEntregueUseCase;
    private readonly CancelarPedidoUseCase _cancelarPedidoUseCase;
    private readonly IPedidoRepository _pedidoRepository;

    public PedidoController(
        CriarPedidoUseCase criarPedidoUseCase,
        AprovarPedidoUseCase aprovarPedidoUseCase,
        MarcarPedidoSeparadoUseCase marcarPedidoSeparadoUseCase,
        MarcarPedidoEmRotaUseCase marcarPedidoEmRotaUseCase,
        MarcarPedidoEntregueUseCase marcarPedidoEntregueUseCase,
        CancelarPedidoUseCase cancelarPedidoUseCase,
        IPedidoRepository pedidoRepository)
    {
        _criarPedidoUseCase = criarPedidoUseCase;
        _aprovarPedidoUseCase = aprovarPedidoUseCase;
        _marcarPedidoSeparadoUseCase = marcarPedidoSeparadoUseCase;
        _marcarPedidoEmRotaUseCase = marcarPedidoEmRotaUseCase;
        _marcarPedidoEntregueUseCase = marcarPedidoEntregueUseCase;
        _cancelarPedidoUseCase = cancelarPedidoUseCase;
        _pedidoRepository = pedidoRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarPedidoRequest request)
    {
        try
        {
            var itens = request.Itens
                .Select(i => new ItemPedidoInput(i.ProdutoId, i.Quantidade))
                .ToList();

            var id = await _criarPedidoUseCase.ExecutarAsync(request.ClienteId, itens);

            return CreatedAtAction(nameof(ObterPorId), new { id }, new { id });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(new { erro = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(id);

        if (pedido is null)
            return NotFound();

        return Ok(PedidoResponse.FromDomain(pedido));
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var pedidos = await _pedidoRepository.ListarTodosAsync();
        return Ok(pedidos.Select(PedidoResponse.FromDomain));
    }

    [HttpGet("cliente/{clienteId:guid}")]
    public async Task<IActionResult> ListarPorCliente(Guid clienteId)
    {
        var pedidos = await _pedidoRepository.ListarPorClienteAsync(clienteId);
        return Ok(pedidos.Select(PedidoResponse.FromDomain));
    }

    [HttpPatch("{id:guid}/aprovar")]
    public async Task<IActionResult> Aprovar(Guid id) => await ExecutarTransicao(() => _aprovarPedidoUseCase.ExecutarAsync(id));

    [HttpPatch("{id:guid}/separar")]
    public async Task<IActionResult> Separar(Guid id) => await ExecutarTransicao(() => _marcarPedidoSeparadoUseCase.ExecutarAsync(id));

    [HttpPatch("{id:guid}/em-rota")]
    public async Task<IActionResult> EmRota(Guid id) => await ExecutarTransicao(() => _marcarPedidoEmRotaUseCase.ExecutarAsync(id));

    [HttpPatch("{id:guid}/entregar")]
    public async Task<IActionResult> Entregar(Guid id) => await ExecutarTransicao(() => _marcarPedidoEntregueUseCase.ExecutarAsync(id));

    [HttpPatch("{id:guid}/cancelar")]
    public async Task<IActionResult> Cancelar(Guid id) => await ExecutarTransicao(() => _cancelarPedidoUseCase.ExecutarAsync(id));

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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DistribuidoraFlores.Api.Modules.Catalogo.Api.DTOs;
using DistribuidoraFlores.Api.Modules.Catalogo.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Catalogo.Application.UseCases;

namespace DistribuidoraFlores.Api.Modules.Catalogo.Api.Controllers;

[ApiController]
[Route("api/produtos")]
[Authorize]
public class ProdutoController : ControllerBase
{
    private readonly CriarProdutoUseCase _criarProdutoUseCase;
    private readonly AdicionarLoteUseCase _adicionarLoteUseCase;
    private readonly IProdutoRepository _produtoRepository;

    public ProdutoController(
        CriarProdutoUseCase criarProdutoUseCase,
        AdicionarLoteUseCase adicionarLoteUseCase,
        IProdutoRepository produtoRepository)
    {
        _criarProdutoUseCase = criarProdutoUseCase;
        _adicionarLoteUseCase = adicionarLoteUseCase;
        _produtoRepository = produtoRepository;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Criar([FromBody] CriarProdutoRequest request)
    {
        try
        {
            var id = await _criarProdutoUseCase.ExecutarAsync(
                request.Nome,
                request.Categoria,
                request.UnidadeMedida,
                request.PrecoUnitario
            );

            return CreatedAtAction(nameof(ObterPorId), new { id }, new { id });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id);

        if (produto is null)
            return NotFound();

        return Ok(ProdutoResponse.FromDomain(produto));
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var produtos = await _produtoRepository.ListarAtivosAsync();
        return Ok(produtos.Select(ProdutoResponse.FromDomain));
    }

    [HttpPost("{id:guid}/lotes")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdicionarLote(Guid id, [FromBody] AdicionarLoteRequest request)
    {
        try
        {
            await _adicionarLoteUseCase.ExecutarAsync(id, request.Quantidade, request.DataValidade);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }
}
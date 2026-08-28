using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    private readonly DefinirImagemProdutoUseCase _definirImagemProdutoUseCase;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IWebHostEnvironment _ambiente;

    public ProdutoController(
        CriarProdutoUseCase criarProdutoUseCase,
        AdicionarLoteUseCase adicionarLoteUseCase,
        DefinirImagemProdutoUseCase definirImagemProdutoUseCase,
        IProdutoRepository produtoRepository,
        IWebHostEnvironment ambiente)
    {
        _criarProdutoUseCase = criarProdutoUseCase;
        _adicionarLoteUseCase = adicionarLoteUseCase;
        _definirImagemProdutoUseCase = definirImagemProdutoUseCase;
        _produtoRepository = produtoRepository;
        _ambiente = ambiente;
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

    [HttpPost("{id:guid}/imagem")]
    [Authorize(Roles = "Admin")]
    [RequestSizeLimit(5_000_000)] // 5 MB
    public async Task<IActionResult> DefinirImagem(Guid id, IFormFile arquivo)
    {
        if (arquivo is null || arquivo.Length == 0)
            return BadRequest(new { erro = "Nenhum arquivo enviado." });

        var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();

        if (!extensoesPermitidas.Contains(extensao))
            return BadRequest(new { erro = "Formato de imagem não suportado. Use JPG, PNG ou WEBP." });

        var pastaUploads = Path.Combine(_ambiente.WebRootPath, "uploads", "produtos");
        Directory.CreateDirectory(pastaUploads);

        var nomeArquivo = $"{id}{extensao}";
        var caminhoCompleto = Path.Combine(pastaUploads, nomeArquivo);

        using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
        {
            await arquivo.CopyToAsync(stream);
        }

        var imagemUrl = $"/uploads/produtos/{nomeArquivo}";

        try
        {
            await _definirImagemProdutoUseCase.ExecutarAsync(id, imagemUrl);
            return Ok(new { imagemUrl });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }
}
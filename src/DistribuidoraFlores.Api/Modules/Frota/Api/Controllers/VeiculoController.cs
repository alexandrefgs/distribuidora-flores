using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DistribuidoraFlores.Api.Modules.Frota.Api.DTOs;
using DistribuidoraFlores.Api.Modules.Frota.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Frota.Application.UseCases;

namespace DistribuidoraFlores.Api.Modules.Frota.Api.Controllers;

[ApiController]
[Route("api/veiculos")]
[Authorize(Roles = "Admin")]
public class VeiculoController : ControllerBase
{
    private readonly CadastrarVeiculoUseCase _cadastrarVeiculoUseCase;
    private readonly IVeiculoRepository _veiculoRepository;

    public VeiculoController(CadastrarVeiculoUseCase cadastrarVeiculoUseCase, IVeiculoRepository veiculoRepository)
    {
        _cadastrarVeiculoUseCase = cadastrarVeiculoUseCase;
        _veiculoRepository = veiculoRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarVeiculoRequest request)
    {
        try
        {
            var id = await _cadastrarVeiculoUseCase.ExecutarAsync(request.Placa, request.Modelo, request.CapacidadeKg);
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
        var veiculo = await _veiculoRepository.ObterPorIdAsync(id);
        return veiculo is null ? NotFound() : Ok(VeiculoResponse.FromDomain(veiculo));
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var veiculos = await _veiculoRepository.ListarAtivosAsync();
        return Ok(veiculos.Select(VeiculoResponse.FromDomain));
    }
}
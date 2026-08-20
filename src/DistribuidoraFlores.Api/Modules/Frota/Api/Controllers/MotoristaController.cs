using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DistribuidoraFlores.Api.Modules.Frota.Api.DTOs;
using DistribuidoraFlores.Api.Modules.Frota.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Frota.Application.UseCases;

namespace DistribuidoraFlores.Api.Modules.Frota.Api.Controllers;

[ApiController]
[Route("api/motoristas")]
[Authorize(Roles = "Admin")]
public class MotoristaController : ControllerBase
{
    private readonly CadastrarMotoristaUseCase _cadastrarMotoristaUseCase;
    private readonly IMotoristaRepository _motoristaRepository;

    public MotoristaController(CadastrarMotoristaUseCase cadastrarMotoristaUseCase, IMotoristaRepository motoristaRepository)
    {
        _cadastrarMotoristaUseCase = cadastrarMotoristaUseCase;
        _motoristaRepository = motoristaRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarMotoristaRequest request)
    {
        try
        {
            var id = await _cadastrarMotoristaUseCase.ExecutarAsync(request.Nome, request.Telefone, request.Cnh);
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
        var motorista = await _motoristaRepository.ObterPorIdAsync(id);
        return motorista is null ? NotFound() : Ok(MotoristaResponse.FromDomain(motorista));
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var motoristas = await _motoristaRepository.ListarAtivosAsync();
        return Ok(motoristas.Select(MotoristaResponse.FromDomain));
    }
}
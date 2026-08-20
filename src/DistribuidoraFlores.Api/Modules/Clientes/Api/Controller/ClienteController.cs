using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DistribuidoraFlores.Api.Modules.Clientes.Api.DTOs;
using DistribuidoraFlores.Api.Modules.Clientes.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Clientes.Application.UseCases;

namespace DistribuidoraFlores.Api.Modules.Clientes.Api.Controllers;

[ApiController]
[Route("api/clientes")]
[Authorize]
public class ClienteController : ControllerBase
{
    private readonly CadastrarClienteUseCase _cadastrarClienteUseCase;
    private readonly AtivarClienteUseCase _ativarClienteUseCase;
    private readonly DesativarClienteUseCase _desativarClienteUseCase;
    private readonly IClienteRepository _clienteRepository;

    public ClienteController(
        CadastrarClienteUseCase cadastrarClienteUseCase,
        AtivarClienteUseCase ativarClienteUseCase,
        DesativarClienteUseCase desativarClienteUseCase,
        IClienteRepository clienteRepository)
    {
        _cadastrarClienteUseCase = cadastrarClienteUseCase;
        _ativarClienteUseCase = ativarClienteUseCase;
        _desativarClienteUseCase = desativarClienteUseCase;
        _clienteRepository = clienteRepository;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarClienteRequest request)
    {
        try
        {
            var id = await _cadastrarClienteUseCase.ExecutarAsync(
                request.NomeFantasia,
                request.Documento,
                request.Telefone,
                request.Email,
                request.Endereco
            );

            return CreatedAtAction(nameof(ObterPorId), new { id }, new { id });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { erro = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        if (!PodeAcessarCliente(id))
            return Forbid();

        var cliente = await _clienteRepository.ObterPorIdAsync(id);

        if (cliente is null)
            return NotFound();

        return Ok(ClienteResponse.FromDomain(cliente));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Listar()
    {
        var clientes = await _clienteRepository.ListarAtivosAsync();
        return Ok(clientes.Select(ClienteResponse.FromDomain));
    }

    [HttpPatch("{id:guid}/ativar")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Ativar(Guid id)
    {
        try
        {
            await _ativarClienteUseCase.ExecutarAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/desativar")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Desativar(Guid id)
    {
        try
        {
            await _desativarClienteUseCase.ExecutarAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }

    // Admin pode acessar qualquer cliente; Comerciante só o próprio (via clienteId do token)
    private bool PodeAcessarCliente(Guid clienteId)
    {
        if (User.IsInRole("Admin"))
            return true;

        var clienteIdClaim = User.FindFirst("clienteId")?.Value;
        return clienteIdClaim is not null && Guid.TryParse(clienteIdClaim, out var idDoToken) && idDoToken == clienteId;
    }
}
using Microsoft.AspNetCore.Mvc;
using DistribuidoraFlores.Api.Modules.Identidade.Api.DTOs;
using DistribuidoraFlores.Api.Modules.Identidade.Application.UseCases;

namespace DistribuidoraFlores.Api.Modules.Identidade.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RegistrarComercianteUseCase _registrarComercianteUseCase;
    private readonly LoginUseCase _loginUseCase;

    public AuthController(RegistrarComercianteUseCase registrarComercianteUseCase, LoginUseCase loginUseCase)
    {
        _registrarComercianteUseCase = registrarComercianteUseCase;
        _loginUseCase = loginUseCase;
    }

    [HttpPost("registrar-comerciante")]
    public async Task<IActionResult> RegistrarComerciante([FromBody] RegistrarComercianteRequest request)
    {
        try
        {
            var id = await _registrarComercianteUseCase.ExecutarAsync(
                request.NomeFantasia, request.Documento, request.Telefone,
                request.Endereco, request.Email, request.Senha);

            return CreatedAtAction(nameof(Login), new { id });
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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var token = await _loginUseCase.ExecutarAsync(request.Email, request.Senha);
            return Ok(new { token });
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { erro = ex.Message });
        }
    }
}
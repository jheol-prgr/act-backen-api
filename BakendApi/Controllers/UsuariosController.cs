using BakendApi.Data;
using BakendApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BakendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController(HeladeriaRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> ObtenerUsuarios(CancellationToken cancellationToken)
    {
        return Ok(await repository.ObtenerUsuariosAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Usuario>> ObtenerUsuarioPorId(int id, CancellationToken cancellationToken)
    {
        var usuario = await repository.ObtenerUsuarioPorIdAsync(id, cancellationToken);

        if (usuario is null)
        {
            return NotFound(new { mensaje = "Usuario no encontrado" });
        }

        return Ok(usuario);
    }

    [HttpPost]
    public async Task<ActionResult<Usuario>> CrearUsuario(CrearUsuarioRequest request, CancellationToken cancellationToken)
    {
        if (!EsUsuarioValido(request.Nombre, request.Email, request.Rol) || string.IsNullOrWhiteSpace(request.PasswordHash))
        {
            return BadRequest(new { mensaje = "Complete los campos obligatorios del usuario" });
        }

        try
        {
            var usuario = await repository.CrearUsuarioAsync(request, cancellationToken);
            return CreatedAtAction(nameof(ObtenerUsuarioPorId), new { id = usuario.Id }, usuario);
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            return Conflict(new { mensaje = "Ya existe un usuario con ese email" });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> ActualizarUsuario(int id, ActualizarUsuarioRequest request, CancellationToken cancellationToken)
    {
        if (!EsUsuarioValido(request.Nombre, request.Email, request.Rol))
        {
            return BadRequest(new { mensaje = "Complete los campos obligatorios del usuario" });
        }

        try
        {
            var actualizado = await repository.ActualizarUsuarioAsync(id, request, cancellationToken);
            if (!actualizado)
            {
                return NotFound(new { mensaje = "Usuario no encontrado" });
            }

            return NoContent();
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            return Conflict(new { mensaje = "Ya existe un usuario con ese email" });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> EliminarUsuario(int id, CancellationToken cancellationToken)
    {
        var eliminado = await repository.DesactivarUsuarioAsync(id, cancellationToken);
        if (!eliminado)
        {
            return NotFound(new { mensaje = "Usuario no encontrado" });
        }

        return NoContent();
    }

    private static bool EsUsuarioValido(string nombre, string email, string rol)
    {
        return !string.IsNullOrWhiteSpace(nombre)
            && !string.IsNullOrWhiteSpace(email)
            && (rol == "admin" || rol == "empleado");
    }
}
